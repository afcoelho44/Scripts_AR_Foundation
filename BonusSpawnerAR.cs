using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Instancia bônus sobre planos detectados (chão, mesas, paredes).
/// Anexe ao mesmo objeto que tem o XR Origin e o AR Plane Manager.
/// </summary>
public class BonusSpawnerAR : MonoBehaviour
{
    [Header("AR")]
    [Tooltip("Se vazio, procura o AR Plane Manager existente na cena (o do XR Origin).")]
    public ARPlaneManager planeManager;
    [Header("Bônus")]
    public GameObject[] bonusPrefabs;
    public float spawnInterval = 5f;
    public int maxActiveBonuses = 5;
    [Tooltip("Tempo em segundos até o bônus desaparecer. Use 0 para nunca desaparecer.")]
    public float bonusLifetime = 5f;

    [Header("Distância da câmera (metros)")]
    public float minSpawnDistance = 0.5f;
    public float maxSpawnDistance = 5f;

    [Header("Planos aceitos")]
    public bool allowHorizontal = true;
    public bool allowVertical = true;
    [Tooltip("Área mínima do plano em m² para receber bônus.")]
    public float minPlaneArea = 0.1f;

    [Header("Posicionamento")]
    [Tooltip("Afastamento da superfície ao longo da normal (m). Use a altura do pivô se necessário.")]
    public float heightOffset = 0.05f;
    [Tooltip("Tentativas de achar um ponto válido a cada spawn.")]
    public int maxAttempts = 20;

    private readonly List<GameObject> activeBonuses = new List<GameObject>();
    private readonly List<ARPlane> candidatePlanes = new List<ARPlane>();

    private void Awake()
    {
        if (planeManager == null)
            planeManager = FindObjectOfType<ARPlaneManager>();

        if (planeManager == null)
            Debug.LogError("[BonusSpawnerAR] Nenhum ARPlaneManager encontrado na cena.");
    }

    private void Start()
    {
        if (planeManager != null)
            InvokeRepeating(nameof(SpawnBonus), 1f, spawnInterval);
    }

    private void SpawnBonus()
    {
        if (bonusPrefabs == null || bonusPrefabs.Length == 0)
            return;

        activeBonuses.RemoveAll(b => b == null);
        if (activeBonuses.Count >= maxActiveBonuses)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        CollectCandidatePlanes();
        if (candidatePlanes.Count == 0)
            return;

        for (int i = 0; i < maxAttempts; i++)
        {
            ARPlane plane = candidatePlanes[Random.Range(0, candidatePlanes.Count)];

            if (!TryGetRandomPointOnPlane(plane, out Vector3 worldPoint))
                continue;

            float dist = Vector3.Distance(cam.transform.position, worldPoint);
            if (dist < minSpawnDistance || dist > maxSpawnDistance)
                continue;

            Vector3 position = worldPoint + plane.normal * heightOffset;
            Quaternion rotation = GetRotation(plane);

            GameObject prefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];
            GameObject bonus = Instantiate(prefab, position, rotation);
            activeBonuses.Add(bonus);

            if (bonusLifetime > 0f)
                Destroy(bonus, bonusLifetime);

            return;
        }
    }

    private void CollectCandidatePlanes()
    {
        candidatePlanes.Clear();

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.trackingState != TrackingState.Tracking)
                continue;

            if (plane.size.x * plane.size.y < minPlaneArea)
                continue;

            bool horizontal = plane.alignment == PlaneAlignment.HorizontalUp;
            bool vertical = plane.alignment == PlaneAlignment.Vertical;

            if ((horizontal && allowHorizontal) || (vertical && allowVertical))
                candidatePlanes.Add(plane);
        }
    }

    // Sorteia um ponto no retângulo do plano e confirma que está dentro do polígono real
    private bool TryGetRandomPointOnPlane(ARPlane plane, out Vector3 worldPoint)
    {
        worldPoint = default;

        NativeArray<Vector2> boundary = plane.boundary;
        if (!boundary.IsCreated || boundary.Length < 3)
            return false;

        Vector2 local = new Vector2(
            plane.center.x + Random.Range(-plane.extents.x, plane.extents.x),
            plane.center.y + Random.Range(-plane.extents.y, plane.extents.y));

        if (!IsInsidePolygon(local, boundary))
            return false;

        // Espaço local do plano: X/Z formam a superfície, Y é a normal
        worldPoint = plane.transform.TransformPoint(new Vector3(local.x, 0f, local.y));
        return true;
    }

    private Quaternion GetRotation(ARPlane plane)
    {
        if (plane.alignment == PlaneAlignment.Vertical)
            return Quaternion.LookRotation(plane.normal, Vector3.up); // de frente para quem olha a parede

        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f); // em pé no chão/mesa
    }

    private static bool IsInsidePolygon(Vector2 p, NativeArray<Vector2> poly)
    {
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            Vector2 a = poly[i];
            Vector2 b = poly[j];
            if ((a.y > p.y) != (b.y > p.y) &&
                p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x)
            {
                inside = !inside;
            }
        }
        return inside;
    }
}