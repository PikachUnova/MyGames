using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetSFX(float volume)
    {
        Debug.Log("Change Volume");
    }
    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        Debug.Log("Change Music Volume");
    }
}
