using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab; // Prefab for grid tiles
    public int gridSize = 10; // Size of the grid
    public float tileSpacing = 2f; // Spacing between tiles

    public Node[,] gridNodes; // 2D array to store grid nodes

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        gridNodes = new Node[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 tilePosition = new Vector3(x * tileSpacing, 0, z * tileSpacing);

                // Instantiate tile prefab
                var tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tile.name = $"Tile ({x}, {z})";

                // Add necessary components to the tile
                AddTileComponents(tile);

                // Create a walkable node for the grid
                gridNodes[x, z] = new Node(tile.transform.position, true, x, z);
            }
        }
    }

    private void AddTileComponents(GameObject tile)
    {
        if (!tile.GetComponent<TileInfo>())
        {
            tile.AddComponent<TileInfo>();
        }
        if (!tile.GetComponent<Collider>())
        {
            tile.AddComponent<BoxCollider>();
        }
    }

    [System.Serializable]
    public class Node
    {
        public Vector3 worldPosition; // Position of the node in the world
        public bool isWalkable; // Whether the node is walkable
        public int gridX, gridZ; // Grid coordinates of the node
        public Node parent; // Parent node for pathfinding
        public int gCost; // Cost to move to this node
        public int hCost; // Heuristic cost to target

        public int fCost => gCost + hCost; // Total cost for A* pathfinding

        public Node(Vector3 worldPosition, bool isWalkable, int gridX, int gridZ)
        {
            this.worldPosition = worldPosition;
            this.isWalkable = isWalkable;
            this.gridX = gridX;
            this.gridZ = gridZ;
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / tileSpacing);
        int z = Mathf.FloorToInt(position.z / tileSpacing);

        if (x >= 0 && x < gridSize && z >= 0 && z < gridSize)
        {
            return gridNodes[x, z];
        }
        return null;
    }

    public void SetObstacle(Vector3 position)
    {
        var node = GetNodeFromWorldPosition(position);
        if (node != null)
        {
            node.isWalkable = false;
        }
    }

    public void SetWalkable(Vector3 position)
    {
        var node = GetNodeFromWorldPosition(position);
        if (node != null)
        {
            node.isWalkable = true;
        }
    }
}
