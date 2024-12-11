using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f; 

    [Header("References")]
    public GridManager gridManager; 
    public ObstacleData obstacleData; // Reference to obstacle data
    public Transform player; 
    private TurnManager turnManager; // Reference to TurnManager

    private bool isMoving = false; 
    private Vector3 targetPosition; // The next target position
    private Vector3 currentTilePosition; // The enemy's current tile position
    private List<GridManager.Node> path; 

    private Collider playerCollider; // Player collider reference

    void Start()
    {
        // Initialize position variables
        targetPosition = transform.position;
        currentTilePosition = transform.position;

        // Get reference to TurnManager
        turnManager = FindObjectOfType<TurnManager>();

        // Get the player's collider
        playerCollider = player.GetComponent<Collider>();
    }

    void Update()
    {
        // Check if it's the enemy's turn before executing AI logic
        if (!turnManager.IsEnemyTurn()) return;

        if (isMoving) return;

        var startNode = GetNodeFromPosition(currentTilePosition);
        var targetNode = GetNodeFromPosition(player.position);

        // Calculate path to the player
        path = FindPath(startNode, targetNode);

        if (path != null && path.Count > 0)
        {
            StartCoroutine(FollowPath());
        }
    }

    // Start the enemy's turn
    public void StartTurn()
    {
        // You can add any logic to prepare the enemy for their turn here
        Debug.Log("Enemy's Turn Started");
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

        // Check horizontal and vertical neighbors
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
            if (!obstacleData.blockedTiles[x * gridManager.gridSize + z])
            {
                neighbors.Add(gridManager.gridNodes[x, z]);
            }
        }
    }

    private int GetDistance(GridManager.Node a, GridManager.Node b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridZ - b.gridZ);
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        // Follow the path to the target position
        foreach (var node in path)
        {
            targetPosition = node.worldPosition + Vector3.up * 0.5f; // Adjust for enemy height

            // Check if the enemy is about to enter the player's collider
            if (IsPlayerNearby(targetPosition))
            {
                // Stop the enemy if it's about to collide with the player
                break;
            }

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentTilePosition = targetPosition;
        }

        isMoving = false;

        // End the enemy's turn once the movement is done
        turnManager.EndEnemyTurn();
    }

    // Check if the enemy is about to enter the player's collider zone
    private bool IsPlayerNearby(Vector3 targetPosition)
    {
        // Calculate the distance between the target position and the player's position
        Collider enemyCollider = GetComponent<Collider>();
        Vector3 directionToPlayer = player.position - targetPosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if the target position is inside the player's collider zone (a threshold distance)
        return distanceToPlayer < (playerCollider.bounds.size.x / 2 + enemyCollider.bounds.size.x / 2);
    }
}
