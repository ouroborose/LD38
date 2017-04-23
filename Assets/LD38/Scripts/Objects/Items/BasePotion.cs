using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePotion : BaseItem {
    public float m_healPercent = 0.2f;

    public override void Collect(BasePlayer player)
    {
        player.Heal(CalculateHealAmount());

        base.Collect(player);
    }

    public int CalculateHealAmount()
    {
        return Mathf.Max(1, Mathf.RoundToInt(Main.Instance.Player.CalculateMaxHP() * m_healPercent));
    }

    public override string CreateInfoText()
    {
        return base.CreateInfoText() + string.Format("\n+{0} Hp", CalculateHealAmount());
    }
}
