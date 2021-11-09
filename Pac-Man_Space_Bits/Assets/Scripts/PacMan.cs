using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : Movement
{
    [SerializeField] LayerMask _whatIsPellet;
    [SerializeField] LayerMask _whatIsGhost;

    private void Start()
    {
        PlayerInputHandler.onMovementInput += SetNextDirection;
    }

    void Move()
    {

    }
}
