using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData {
    public WorldSideData[] m_worldSideDatas = new WorldSideData[6];

    public int m_topSideIndex;

    public float m_rotationX;
    public float m_rotationY;
    public float m_rotationZ;
}
