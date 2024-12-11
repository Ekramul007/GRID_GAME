using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed at which the player moves

    [Header("References")]
    public GridManager gridManager; // Reference to the GridManager
    public ObstacleData obstacleData; // Reference to obstacle data

    private bool isMoving = false; // Flag to prevent input during movement
    private Vector3 targetPosition; // Destination position
    private Vector3 currentTilePosition; // Player's current tile position
    private List<GridManager.Node> path; // Path to follow

    private TurnManager turnManager; // Reference to TurnManager to know whose turn it is

    void Start()
    {
        // Initialize positions
        targetPosition = transform.position;
        currentTilePosition = transform.position;
        turnManager = FindObjectOfType<TurnManager>(); // Get the TurnManager reference
    }

    void Update()
    {
        if (turnManager.IsPlayerTurn()) // Only allow player movement during the player's turn
        {
            if (isMoving) return;

            // Handle mouse click input for movement
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }
    }

    // Called at the start of the player's turn to prepare for movement
    public void StartTurn()
    {
        isMoving = false; // Reset movement flag, in case the player was moving during the last turn
        // Any other setup for the player when the turn starts can go here
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedTile = hit.collider.gameObject;
            TileInfo tileInfo = clickedTile.GetComponent<TileInfo>();

            if (tileInfo != null)
            {
                int x = Mathf.RoundToInt(clickedTile.transform.position.x / gridManager.tileSpacing);
                int z = Mathf.RoundToInt(clickedTile.transform.position.z / gridManager.tileSpacing);

                var startNode = GetNodeFromPosition(currentTilePosition);
                var targetNode = gridManager.gridNodes[x, z];

                path = FindPath(startNode, targetNode);

                if (path != null && path.Count > 0)
                {
                    StartCoroutine(FollowPath());
                }
            }
        }
    }

    private GridManager.Node GetNodeFromPosition(Vector3 position)
    {
        return gridManager.gridNodes[
            Mathf.RoundToInt(position.x / gridManager.tileSpacing),
            Mathf.RoundToInt(position.z / gridManager.tileSpacing)
        ];
    }

    private List<GridManager.Node> FindPath(GridManager.Node startNode, GridManager.Node targetNode)
    {
        var openSet = new List<GridManager.Node> { startNode };
        var closedSet = new HashSet<GridManager.Node>();

        while (openSet.Count > 0)
        {
            var currentNode = GetLowestFCostNode(openSet);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // No path found
    }

    private GridManager.Node GetLowestFCostNode(List<GridManager.Node> nodes)
    {
        GridManager.Node lowestFCostNode = nodes[0];
        foreach (var node in nodes)
        {
            if (node.fCost < lowestFCostNode.fCost ||
                (node.fCost == lowestFCostNode.fCost && node.hCost < lowestFCostNode.hCost))
            {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }

    private List<GridManager.Node> RetracePath(GridManager.Node startNode, GridManager.Node endNode)
    {
        var path = new List<GridManager.Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private List<GridManager.Node> GetNeighbors(GridManager.Node node)
    {
        var neighbors = new List<GridManager.Node>();

        for (int x = -1; x <= 1; x += 2)
        {
            AddNeighborIfValid(node.gridX + x, node.gridZ, neighbors);
        }
        for (int z = -1; z <= 1; z += 2)
        {
            AddNeighborIfValid(node.gridX, node.gridZ + z, neighbors);
        }

        return neighbors;
    }

    private void AddNeighborIfValid(int x, int z, List<GridManager.Node> neighbors)
    {
        if (x >= 0 && x < gridManager.gridSize && z >= 0 && z < gridManager.gridSize)
        {
            // Check if the tile contains an obstacle or an enemy
            if (!obstacleData.blockedTiles[x * gridManager.gridSize + z] &&
                !IsEnemyOnTile(gridManager.gridNodes[x, z].worldPosition))
            {
                neighbors.Add(gridManager.gridNodes[x, z]);
            }
        }
    }

    private bool IsEnemyOnTile(Vector3 position)
    {
        // Cast a small overlap sphere to detect objects with the "Enemy" tag
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    private int GetDistance(GridManager.Node a, GridManager.Node b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridZ - b.gridZ);
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        foreach (var node in path)
        {
            targetPosition = node.worldPosition + Vector3.up * 0.5f;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentTilePosition = targetPosition;
        }

        isMoving = false;
        turnManager.EndPlayerTurn(); // End the player's turn after movement is done
    }
}
