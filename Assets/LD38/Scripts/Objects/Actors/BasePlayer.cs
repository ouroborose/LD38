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
        BaseItem item = tile.m_obj as BaseItem;
        if(item != null)
        {
            // pick up item
        }

        BaseTrap trap = tile.m_obj as BaseTrap;
        if(trap != null)
        {
            // hit trap

        }

        base.SetTile(tile, rotateToTile, localRotation);
    }
}
