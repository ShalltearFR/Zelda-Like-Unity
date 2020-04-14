using UnityEngine;
using System.Collections;

// Script permettant à la caméra de suivre le joueur

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothing;
    public VectorValue startingPlayerPosition;
    private int fpsLimite = 60;

    [Header("Limitation Camera")]
    public Vector2 minPosition;
    public Vector2 maxPosition;

    [Header("Taille Map")]
    public Vector2 mapSizeMin;
    public Vector2 mapSizeMax;
    public GameObject activeMap;

    [Header("Résolution écran")]
    public Vector3 ScreenResolution;

    void Start()
    {
        // Limite les FPS du jeu à 60 (peut etre ...)
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;
        
        
        // Recupère la résolution afin d'adapter la cam
        ScreenResolution = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Recupère les valeur x/y min/max sur la map active
        mapSizeMin = activeMap.GetComponent<MapSize>().sizeMin;
        mapSizeMax = activeMap.GetComponent<MapSize>().sizeMax;

        // Calcule la limite de déplacement de la map
        minPosition.x = ScreenResolution.x + mapSizeMin.x;
        minPosition.y = ScreenResolution.y + mapSizeMin.y;

        maxPosition.x = mapSizeMax.x - ScreenResolution.x;
        maxPosition.y = mapSizeMax.y - ScreenResolution.y;

        gameObject.GetComponent<Transform>().position = new Vector3(startingPlayerPosition.initialValue.x, startingPlayerPosition.initialValue.y, -4);

        StartCoroutine(Charging());
    }

    private IEnumerator Charging()
    {
        // Met en pause un bref delai le temps de chargement
        yield return new WaitForSeconds(0.05f);
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.idle;
    }

    public void RefreshCamLimit(GameObject gameobject)
    {
        // Met à jour les limites de positions de la camera x/y min/max
        activeMap = gameobject;

        mapSizeMin = gameobject.GetComponent<MapSize>().sizeMin;
        mapSizeMax = gameobject.GetComponent<MapSize>().sizeMax;

        minPosition.x = ScreenResolution.x + mapSizeMin.x + activeMap.transform.position.x;
        minPosition.y = ScreenResolution.y + mapSizeMin.y + activeMap.transform.position.y;

        maxPosition.x = mapSizeMax.x - ScreenResolution.x + activeMap.transform.position.x;
        maxPosition.y = mapSizeMax.y - ScreenResolution.y + activeMap.transform.position.y;
    }

    void LateUpdate()
    {
        // Camera qui suit le personnage
        if (GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.bloquing) { 
            if (transform.position != target.position)
            {
                Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

                targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
            }
        }
    }

    private void Update()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;
    }
}
