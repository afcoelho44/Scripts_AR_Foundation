using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnWall : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToPlace;

    [SerializeField]
    private float offsetFromWall = 0.02f;

    private ARPlaneManager planeManager;

    private bool objectPlaced = false;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void Update()
    {
        if (objectPlaced)
            return;

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.alignment != PlaneAlignment.Vertical)
                continue;

            Debug.Log("PAREDE DETECTADA!");

            // Centro do plano em coordenadas do mundo
            Vector3 wallPosition =
                plane.transform.TransformPoint(plane.center);

            // Direção perpendicular à parede
            Vector3 wallNormal = plane.transform.up;

            // Coloca o objeto um pouquinho para fora da parede
            Vector3 objectPosition =
                wallPosition + wallNormal * offsetFromWall;

            // Cria o objeto
            GameObject obj = Instantiate(
                objectToPlace,
                objectPosition,
                plane.transform.rotation
            );

            // Reduz o tamanho apenas para o teste
            obj.transform.localScale = Vector3.one * 0.2f;

            Debug.Log("OBJETO COLOCADO NA PAREDE!");

            objectPlaced = true;

            break;
        }
    }
}