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

    private void UpdateDisplay(int score) => _scoreText.text = "SCORE : " + score;
}
