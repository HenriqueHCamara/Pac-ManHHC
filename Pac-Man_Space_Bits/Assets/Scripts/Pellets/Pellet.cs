using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour, ICollectible
{
    [SerializeField] public PelletsSet pelletsSet;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public SpriteRenderer spriteRenderer;

    public static event Action onPelletCollected;
    [ContextMenu("Collect")]
    public void Collected()
    {
        StartCoroutine(CollectSequence());
    }

    public virtual IEnumerator CollectSequence() 
    {
        spriteRenderer.enabled = false;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        this.gameObject.SetActive(false);
        onPelletCollected?.Invoke();
    }

    private void OnEnable()
    {
        AddToSet();
    }

    private void AddToSet()
    {
        if (pelletsSet != null && !pelletsSet.Items.Contains(this))
        {
            pelletsSet.Add(this);
        }
    }

    private void OnDisable()
    {
        if (pelletsSet != null)
        {
            pelletsSet.Remove(this);
        }
    }
}
