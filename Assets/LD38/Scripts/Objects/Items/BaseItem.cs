using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : BaseObject {
    [SerializeField] private AudioClip[] m_collectSounds;

    public virtual void Collect(BasePlayer player)
    {
        AudioManager.Instance.PlayOneShot(m_collectSounds);
        ShowPopText(string.Format("Get {0}", m_displayName), Color.white, Vector3.up * 4.5f, 1.0f, 5.0f);
        Destroy(gameObject);
    }
}
