using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject leaderboardPanel;

    [Header("Main Panel")]
    [SerializeField] TMP_InputField mainPanel_nameInput;

    [Header("HUD")]
    [SerializeField] TextMeshProUGUI hud_scoreText;
    [SerializeField] TextMeshProUGUI hud_distanceText;

    [SerializeField] string scorePrefix = "Score: ";
    [SerializeField] string bonusScorePrefix = "Bonus Score: ";
    [SerializeField] string distancePrefix = "Distance Travelled: ";
    [SerializeField] string distanceSuffix = "m";
    [SerializeField] string durationSuffix = "Game Duration: ";

    [Header("Game Over Panel")]
    [SerializeField] TextMeshProUGUI gameOver_valuesText;

    [Header("Leaderboard Panel")]
    [SerializeField] Transform leaderboard_buttonsParent;
    [SerializeField] TextMeshProUGUI leaderboard_names;
    [SerializeField] TextMeshProUGUI leaderboard_scores;

    public void UpdateScore(int score)
    {
        hud_scoreText.text = scorePrefix + score.ToString();
    }

    public void UpdateDistance(float distance)
    {
        hud_distanceText.text = string.Format("{0:F2}", distance) + distanceSuffix;
    }

    public void ShowMainPanel(bool show)
    {
        mainPanel.SetActive(show);
    }

    public void ShowHUD(bool show)
    {
        hud.SetActive(show);
    }

    public void ShowGameOverPanel(bool show)
    {
        gameOverPanel.SetActive(show);
    }

    public void ShowLeaderboardPanel(bool show)
    {
        leaderboardPanel.SetActive(show);
    }

    public string GetNameInputText()
    {
        return mainPanel_nameInput.text.Trim();
    }

    public void SetValuesText(string text)
    {
        gameOver_valuesText.text = text;
    }

    public void SetLeaderboardValues(List<string[]> leaderboard)
    {
        string namesText = "";
        string scoresText = "";
        foreach (string[] entry in leaderboard)
        {
            namesText += string.IsNullOrEmpty(entry[1]) || string.IsNullOrWhiteSpace(entry[1]) ? "Anonymous\n" : entry[1] + "\n";
            scoresText += entry[2] + "\n";
        }
        leaderboard_names.text = namesText;
        leaderboard_scores.text = scoresText;
    }

    public void ShowLeaderboardInfo(int index, string[] info)
    {
        GameObject button = leaderboard_buttonsParent.GetChild(index).gameObject;
        string infoText = bonusScorePrefix + info[0] + "\n"
            + distancePrefix + string.Format("{0:F2}", float.Parse(info[1])) + distanceSuffix + "\n"
            + durationSuffix + TimeSpan.FromSeconds(float.Parse(info[2])).ToString("mm':'ss");
        button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = infoText;
        button.transform.GetChild(0).gameObject.SetActive(true);
    }
}
