using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1.5f;
    public int damage = 10;
    public float attackDistance = 0.5f;

    Transform cameraTransform;
    PlayerHealth playerHealth;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        Vector3 dir =
            (cameraTransform.position - transform.position).normalized;

        float currentSpeed = speed;

        if (GameManager.Instance != null)
        {
            currentSpeed *= GameManager.Instance.enemySpeedMultiplier;
        }

        transform.position +=
            dir * currentSpeed * Time.deltaTime;

        float distance =
            Vector3.Distance(
                transform.position,
                cameraTransform.position
            );

        if (distance < attackDistance)
        {
            Debug.Log("Player tomou dano");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}