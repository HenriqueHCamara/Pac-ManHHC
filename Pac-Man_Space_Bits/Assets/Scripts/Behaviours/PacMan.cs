using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    [SerializeField] LayerMask _whatIsPellet;
    [SerializeField] Node StartNode; 

    [SerializeField] Movement movement;

    public bool isPlayerInvincible;

    Animator Animator;
    SpriteRenderer spriteRenderer;
    public AudioSource audioSource;

    public Vector2 startPosition;

    private void Start()
    {
        PlayerInputHandler.onMovementInput += SetNextDirection;

        if (!Animator)
        {
            Animator = GetComponent<Animator>();
        }
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        movement._currentNode = StartNode;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (movement._currentMovement == Vector2.zero)
        {
            Animator.SetBool("IsMoving", false);
            audioSource.Stop();
        }
        else
        {
            Animator.SetBool("IsMoving", true);
            if (!audioSource.isPlaying)
                audioSource.Play();

            if (movement._currentMovement == Vector2.up)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (movement._currentMovement == Vector2.down)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            else if (movement._currentMovement == Vector2.right)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
    }

    void SetNextDirection(Vector2 dir) => movement.SetNextDirection(dir);

    public static event Action onPlayerDeath;

    public void DiePacMan() 
    {
        movement._currentMovement = Vector2.zero;
        movement._nextMovement = Vector2.zero;
        audioSource.Stop();
        movement.CanMove = false;
        movement._currentNode = null;
    }

    public void ResetPacMan() 
    {
        Animator.SetTrigger("Reset");
        audioSource.Stop();
        movement.CanMove = true;
        movement._currentNode = StartNode;
    }

    [ContextMenu("Die")]
    public void DeathTrigger() 
    {
        Animator.SetTrigger("Death");
        audioSource.Stop();
        onPlayerDeath?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pellet"))
        {
            collision.GetComponent<ICollectible>().Collected();
        }
        if (collision.CompareTag("Ghost"))
        {
            if (!isPlayerInvincible)
                DeathTrigger();
            else
                collision.GetComponent<IGhost>().PlayerTouched();
        }
    }
}
