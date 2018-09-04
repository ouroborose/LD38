using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    public const string MUSIC_VOLUME_KEY = "__MUSIC_VOLUME__";
    public const string SFX_VOLUME_KEY = "__SFX_VOLUME__";

    [SerializeField] private AudioClip m_confirm;

    public AudioSource m_bgMusic;
    
    [Range(0.0f, 1.0f)]
    public float m_sfxVolume = 1.0f;

    public float m_musicVolume
    {
        get { return m_bgMusic.volume; }
        set { m_bgMusic.volume = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        m_musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, m_musicVolume);
        m_sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, m_sfxVolume);
    }

    protected override void OnDestroy()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, m_musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, m_sfxVolume);

        base.OnDestroy();
    }


    public void PlayOneShot(AudioClip[] clips, float volume = 1.0f)
    {
        if(clips == null || clips.Length <= 0)
        {
            return;
        }

        PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }

    public void PlayOneShot(AudioClip clip, float volume = 1.0f)
    {
        if(m_sfxVolume <= 0.0f)
        {
            return;
        }

        GameObject obj = new GameObject(clip.name);
        obj.transform.parent = transform;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * m_sfxVolume;
        source.Play();
        Destroy(obj, clip.length);
    }

    public void PlayConfirm()
    {
        PlayOneShot(m_confirm, 0.5f);
    }
}
