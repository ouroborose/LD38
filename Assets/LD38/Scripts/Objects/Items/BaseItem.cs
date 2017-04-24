using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : BaseObject {
    public virtual void Collect(BasePlayer player)
    {
        ShowPopText(string.Format("Get {0}", m_displayName), Color.white, Vector3.up * 4.5f, 1.0f, 5.0f);
        Destroy(gameObject);
    }
}
