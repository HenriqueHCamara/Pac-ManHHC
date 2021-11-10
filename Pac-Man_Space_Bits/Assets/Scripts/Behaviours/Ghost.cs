using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour, IGhost
{
    [SerializeField] public GhostsSet ghostsSet;
    [SerializeField] public Movement movement;

    [SerializeField] public AnimationClip LookUp;
    [SerializeField] public AnimationClip LookDown;
    [SerializeField] public AnimationClip LookRight;
    [SerializeField] public AnimationClip LookLeft;
    [SerializeField] public AnimationClip Scared;

    [SerializeField] public Animator animator;
    [SerializeField] public RuntimeAnimatorController NormalController;
    [SerializeField] public RuntimeAnimatorController ScaredController;

    [SerializeField] public GameObject ghostNodeStart;
    [SerializeField] public GameObject ghostNodeCenter;
    [SerializeField] public GameObject ghostNodeLeft;
    [SerializeField] public GameObject ghostNodeRight;

    [SerializeField] public GameObject StartingNode;
    [SerializeField] public bool CanLeaveHome = false;

    public enum GhostNodeStateMachineEnum { Respawning, LeftNode, RightNode, CenterNode, StartNode, MovingBetweenNodes }
    public GhostNodeStateMachineEnum ghostNodeState;

    bool _isScared;

    // Start is called before the first frame update
    void Awake()
    {
        if (!movement)
        {
            movement = GetComponent<Movement>();
        }
    }

    private void Start()
    {
        SuperPellet.onSuperPelletCollected += PlayerCollectedSuperPellet;

        movement.IsGhost = true;
    }

    public void PlayerCollectedSuperPellet() 
    {
        _isScared = true;
        animator.runtimeAnimatorController = ScaredController;
        StartCoroutine(ScaredSequence());
    }

    IEnumerator ScaredSequence() 
    {
        yield return new WaitForSeconds(10f);
        animator.runtimeAnimatorController = NormalController;
        _isScared = false;
    }

    public void ReachedCenterOfNode(Node node)
    {
        if (ghostNodeState == GhostNodeStateMachineEnum.MovingBetweenNodes)
        {

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
                else if(ghostNodeState == GhostNodeStateMachineEnum.CenterNode)
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

    void DetermineDirection()
    {

    }

    void GetClosestDirectionToTarget(Vector2 ghostTarget)
    {
        float shortestDistanceToTarget = 0;
        Vector2 lastMovingDirection = movement._lastMovement;
        Vector2 newMovingDirection = Vector2.zero;

        Node node = movement._currentNode;

        if (node.canMoveUp && !lastMovingDirection.Equals(Vector2.down))
        {
            GameObject nodeUp = node.NodeUp;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeUp.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.up;
            }
        }

        if (node.canMoveDown && !lastMovingDirection.Equals(Vector2.up))
        {
            GameObject nodeDown = node.NodeUp;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeDown.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.down;
            }
        }

        if (node.canMoveRight && !lastMovingDirection.Equals(Vector2.left))
        {
            GameObject nodeRight = node.NodeUp;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeRight.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.right;
            }
        }

        if (node.canMoveLeft && !lastMovingDirection.Equals(Vector2.right))
        {
            GameObject nodeLeft = node.NodeUp;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeLeft.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.left;
            }
        }
    }

    public void ResetGhost()
    {

    }

    private void OnEnable()
    {
        AddToSet();
    }

    private void AddToSet()
    {
        if (ghostsSet != null && !ghostsSet.Items.Contains(this))
        {
            ghostsSet.Add(this);
        }
    }

    private void OnDisable()
    {
        if (ghostsSet != null)
        {
            ghostsSet.Remove(this);
        }
    }

    public void PlayerTouched()
    {
        throw new System.NotImplementedException();
    }
}
