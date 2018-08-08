using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePortal : BaseItem {

    public override void Collect(BasePlayer player)
    {
        ShowPositivePopText(string.Format("World {0} Complete!", Main.Instance.m_currentLevel));
        Main.Instance.AdvanceToNextStage();
        base.Collect(player);
    }
}
