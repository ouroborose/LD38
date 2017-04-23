using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    public Vector3 m_rotationSpeed = new Vector3(0, 90, 0);
    public Space m_rotationSpace = Space.Self;
    protected void Update()
    {
        transform.Rotate(m_rotationSpeed * Time.deltaTime, m_rotationSpace);
    }
}
