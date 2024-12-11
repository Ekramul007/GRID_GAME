using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerControl player; // Reference to the player script
    public EnemyAI enemy; // Reference to the enemy script

    private bool isPlayerTurn = true; 

    void Start()
    {
        
        SetPlayerTurn(); 
    }

    
    public void EndPlayerTurn()
    {
        
        isPlayerTurn = false;
        SetEnemyTurn();
    }

    
    public void EndEnemyTurn()
    {
        
        isPlayerTurn = true;
        SetPlayerTurn();
    }

    private void SetPlayerTurn()
    {
        
        player.enabled = true; 
        enemy.enabled = false; 
        player.StartTurn(); 
    }

    private void SetEnemyTurn()
    {
        
        player.enabled = false; 
        enemy.enabled = true; 
        enemy.StartTurn(); 
    }

    
    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    
    public bool IsEnemyTurn()
    {
        return !isPlayerTurn;
    }
}
