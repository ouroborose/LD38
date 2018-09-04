using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour {

    [SerializeField] protected Slider m_musicSlider;
    [SerializeField] protected Slider m_sfxSlider;

    public void OnEnable()
    {
        m_musicSlider.value = AudioManager.Instance.m_musicVolume;
        m_sfxSlider.value = AudioManager.Instance.m_sfxVolume;
    }

    public void OnMusicVolumeChange(float value)
    {
        AudioManager.Instance.m_musicVolume = value;
    }

    public void OnSFXVolumeChange(float value)
    {
        AudioManager.Instance.m_sfxVolume = value;
    }
}
