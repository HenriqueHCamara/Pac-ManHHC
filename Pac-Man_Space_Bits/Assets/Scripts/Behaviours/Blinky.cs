using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    private void Awake()
    {
        ghostNodeState = GhostNodeStateMachineEnum.StartNode;
        StartingNode = ghostNodeStart;

        movement._currentNode = StartingNode.GetComponent<Node>();
    }
}
