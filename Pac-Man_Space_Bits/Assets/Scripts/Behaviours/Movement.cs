using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] public float _speedMultiplier = 1f;

    public Vector2 _currentMovement;
    public Vector2 _nextMovement;
    public Vector2 _lastMovement;

    Animator animator;
    BoxCollider2D boxCollider2D;

    public bool CanMove = true;
    public bool IsGhost = false;

    public Node _currentNode;
    Node _lastNode;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        if (!boxCollider2D)
            boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_currentNode && CanMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, _currentNode.transform.position, (_speedMultiplier * _speed) * Time.deltaTime);
            if (IsOnNode())
                MoveToNextNode();
        }
    }

    private void MoveToNextNode()
    {
        Node newNode = _currentNode.GetAvaiableNodeFromDirection(_nextMovement);

        if (newNode)
        {
            _lastNode = _currentNode;
            _currentNode = newNode;
            _currentMovement = _nextMovement;
        }
        else
        {
            newNode = _currentNode.GetAvaiableNodeFromDirection(_currentMovement);
            if (newNode)
            {
                _currentNode = newNode;
            }
            else
            {
                _currentMovement = Vector2.zero;
            }

        }
    }


    public bool IsOnNode()
    {
        if (transform.position == _currentNode.gameObject.transform.position)
        {
            if (IsGhost)
            {
                if (this.GetComponent<Blinky>())
                {
                    GetComponent<Blinky>().ReachedCenterOfNode(_currentNode);
                }
                else if (GetComponent<Pinky>())
                {
                    GetComponent<Pinky>().ReachedCenterOfNode(_currentNode);

                }
                else if (GetComponent<Clyde>())
                {

                    GetComponent<Clyde>().ReachedCenterOfNode(_currentNode);
                }
                else
                {

                    GetComponent<Inky>().ReachedCenterOfNode(_currentNode);
                }
                //GetComponent<Ghost>().ReachedCenterOfNode(_currentNode);
            }
            return true;
        }
        else
            return false;
    }

    public virtual void SetNextDirection(Vector2 direction)
    {
        _nextMovement = direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 pos = transform.position;
        Gizmos.DrawSphere(_currentMovement + pos, .25f);
    }
}
