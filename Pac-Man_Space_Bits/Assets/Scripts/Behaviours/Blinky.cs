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
        transform.position = StartingNode.transform.position;
    }

    void DetermineDirection() 
    {
        Vector2 direction = GetClosestDirectionToTarget(pacman.transform.position);
        movement._lastMovement = direction;
        movement.SetNextDirection(direction);
    }

    public override void ReachedCenterOfNode(Node node)
    {
        if (ghostNodeState == GhostNodeStateMachineEnum.MovingBetweenNodes)
        {
            DetermineDirection();
        }
        else if (ghostNodeState == GhostNodeStateMachineEnum.Respawning)
        {

        }
        else
        {
            if (CanLeaveHome)
            {
                if (ghostNodeState == GhostNodeStateMachineEnum.LeftNode)
                {
                    ghostNodeState = GhostNodeStateMachineEnum.CenterNode;
                    movement.SetNextDirection(Vector2.right);
                }
                else if (ghostNodeState == GhostNodeStateMachineEnum.RightNode)
                {
                    ghostNodeState = GhostNodeStateMachineEnum.CenterNode;
                    movement.SetNextDirection(Vector2.left);

                }
                else if (ghostNodeState == GhostNodeStateMachineEnum.CenterNode)
                {
                    ghostNodeState = GhostNodeStateMachineEnum.StartNode;
                    movement.SetNextDirection(Vector2.up);

                }
                else if (ghostNodeState == GhostNodeStateMachineEnum.StartNode)
                {
                    ghostNodeState = GhostNodeStateMachineEnum.MovingBetweenNodes;
                    movement.SetNextDirection(Vector2.right);
                }
            }
        }
    }
}
