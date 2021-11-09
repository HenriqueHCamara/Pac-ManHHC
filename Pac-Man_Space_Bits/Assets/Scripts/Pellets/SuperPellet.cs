using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPellet : Pellet
{
    public static event Action onSuperPelletCollected;
    public override IEnumerator CollectSequence()
    {
        spriteRenderer.enabled = false;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        this.gameObject.SetActive(false);
        onSuperPelletCollected?.Invoke();
    }
}
