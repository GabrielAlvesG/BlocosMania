using TMPro;
using UnityEngine;

public class RankingItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text posText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Color CorPrimeiro = Color.yellow;

    public void Setup(int position, string playerName, int score, float timeSpent, bool isPlaceholder = false)
    {
        if (posText != null)
        {
            posText.text = $"{position}.";
            if (position == 1) posText.color = CorPrimeiro;
        }

        if (isPlaceholder)
        {
            // Formatação para vagas sem jogador registrado
            if (nameText != null) nameText.text = "-----";
            if (scoreText != null) scoreText.text = "-----";
            if (timeText != null) timeText.text = "--:--";
        }
        else
        {
            // Dados reais de um jogador
            if (nameText != null) nameText.text = playerName;
            if (scoreText != null) scoreText.text = score.ToString();
            if (timeText != null) timeText.text = FormatTime(timeSpent);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
