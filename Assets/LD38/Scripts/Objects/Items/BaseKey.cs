using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseKey : BaseItem {
    public static readonly List<BaseKey> s_allKeys = new List<BaseKey>();

    protected override void Awake()
    {
        base.Awake();
        s_allKeys.Add(this);
    }
    
    protected override void OnDestroy()
    {
        s_allKeys.Remove(this);
        base.OnDestroy();
    }

    public override void Collect(BasePlayer player)
    {
        player.AddKey();
        base.Collect(player);
    }
}
