using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public ObstacleData obstacleData; // Reference to obstacle data
    public GameObject obstaclePrefab; // Prefab for obstacles
    public Transform gridParent; // Parent object containing the grid tiles

    void Start()
    {
        PlaceObstacles();
    }

    private void PlaceObstacles()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                int index = i * 10 + j;

                if (obstacleData.blockedTiles[index])
                {
                    Vector3 obstaclePosition = new Vector3(i * 2f, 0.5f, j * 2f);
                    Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, gridParent);
                }
            }
        }
    }
}
