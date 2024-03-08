using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    // --------------------------------------------- //
    // ----------------- VARIABLES ----------------- //
    // --------------------------------------------- //

    public GameObject[] obstaclePrefabs;    // Tableau des pr�fabriqu�s des obstacles
    public Terrain terrain;                 // R�f�rence au terrain
    public int numberOfObstacles = 10;      // Nombre d'obstacles � g�n�rer
    public float minDistance = 2f;          // Distance minimale entre les obstacles

    // ------------------------------------------------------- //
    // ----------------- FONCTION PRINCIPALE ----------------- //
    // ------------------------------------------------------- //
    void Start()
    {
        GenerateObstacles();
    }

    void GenerateObstacles()
    {
        List<Vector3> points = GenerateRandomPointsOnTerrain(numberOfObstacles);

        foreach (Vector3 point in points)
        {
            // V�rifie si un obstacle peut �tre plac� � ce point
            bool canPlaceObstacle = CanPlaceObstacleAtPoint(point);
            if (canPlaceObstacle)
            {
                // S�lectionne un pr�fabriqu� d'obstacle al�atoire
                GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                // Instancie l'obstacle � la position calcul�e
                GameObject obstacle = Instantiate(obstaclePrefab, point, Quaternion.identity);

                // G�n�re une �chelle al�atoire pour l'obstacle
                float randomScale = Random.Range(2f, 4f);
                obstacle.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

                // G�n�re une rotation al�atoire pour l'obstacle
                float randomRotation = Random.Range(0f, 360f);
                obstacle.transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);

                // Assurez-vous que l'obstacle est positionn� sur le terrain
                Vector3 terrainPoint = new Vector3(point.x, terrain.SampleHeight(point), point.z);
                obstacle.transform.position = terrainPoint;

                // Obtenez la normale du terrain � la position de l'obstacle
                Vector3 terrainNormal = terrain.terrainData.GetInterpolatedNormal(point.x / terrain.terrainData.size.x, point.z / terrain.terrainData.size.z);

                // Orientez l'obstacle pour qu'il suive la forme du terrain
                obstacle.transform.up = terrainNormal;

            }
        }
    }

    bool CanPlaceObstacleAtPoint(Vector3 point)
    {
        // Recherche des collisions dans un rayon autour du point
        Collider[] colliders = Physics.OverlapSphere(point, minDistance);
        foreach (Collider collider in colliders)
        {
            // V�rifie si le collider n'appartient pas au terrain
            if (collider.gameObject != terrain.gameObject)
            {
                // Si un collider autre que celui du terrain est trouv�, il y a d�j� un objet � cet emplacement
                return false;
            }
        }
        // Si aucun collider autre que celui du terrain n'est trouv�, l'emplacement est valide pour placer un obstacle
        return true;
    }

    List<Vector3> GenerateRandomPointsOnTerrain(int numberOfPoints)
    {
        List<Vector3> points = new List<Vector3>();
        Bounds terrainBounds = terrain.terrainData.bounds;

        for (int i = 0; i < numberOfPoints; i++)
        {
            // G�n�re des coordonn�es al�atoires � l'int�rieur des limites du terrain
            float randomX = Random.Range(terrainBounds.min.x, terrainBounds.max.x);
            float randomZ = Random.Range(terrainBounds.min.z, terrainBounds.max.z);
            Vector3 randomPoint = new Vector3(randomX, 0f, randomZ);

            // R�cup�rer la hauteur du terrain au point g�n�r�
            float terrainHeight = terrain.SampleHeight(randomPoint);

            // Si le point g�n�r� est en dessous de la hauteur du terrain, ajustez sa hauteur
            if (randomPoint.y < terrainHeight)
            {
                randomPoint.y = terrainHeight;
            }

            points.Add(randomPoint);
        }

        return points;
    }
}
