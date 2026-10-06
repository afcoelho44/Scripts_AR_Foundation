using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Coloca um cubo sobre um plano detectado quando o usuário toca na tela.
/// Anexe ao MESMO objeto que tem XR Origin, AR Raycast Manager e AR Plane Manager.
/// Funciona com o Input Manager antigo e com o novo Input System.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceCubeOnPlane : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float cubeSize = 0.15f;
    [SerializeField] private bool placeMultiple = true;

    private ARRaycastManager raycastManager;
    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject spawnedCube;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (!TryGetTapPosition(out Vector2 screenPos))
            return;

        Debug.Log($"[PlaceCube] Toque em {screenPos}");

        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;
            Vector3 position = pose.position + pose.up * (cubeSize * 0.5f);

            if (placeMultiple || spawnedCube == null)
                spawnedCube = CreateCube(position, pose.rotation);
            else
                spawnedCube.transform.SetPositionAndRotation(position, pose.rotation);

            Debug.Log($"[PlaceCube] Cubo em {position}");
        }
        else
        {
            Debug.Log("[PlaceCube] Raycast não acertou nenhum plano");
        }
    }

    private bool TryGetTapPosition(out Vector2 position)
    {
#if ENABLE_INPUT_SYSTEM
        var touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
        {
            position = touchscreen.primaryTouch.position.ReadValue();
            return true;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
#endif
        position = default;
        return false;
    }

    private GameObject CreateCube(Vector3 position, Quaternion rotation)
    {
        if (cubePrefab != null)
            return Instantiate(cubePrefab, position, rotation);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetPositionAndRotation(position, rotation);
        cube.transform.localScale = Vector3.one * cubeSize;
        return cube;
    }
}