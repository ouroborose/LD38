using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : BaseObject {
    public virtual void Collect(BasePlayer player)
    {
        Destroy(gameObject);
    }
}
