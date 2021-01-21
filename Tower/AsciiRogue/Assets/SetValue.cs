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
    }

    private void OnEnable()
    {
        float val;
        mixer.GetFloat("MusicVol", out val);
        GetComponent<Slider>().value = val;
    }
}
