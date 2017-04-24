using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopTextManager : Singleton<PopTextManager> {

    public GameObject m_popTextPrefab;
    private List<PopText> m_pool = new List<PopText>();

    protected PopText GetAvailablePopText()
    {
        PopText popText = null;
        for(int i =0; i < m_pool.Count; ++i)
        {
            if(!m_pool[i].gameObject.activeSelf)
            {
                popText = m_pool[i];
                break;
            }
        }

        if(popText == null)
        {
            GameObject popTextObj = Instantiate(m_popTextPrefab);
            popTextObj.transform.SetParent(transform);
            popText = popTextObj.GetComponent<PopText>();
            m_pool.Add(popText);
        }

        popText.transform.rotation = Camera.main.transform.rotation;
        return popText;
    }

    public PopText Show(string text, Vector3 worldPosStart, Vector3 dir, Color color, float lifeTime = 1.5f, bool applyGravity = true)
    {
        PopText popText = GetAvailablePopText();
        popText.Show(text, color, worldPosStart, dir, lifeTime, applyGravity);
        return popText;
    }
}
