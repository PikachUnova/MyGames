using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;

    private AudioSource audioSource;
    public AudioClip warp;
    
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //Navigates the Player from one scene to another
    void OnTriggerEnter(Collider other)
    {   
        // The contacted object navigates the player 
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(warp);
            loadingScreen.SetActive(true);
            progressBar.value = 0;
            StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    IEnumerator LoadAsynchronously(int sceneIn)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIn);
        while (!operation.isDone)
        {
            progressBar.value = operation.progress;
            yield return null;
        }
        loadingScreen.SetActive(false);
    }
}
