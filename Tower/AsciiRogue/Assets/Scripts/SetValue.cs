using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetValue : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("musicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {

        GetComponent<Slider>().value = PlayerPrefs.GetFloat("musicVolume");
    }
}
