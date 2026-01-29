using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    private AudioSource audiosource;

    public AudioClip missClip; // A clip when Rex loses all his health

    private bool played = false;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayTrack(0);
            audiosource.volume = 0.8f;
        }
    }

    // Update is called once per frame
    void PlayTrack(int index)
    {
        if (index >= 0 && index < musicTracks.Length)
        {
            audiosource.clip = musicTracks[index];
            audiosource.Play();
        }
    }

    public void StartPlay() // Start scene where rex transitions or respawns
    {
        // Play current track
        PlayTrack(SceneManager.GetActiveScene().buildIndex - 1);
        played = false;
    }

    void Update()
    {
        // Play miss SFX if the player ran out of HP
        if (GetComponent<PlayerHealth>().isDead && played == false)
        {
            played = true;
            audiosource.clip = missClip;
            audiosource.Play();
            return;
        }
    }
}
