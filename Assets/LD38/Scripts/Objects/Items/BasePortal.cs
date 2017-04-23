using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePortal : BaseItem {
    public override void Collect(BasePlayer player)
    {
        Main.Instance.AdvanceToNextStage();
        base.Collect(player);
    }
}
