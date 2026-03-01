using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void OnEnable() => ScoreManager.Instance.OnScoreChanged += UpdateDisplay;
    private void OnDisable() => ScoreManager.Instance.OnScoreChanged -= UpdateDisplay;
    private void Start() => UpdateDisplay(ScoreManager.Instance.Score);


    private void UpdateDisplay(int score) => _scoreText.text = score.ToString();
}
