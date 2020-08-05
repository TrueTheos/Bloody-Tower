using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenAnimation : MonoBehaviour
{

    public Text text;

    private int scene = 1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Animate());
        StartCoroutine(LoadNewScene());
    }

    IEnumerator Animate()
    {
        while(true)
        {
            yield return new WaitForSeconds(.5f);
            text.text = @"Blood God is cleaning his tower...

              \o\";
            yield return new WaitForSeconds(.5f);
            text.text = @"Blood God is cleaning his tower...

              /o/";
        }
    }

    [System.Obsolete]
    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(3);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
}
