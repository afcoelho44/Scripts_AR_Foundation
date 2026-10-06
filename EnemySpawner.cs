using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float minSpawnDistance = 3f;
    public float maxSpawnDistance = 5f;

    public float minHeight = -1f;
    public float maxHeight = 1f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, 5f);
    }

    void SpawnEnemy()
    {
        Transform cam = Camera.main.transform;

        // Pega a direção para frente da câmera, mas ignora inclinação para cima/baixo
        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        // Sorteia um ângulo dentro de 90 graus na frente
        float angle = Random.Range(-45f, 45f);

        // Gera uma direção dentro desse arco
        Vector3 direction = Quaternion.Euler(0f, angle, 0f) * forward;

        // Sorteia a distância
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 spawnPos = cam.position + direction * distance;

        // Altura aleatória
        spawnPos.y += Random.Range(minHeight, maxHeight);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}