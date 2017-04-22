using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseActor
{
    public int m_numKeys { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        m_numKeys = 0;
    }

    public override void SetTile(BaseTile tile, bool rotateToTile = true, Vector3 localRotation = default(Vector3))
    {
        for(int i = 0; i < tile.m_objs.Count; ++i)
        {
            BaseObject obj = tile.m_objs[i];
            BaseItem item = obj as BaseItem;
            if (item != null)
            {
                // pick up item
            }

            BaseTrap trap = obj as BaseTrap;
            if (trap != null)
            {
                // hit trap
                trap.Activate(this);
            }
        }

        base.SetTile(tile, rotateToTile, localRotation);
    }
}
