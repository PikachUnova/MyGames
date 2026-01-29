using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;


    // Update is called once per frame
    public void PlayGame()
    {
        loadingScreen.SetActive(true);
        progressBar.value = 0;
        StartCoroutine(LoadAsynchronously(1));
    }

    IEnumerator LoadAsynchronously(int sceneIn)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIn);
        while (!operation.isDone)
        {
            progressBar.value = operation.progress;
            Destroy(this.gameObject);
            yield return null;
        }
        
    }

    public void HowToPlayGame()
    {
        SceneManager.LoadScene("HowToPlay");
    }
    public void OptionsGame()
    {
        SceneManager.LoadScene("Options");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
