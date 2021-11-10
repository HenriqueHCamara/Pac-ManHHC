using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool canMoveUp;
    public bool canMoveDown; 
    public bool canMoveLeft;
    public bool canMoveRight;

    public GameObject NodeUp;
    public GameObject NodeDown;
    public GameObject NodeLeft;
    public GameObject NodeRight;

    public bool isWarpRightNode = false;
    public bool isWarpLeftNode = false;
    public bool isWarpingNode = false;

    [SerializeField] LayerMask _whatIsNode;

    private void Start()
    {
        Vector2 pos = transform.position;

        RaycastHit2D[] nodeHitDown;
        nodeHitDown = Physics2D.RaycastAll(pos, -Vector2.up, 1f,_whatIsNode);
        RaycastHit2D[] nodeHitUp;
        nodeHitUp = Physics2D.RaycastAll(pos, Vector2.up, 1f,_whatIsNode);
        RaycastHit2D[] nodeHitLeft;
        nodeHitLeft = Physics2D.RaycastAll(pos, -Vector2.right, 1f,_whatIsNode);
        RaycastHit2D[] nodeHitRight;
        nodeHitRight = Physics2D.RaycastAll(pos, Vector2.right, 1f, _whatIsNode);

        for (int i = 0; i < nodeHitDown.Length; i++)
        {
            float nodeHitDistance = Mathf.Abs(nodeHitDown[i].point.y - transform.position.y);
            if (nodeHitDistance < 1.1f)
            {
                canMoveDown = true;
                NodeDown = nodeHitDown[i].collider.gameObject;
            }
        }

        for (int i = 0; i < nodeHitUp.Length; i++)
        {
            float nodeHitDistance = Mathf.Abs(nodeHitUp[i].point.y - transform.position.y);
            if (nodeHitDistance < 1.1f)
            {
                canMoveUp = true;
                NodeUp = nodeHitUp[i].collider.gameObject;
            }
        }

        for (int i = 0; i < nodeHitLeft.Length; i++)
        {
            float nodeHitDistance = Mathf.Abs(nodeHitLeft[i].point.x - transform.position.x);
            if (nodeHitDistance < 1.1f)
            {
                canMoveLeft = true;
                NodeLeft = nodeHitLeft[i].collider.gameObject;
            }
        }

        for (int i = 0; i < nodeHitRight.Length; i++)
        {
            float nodeHitDistance = Mathf.Abs(nodeHitRight[i].point.x - transform.position.x);
            if (nodeHitDistance < 1.1f)
            {
                canMoveRight = true;
                NodeRight = nodeHitRight[i].collider.gameObject;
            }
        }
    }

    public Node GetAvaiableNodeFromDirection(Vector2 direction) 
    {
        if (direction.Equals(Vector2.left) && canMoveLeft)
        {
            return NodeLeft.GetComponent<Node>();
        }
        else if (direction.Equals(Vector2.right) && canMoveRight)
        {
            return NodeRight.GetComponent<Node>();
        }
        else if (direction.Equals(Vector2.down) && canMoveDown)
        {
            return NodeDown.GetComponent<Node>();
        }
        else if (direction.Equals(Vector2.up) && canMoveUp)
        {
            return NodeUp.GetComponent<Node>();
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .25f);
    }
}
