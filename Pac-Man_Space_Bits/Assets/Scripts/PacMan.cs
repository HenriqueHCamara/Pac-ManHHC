using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : Movement
{
    [SerializeField] LayerMask _whatIsPellet;

    public bool isPlayerInvincible;
    
    private void Start()
    {
        PlayerInputHandler.onMovementInput += SetNextDirection;
    }

    public static event Action onPlayerDeath;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pellet"))
        {
            collision.GetComponent<ICollectible>().Collected();
        }
        if (collision.CompareTag("Ghost"))
        {
            if (!isPlayerInvincible)
                onPlayerDeath?.Invoke();
            else
                collision.GetComponent<IGhost>().PlayerTouched();
        }
    }
}
