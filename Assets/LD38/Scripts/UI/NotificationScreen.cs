using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationScreen : BaseScreen {
    public TextMeshProUGUI m_dialog;
    public TextMeshProUGUI m_optionALabel;
    public TextMeshProUGUI m_optionBLabel;

    public System.Action m_onOptionA;
    public System.Action m_onOptionB;

    public void ShowNotification(string dialog, string optionALabel, System.Action onOptionA, string optionBLabel, System.Action onOptionB)
    {
        m_dialog.text = dialog;

        m_optionALabel.text = optionALabel;
        m_onOptionA = onOptionA;
        m_optionBLabel.text = optionBLabel;
        m_onOptionB = onOptionB;
        Show();
    }

    public void OnOptionASelected()
    {
        if(m_onOptionA != null)
        {
            m_onOptionA.Invoke();
        }
        Hide();
    }

    public void OnOptionBSelected()
    {
        if (m_onOptionB != null)
        {
            m_onOptionB.Invoke();
        }
        Hide();
    }
}
