using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPellet : Pellet
{
    public static event Action onSuperPelletCollected;
    public static event Action<int> onSuperPelletDone;
    public override IEnumerator CollectSequence()
    {
        onSuperPelletCollected?.Invoke();
        spriteRenderer.enabled = false;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        this.gameObject.SetActive(false);
        onSuperPelletDone?.Invoke(50);
    }
}
