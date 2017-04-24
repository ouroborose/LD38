using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopText : MonoBehaviour {
    [SerializeField] private TextMeshPro m_text;
    [SerializeField] private Rigidbody m_rigidbody;

    public void Show(string text, Color color, Vector3 worldPosStart, Vector3 dir, float lifeTime = 1.0f, bool applyGravity = true)
    {
        m_text.text = text;
        m_text.color = color;
        transform.position = worldPosStart;

        gameObject.SetActive(true);

        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.useGravity = applyGravity;
        m_rigidbody.AddForce(dir, ForceMode.Impulse);

        DOTween.To(() => m_text.alpha, (x) => m_text.alpha = x, 0.0f, lifeTime).OnComplete(Hide);
    }

    protected void Hide()
    {
        gameObject.SetActive(false);
    }
}
