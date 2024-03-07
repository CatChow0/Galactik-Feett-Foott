using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // Tableau des préfabriqués des obstacles
    public Terrain terrain; // Référence au terrain
    public int numberOfObstacles = 10; // Nombre d'obstacles à générer
    public float minDistance = 2f; // Distance minimale entre les obstacles

    void Start()
    {
        GenerateObstacles();
    }

    void GenerateObstacles()
    {
        List<Vector3> points = GenerateRandomPointsOnTerrain(numberOfObstacles);

        foreach (Vector3 point in points)
        {
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject obstacle = Instantiate(obstaclePrefab, point, Quaternion.identity);

            // Assurez-vous que l'obstacle est positionné au-dessus du terrain
            Vector3 terrainPoint = new Vector3(point.x, terrain.SampleHeight(point) + obstacle.transform.localScale.y / 2f, point.z);
            obstacle.transform.position = terrainPoint;
        }
    }

    List<Vector3> GenerateRandomPointsOnTerrain(int numberOfPoints)
    {
        List<Vector3> points = new List<Vector3>();
        Bounds terrainBounds = terrain.terrainData.bounds;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float randomX = Random.Range(terrainBounds.min.x, terrainBounds.max.x);
            float randomZ = Random.Range(terrainBounds.min.z, terrainBounds.max.z);
            Vector3 randomPoint = new Vector3(randomX, 0f, randomZ); // Assurez-vous que la hauteur est ajustée plus tard

            // Récupérer la hauteur du terrain au point généré
            float terrainHeight = terrain.SampleHeight(randomPoint);

            // Si le point généré est en dessous de la hauteur du terrain, ajustez sa hauteur
            if (randomPoint.y < terrainHeight)
            {
                randomPoint.y = terrainHeight;
            }

            points.Add(randomPoint);
        }

        return points;
    }
}
