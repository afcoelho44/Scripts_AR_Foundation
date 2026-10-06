using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool alreadyHit = false;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (alreadyHit) return;

        Debug.Log("Acertou: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            alreadyHit = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnemyDestroyed();
            }

            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }
    }
}