using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileForce = 10f;

    [Header("Combo: tiro em leque")]
    [Tooltip("Duração do combo em segundos.")]
    public float comboDuration = 5f;
    [Tooltip("Tempo máximo acumulado ao pegar vários bônus seguidos.")]
    public float maxComboDuration = 15f;
    [Tooltip("Quantidade de balas no leque.")]
    public int spreadCount = 5;
    [Tooltip("Ângulo total do leque em graus (ex.: 30 = de -15° a +15°).")]
    public float spreadAngle = 30f;

    private float spreadEndTime = 0f;

    // Mantido com este nome para não quebrar outros scripts que leiam o estado
    public bool doubleShot => Time.time < spreadEndTime;

    // Segundos restantes do combo (útil para mostrar numa barra/contador de UI)
    public float ComboTimeLeft => Mathf.Max(0f, spreadEndTime - Time.time);

    public void Shoot()
    {
        Debug.Log("ATIROU");

        Transform cam = Camera.main.transform;

        if (doubleShot)
            ShootSpread(cam, spreadCount, spreadAngle);
        else
            ShootSpread(cam, 1, 0f);
    }

    // Balas saindo da mesma posição, abertas em leque na horizontal
    void ShootSpread(Transform cam, int count, float totalAngle)
    {
        count = Mathf.Max(1, count);

        Vector3 spawnPosition = cam.position + cam.forward * 1.5f;

        for (int i = 0; i < count; i++)
        {
            // Distribui os ângulos de -total/2 até +total/2
            float t = count == 1 ? 0.5f : (float)i / (count - 1);
            float angle = Mathf.Lerp(-totalAngle / 2f, totalAngle / 2f, t);

            Vector3 direction = Quaternion.AngleAxis(angle, cam.up) * cam.forward;

            SpawnProjectile(spawnPosition, direction);
        }
    }

    void SpawnProjectile(Vector3 position, Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
            rb.velocity = direction * projectileForce;
    }

    // Cada bônus coletado soma comboDuration ao tempo restante (limitado a maxComboDuration)
    public void ActivateDoubleShot()
    {
        float start = Mathf.Max(Time.time, spreadEndTime);
        spreadEndTime = Mathf.Min(start + comboDuration, Time.time + maxComboDuration);

        Debug.Log($"TIRO EM LEQUE ATIVADO! Restam {ComboTimeLeft:F1}s");
    }
}