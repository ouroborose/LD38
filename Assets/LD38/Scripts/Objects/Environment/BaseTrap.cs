using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTrap : BaseObject {
    [SerializeField] private float m_damagePercent = 0.2f;

    public void Activate(BaseActor actor)
    {
        actor.TakeDamage(CalculateDamage(), this);
    }

    public int CalculateDamage()
    {
        return Mathf.Max(1, Mathf.RoundToInt(Main.Instance.Player.CalculateMaxHP() * m_damagePercent));
    }

    public override string CreateInfoText()
    {
        return base.CreateInfoText() + string.Format("\nDmg: {0}", CalculateDamage());
    }
}
