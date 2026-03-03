using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged += UpdateDisplay;
        UpdateDisplay(ScoreManager.Instance.Score);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged -= UpdateDisplay;
    }

    /// <summary>Updates the score label with the current score value.</summary>
    private void UpdateDisplay(int score) => _scoreText.text = "SCORE : " + score;
}
