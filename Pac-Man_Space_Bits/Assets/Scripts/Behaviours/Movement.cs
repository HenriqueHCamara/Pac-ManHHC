using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _speedMultiplier = 1f;

    Vector2 _currentMovement;
    Vector2 _nextMovement;

    [SerializeField] LayerMask _isOccupiedByWhat;

    Rigidbody2D rigidbody2D;
    Animator animator;
    BoxCollider2D boxCollider2D;

    private void Awake()
    {
        if (!rigidbody2D)
            rigidbody2D = GetComponent<Rigidbody2D>();
        if (!animator)
            animator = GetComponent<Animator>();
        if (!boxCollider2D)
            boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector2 position = rigidbody2D.position;
        Vector2 translation = _currentMovement * _speed * _speedMultiplier * Time.deltaTime;

        rigidbody2D.MovePosition(position + translation);
    }

    public bool IsOccupied(Vector2 move)
    {
        Vector3 ls = transform.localScale / 6;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position,
                                         ls,
                                         0f,
                                         move,
                                         1f,
                                         _isOccupiedByWhat);

        return hit.collider != null;
    }

    public virtual void SetNextDirection(Vector2 direction)
    {
        if (IsOccupied(direction))
        {
            _currentMovement = direction;
            _nextMovement = Vector2.zero;
        }
        else
            _nextMovement = direction;
        
    }
}
