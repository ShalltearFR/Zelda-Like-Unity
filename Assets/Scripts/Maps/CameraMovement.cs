using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

// Script permettant à la caméra de suivre le joueur

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothing;
    public VectorValue startingPlayerPosition;
    private int fpsLimite = 60;
    public GameObject fadeOutPanel;
    private bool initCamera = false;
    private SaveManager saveManager;

    [Header("Limitation Camera")]
    public Vector2 minPosition;
    public Vector2 maxPosition;

    [Header("Taille Map")]
    public Vector2 mapSizeMin;
    public Vector2 mapSizeMax;
    public GameObject activeMap;

    [Header("Résolution écran")]
    public Vector3 ScreenResolution;

    public IEnumerator InitCamera()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu") { gameObject.transform.position = new Vector3(target.position.x, target.position.y, -4); }
        // Animation de fondu de sortie
        if (fadeOutPanel != null)
        {
            GameObject panel = Instantiate(fadeOutPanel, target.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(this.gameObject.transform);
            Destroy(panel, 2.50f);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    private void GetActiveMap()
    {
        // Recupere le nom de la map
        if (saveManager.onlyTeleport.RuntimeValue == "")
        {
            StringValue activeMapTemp = GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[8] as StringValue;
            string map = activeMapTemp.RuntimeValue;
            activeMap = GameObject.Find(map);
        } else
        {
            StringValue activeMapTemp = GameObject.Find("Save Manager").GetComponent<SaveManager>().onlyTeleport as StringValue;
            string map = activeMapTemp.RuntimeValue;
            activeMap = GameObject.Find(map);
            activeMapTemp.RuntimeValue = "";
        }

        gameObject.GetComponent<Transform>().position = new Vector3(startingPlayerPosition.teleporationValue.x, startingPlayerPosition.teleporationValue.y, -4);
        Init();
    }

    private void Init()
    {
        if (activeMap == null) { activeMap = GameObject.Find("Map1"); }

        // Recupère les valeur x/y min/max sur la map active
        mapSizeMin = activeMap.GetComponent<MapSize>().sizeMin;
        mapSizeMax = activeMap.GetComponent<MapSize>().sizeMax;

        // Calcule la limite de déplacement de la map
        minPosition.x = ScreenResolution.x + mapSizeMin.x;
        minPosition.y = ScreenResolution.y + mapSizeMin.y;

        maxPosition.x = mapSizeMax.x - ScreenResolution.x;
        maxPosition.y = mapSizeMax.y - ScreenResolution.y;

        if (minPosition.x > maxPosition.x)
        {
            float xPositionTemp = minPosition.x;
            minPosition.x = maxPosition.x;
            maxPosition.x = xPositionTemp;
        }

        if (GameObject.Find("Set Teleport") != null) { GameObject.Find("Set Teleport").GetComponent<SetTeleport>().Teleport(); }

        StartCoroutine(Charging());
        //InvokeRepeating("InitCamera", 0f, 0.00f);
        RefreshCamLimit(activeMap);
    }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // Recupère la résolution afin d'adapter la cam
            ScreenResolution = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        }
    }

    void Start()
    {
        // Limite les FPS du jeu à 60 (peut etre ...)
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();
            target = GameObject.FindWithTag("Player").transform;

            //        Debug.Log("Screen.width =" + Screen.width);
            //        Debug.Log("Screen.height = " + Screen.height);
            //       Debug.Log("Camera.main.transform.position.z = " + Camera.main.transform.position.z);

            //RefreshCamLimit(activeMap);


            InvokeRepeating("GetActiveMap", 0.10f, 0.00f);
        } else  { StartCoroutine(InitCamera()); }
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
        initCamera = true;
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (initCamera)
            {
                // Camera qui suit le personnage
                if (GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.bloquing)
                {
                    if (transform.position != target.position)
                    {
                        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

                        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
                        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

                        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
                    }
                }
            }
        }
    }

    private void Update()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;
    }
}
