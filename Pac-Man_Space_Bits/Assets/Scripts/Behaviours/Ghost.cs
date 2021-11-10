using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour, IGhost
{
    public GhostsSet ghostsSet;
    [SerializeField] public Movement movement;
    [SerializeField] public PacMan pacman;

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
    [SerializeField] public GameObject ghostTargetNode;

    [SerializeField] public GameObject StartingNode;
    [SerializeField] public bool CanLeaveHome = false;
    [SerializeField] public bool IsChaseMode = false;
    public bool AlreadyEatenDuringInvincibility = false;

    public enum GhostNodeStateMachineEnum { Respawning, LeftNode, RightNode, CenterNode, StartNode, MovingBetweenNodes }
    public GhostNodeStateMachineEnum ghostNodeState;
    public GhostNodeStateMachineEnum ghostRespawnState;

    UnityEngine.Coroutine PelletTime;
    bool isPelletTimeRunning;

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
        GameManager.onSuperPelletStop += resetAlreadyEaten;

        movement.IsGhost = true;
    }

    public void PlayerCollectedSuperPellet()
    {
        AlreadyEatenDuringInvincibility = false;
        animator.runtimeAnimatorController = NormalController;
        animator.runtimeAnimatorController = ScaredController;
        if (!isPelletTimeRunning)
        {
            PelletTime = StartCoroutine(ScaredSequence());
        }
        else
        {
            StopCoroutine(PelletTime);
            PelletTime = StartCoroutine(ScaredSequence());
        }
    }

    IEnumerator ScaredSequence()
    {
        isPelletTimeRunning = true;
        
        yield return new WaitForSeconds(10f);
        animator.runtimeAnimatorController = NormalController;
        isPelletTimeRunning = false;
    }

    public virtual void ReachedCenterOfNode(Node node)
    {
    }

    public Vector2 GetClosestDirectionToTarget(Vector2 ghostTarget)
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
            GameObject nodeDown = node.NodeDown;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeDown.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.down;
            }
        }

        if (node.canMoveRight && !lastMovingDirection.Equals(Vector2.left))
        {
            GameObject nodeRight = node.NodeRight;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeRight.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.right;
            }
        }

        if (node.canMoveLeft && !lastMovingDirection.Equals(Vector2.right))
        {
            GameObject nodeLeft = node.NodeLeft;
            float distanceBetweenNodeAndTarget = Vector2.Distance(nodeLeft.transform.position, ghostTarget);

            if (distanceBetweenNodeAndTarget < shortestDistanceToTarget || shortestDistanceToTarget == 0)
            {
                shortestDistanceToTarget = distanceBetweenNodeAndTarget;
                newMovingDirection = Vector2.left;
            }
        }

        return newMovingDirection;
    }

    public void ResetGhost()
    {
        animator.runtimeAnimatorController = NormalController;
        movement.CanMove = true;
        transform.position = StartingNode.transform.position;
        movement._currentNode = StartingNode.GetComponent<Node>();
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
        SuperPellet.onSuperPelletCollected -= PlayerCollectedSuperPellet;
        GameManager.onSuperPelletStop -= resetAlreadyEaten;

    }

    public void resetAlreadyEaten() => AlreadyEatenDuringInvincibility = false;

    public static event Action<int> onGhostEaten;
    public void PlayerTouched()
    {
        onGhostEaten?.Invoke(200);
        AlreadyEatenDuringInvincibility = true;
        ghostNodeState = GhostNodeStateMachineEnum.Respawning;
        transform.position = ghostNodeCenter.transform.position;
        movement._currentNode = ghostNodeCenter.GetComponent<Node>();
        animator.runtimeAnimatorController = NormalController;
    }
}
