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
    private void OnDisable()
    {
        Ghost.onGhostEaten -= FreezeGame;
    }

    void FreezeGame(int value) 
    {
        StartCoroutine(freezeFrame());
    }

    IEnumerator freezeFrame() 
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 1f;
    }

}
