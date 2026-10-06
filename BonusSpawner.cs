using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public GameObject[] bonusPrefabs;

    public float minSpawnDistance = 3f;
    public float maxSpawnDistance = 5f;

    public float minHeight = -1f;
    public float maxHeight = 1f;

    public float spawnInterval = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnBonus), 1f, spawnInterval);
    }

    void SpawnBonus()
    {
        if (bonusPrefabs.Length == 0)
            return;

        Transform cam = Camera.main.transform;

        // Direção para frente da câmera
        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        // Ângulo aleatório
        float angle = Random.Range(-45f, 45f);

        Vector3 direction =
            Quaternion.Euler(0f, angle, 0f) * forward;

        // Distância aleatória
        float distance =
            Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 spawnPos =
            cam.position + direction * distance;

        // Altura aleatória
        spawnPos.y += Random.Range(minHeight, maxHeight);

        // Escolhe um bônus aleatório
        int randomIndex =
            Random.Range(0, bonusPrefabs.Length);

        GameObject selectedBonus =
            bonusPrefabs[randomIndex];

        Instantiate(
            selectedBonus,
            spawnPos,
            Quaternion.identity
        );
    }
}