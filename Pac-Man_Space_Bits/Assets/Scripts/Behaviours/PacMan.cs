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
    AudioSource audioSource;

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

    [ContextMenu("Die")]
    public void DeathTrigger() 
    {
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
                onPlayerDeath?.Invoke();
            else
                collision.GetComponent<IGhost>().PlayerTouched();
        }
    }
}
