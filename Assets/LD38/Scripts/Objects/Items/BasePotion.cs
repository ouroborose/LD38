using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePotion : BaseItem {

    public override void Collect(BasePlayer player)
    {
        player.Heal(player.CalculateMaxHP());

        base.Collect(player);
    }
}
