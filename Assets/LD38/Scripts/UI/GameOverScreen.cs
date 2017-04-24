using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : BaseScreen {
    public const string GAME_OVER_TEXT = "World\n{0}\n\nGame\nOver";

    [SerializeField] private TextMeshProUGUI m_gameOverText;

    public override void Show(bool instant = false)
    {
        m_gameOverText.text = string.Format(GAME_OVER_TEXT, Main.Instance.m_currentLevel);
        base.Show(instant);
    }
}
