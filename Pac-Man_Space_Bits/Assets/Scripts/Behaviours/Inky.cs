using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost, IGhost
{
    Vector2 newTarget;

    public GameObject Blinky;

    private void Awake()
    {
        ghostNodeState = GhostNodeStateMachineEnum.CenterNode;


        movement._currentNode = StartingNode.GetComponent<Node>();
        transform.position = StartingNode.transform.position;

        IsChaseMode = false;
    }

    void DetermineDirection()
    {
        Vector2 direction = Vector2.zero;
        movement._speedMultiplier = 1f;

        if (pacman.isPlayerInvincible && !AlreadyEatenDuringInvincibility)
        {
            movement._speedMultiplier = 0.7f;
            direction = GetClosestDirectionToTarget(ghostTargetNode.transform.position);
        }
        else if (IsChaseMode)
        {
            Vector2 pacmanDirection = pacman.GetComponent<Movement>()._currentMovement;
            float NodeDistance = 1f;

            Vector2 target = pacman.transform.position;

            if (pacmanDirection == Vector2.right)
                target.x += NodeDistance * 2;

            else if (pacmanDirection == Vector2.left)
                target.x -= NodeDistance * 2;

            else if (pacmanDirection == Vector2.up)
                target.y += NodeDistance * 2;

            else if (pacmanDirection == Vector2.down)
                target.y -= NodeDistance * 2;


            float xDistance = target.x - Blinky.transform.position.x;
            float yDistance = target.y - Blinky.transform.position.y;

            newTarget = new Vector2(target.x + xDistance, target.y + yDistance);

            direction = GetClosestDirectionToTarget(newTarget);
        }
        else
        {
            direction = GetClosestDirectionToTarget(ghostTargetNode.transform.position);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(newTarget, .3f);
    }
}
