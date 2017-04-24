using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    [SerializeField] private AudioClip m_confirm;

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
        GameObject obj = new GameObject(clip.name);
        obj.transform.parent = transform;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(obj, clip.length);
    }

    public void PlayConfirm()
    {
        PlayOneShot(m_confirm, 0.5f);
    }
}
