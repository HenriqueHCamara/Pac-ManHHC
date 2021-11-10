using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFreezeEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Ghost.onGhostEaten += FreezeGame;
    }

    void FreezeGame() 
    {
        StartCoroutine(freezeFrame());
    }

    IEnumerator freezeFrame() 
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(.15f);
        Time.timeScale = 1f;
    }

}
