using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public GameObject settingsCanvas, mainCanvas;
    public AudioMixer mixer;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat("musicVolume"));
        mixer.SetFloat("MusicVol", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume")) * 20);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            NewGame();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GoToSettings();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            EnableCredits();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToMainMenu();
        }
    }

    public void EnableCredits()
    {
        StartCoroutine(GoToCredits());
    }

    public void GoBackToMainMenu()
    {
        StartCoroutine(GoToMainMenu());
    }

    public void QuitTheGame()
    {
        StartCoroutine(Quit());
    }

    public void NewGame()
    {
        StartCoroutine(Transition());
    }

    public Animator animator;
    IEnumerator Transition()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Scene 0", LoadSceneMode.Single);
    }

    IEnumerator Quit()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    IEnumerator GoToMainMenu()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    IEnumerator GoToCredits()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }

    public void GoToSettings()
    {
        StartCoroutine(GoToSet());
    }

    IEnumerator GoToSet()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        mainCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }
}
