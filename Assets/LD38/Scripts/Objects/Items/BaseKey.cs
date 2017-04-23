using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseKey : BaseItem {
    public override void Collect(BasePlayer player)
    {
        player.AddKey();
        base.Collect(player);
    }
}
