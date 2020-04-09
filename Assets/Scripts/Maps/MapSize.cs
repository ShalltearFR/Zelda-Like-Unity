using UnityEngine;
using UnityEngine.Tilemaps;

// Script permettant de calculer les limites de vision de la caméra
public class MapSize : MonoBehaviour
{
    public Vector2 sizeMin;
    public Vector2 sizeMax;
    public GameObject gameObjectMap;

    void Awake()
    {
    // Recupère le gameobject où est implanté le script
    gameObjectMap = this.gameObject;

    // Recupère les valeurs x/y min/max pour quadriller la map
    sizeMin.x = transform.GetChild(0).GetComponent<Tilemap>().cellBounds.xMin;
    sizeMin.y = transform.GetChild(0).GetComponent<Tilemap>().cellBounds.yMin;

    sizeMax.x = transform.GetChild(0).GetComponent<Tilemap>().cellBounds.xMax;
    sizeMax.y = transform.GetChild(0).GetComponent<Tilemap>().cellBounds.yMax;

    }
}
