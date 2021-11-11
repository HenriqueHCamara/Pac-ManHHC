using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer _masterMixer;

    public void SetVolume(float value) 
    {
        _masterMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
}
