using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public string GetTileInfo()
    {
        int xIndex = Mathf.RoundToInt(transform.position.x / 2f); // Adjust based on spacing
        int zIndex = Mathf.RoundToInt(transform.position.z / 2f); // Adjust based on spacing
        return $"Tile Coordinates: ({xIndex}, {zIndex})";
    }
}
