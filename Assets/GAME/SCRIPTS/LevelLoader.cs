using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [Header("Win Condition")]
    [SerializeField] private int _scoreThreshold = 50;
    [SerializeField] private string _winSceneName = "Win";

    [Header("Timer")]
    [SerializeField] private float _timeLimit = 60f;
    [SerializeField] private string _loseSceneName = "Lose";

    private float _timeRemaining;
    private bool _levelEnded = false;

    private void Start()
    {
        _timeRemaining = _timeLimit;

        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged += CheckScoreThreshold;
    }

    private void Update()
    {
        if (_levelEnded) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0f)
            EndLevel(false);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged -= CheckScoreThreshold;
    }

    private void CheckScoreThreshold(int score)
    {
        if (score < _scoreThreshold) return;
        EndLevel(true);
    }
    private void EndLevel(bool won)
    {
        if (_levelEnded) return;
        _levelEnded = true;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= CheckScoreThreshold;

        SceneManager.LoadScene(won ? _winSceneName : _loseSceneName);
    }
}
