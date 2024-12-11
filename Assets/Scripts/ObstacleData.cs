using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Game/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    public bool[] blockedTiles = new bool[100]; // 10x10 grid flattened into a 1D array
}
