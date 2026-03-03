using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private int _scoreThreshold = 50;
    [SerializeField] private string _targetSceneName = "Level2";

    private void Start()
    {
        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged += CheckScoreThreshold;
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged -= CheckScoreThreshold;
    }

    private void CheckScoreThreshold(int score)
    {
        if (score < _scoreThreshold) return;
        ScoreManager.Instance.OnScoreChanged -= CheckScoreThreshold;
        SceneManager.LoadScene(_targetSceneName);
    }
}
