using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenAnimation : MonoBehaviour
{
    public Text text;

    void Start()
    {
        StartCoroutine(LoadNewScene());
    }

    [System.Obsolete]
    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
}
