using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    public void PlayOneShot(AudioClip clip)
    {
        GameObject obj = new GameObject(clip.name);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(obj, clip.length);
    }
}
