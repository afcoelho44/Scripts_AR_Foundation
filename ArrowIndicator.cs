using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public RectTransform arrow;

    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemys.Length == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        GameObject nearest = GetNearestEnemy(enemys);

        Vector3 viewportPos = arCamera.WorldToViewportPoint(nearest.transform.position);

        bool isInFrontOfCamera = viewportPos.z > 0;
        bool isInsideScreen =
            viewportPos.x >= 0f && viewportPos.x <= 1f &&
            viewportPos.y >= 0f && viewportPos.y <= 1f;

        // Se o inimigo/quadrado estiver visível na tela, a seta some
        if (isInFrontOfCamera && isInsideScreen)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);

        // Posição do inimigo em relação à câmera
        Vector3 localPos = arCamera.transform.InverseTransformPoint(nearest.transform.position);

        // Se estiver mais para a esquerda
        if (localPos.x < -0.5f)
        {
            PointLeft();
        }
        // Se estiver mais para a direita
        else if (localPos.x > 0.5f)
        {
            PointRight();
        }
        // Se estiver mais centralizado, aponta para frente
        else
        {
            PointForward();
        }
    }

    GameObject GetNearestEnemy(GameObject[] enemys)
    {
        GameObject nearest = enemys[0];
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemys)
        {
            float distance = Vector3.Distance(
                arCamera.transform.position,
                enemy.transform.position
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    void PointRight()
    {
        // Sua seta original aponta para a direita
        arrow.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void PointLeft()
    {
        arrow.localRotation = Quaternion.Euler(0f, 0f, 180f);
    }

    void PointForward()
    {
        // Aponta para cima/frente
        arrow.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }
}