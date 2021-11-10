using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost, IGhost
{
    private void Awake()
    {
        ghostNodeState = GhostNodeStateMachineEnum.StartNode;

        movement._currentNode = StartingNode.GetComponent<Node>();
        transform.position = StartingNode.transform.position;
    }

    void DetermineDirection()
    {
        Vector2 direction = Vector2.zero;
        if (pacman.isPlayerInvincible && !AlreadyEatenDuringInvincibility)
        {
            direction = GetClosestDirectionToTarget(ghostTargetNode.transform.position);
        }
        else
        {
            direction = GetClosestDirectionToTarget(pacman.transform.position);
        }
        movement._lastMovement = direction;
        movement.SetNextDirection(direction);


        if (direction.Equals(Vector2.up))
            animator.Play(LookUp.name);

        else if (direction.Equals(Vector2.down))
            animator.Play(LookDown.name);

        else if (direction.Equals(Vector2.right))
            animator.Play(LookRight.name);

        else
            animator.Play(LookLeft.name);

    }

    public override void ReachedCenterOfNode(Node node)
    {
        if (ghostNodeState == GhostNodeStateMachineEnum.MovingBetweenNodes)
        {
            DetermineDirection();
        }
        else if (ghostNodeState == GhostNodeStateMachineEnum.Respawning)
        {
            ghostNodeState = ghostRespawnState;
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
