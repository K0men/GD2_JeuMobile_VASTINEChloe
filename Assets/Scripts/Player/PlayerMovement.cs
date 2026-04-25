using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform[] m_transforms;
    [SerializeField] private InputPlayerManagerCustom m_inputManager;
    [SerializeField] private AudioEventDispatcher m_audioEventDispatcher;

    private int m_currentIndex = 2;
    private const int MoveSpeed = 1;

    private void OnEnable()
    {
        m_inputManager.OnMoveLeft += MoveToPreviousPosition;
        m_inputManager.OnMoveRight += MoveToNextPosition;
    }

    private void OnDisable()
    {
        m_inputManager.OnMoveLeft -= MoveToPreviousPosition;
        m_inputManager.OnMoveRight -= MoveToNextPosition;
    }

    private void Start()
    {
        m_currentIndex = 1;
        transform.position = m_transforms[m_currentIndex].position;
    }

    public void MoveToNextPosition()
    {
        m_currentIndex = Mathf.Clamp(m_currentIndex + MoveSpeed, 0, m_transforms.Length - 1);
        UpdatePosition();
    }

    public void MoveToPreviousPosition()
    {
        m_currentIndex = Mathf.Clamp(m_currentIndex - MoveSpeed, 0, m_transforms.Length - 1);
        UpdatePosition();
    }

    public void MoveToDirection(int direction)
    {
        m_currentIndex = Mathf.Clamp(m_currentIndex + MoveSpeed * direction, 0, m_transforms.Length - 1);
        UpdatePosition();
    }
    public void UpdatePosition()
    {
        transform.position = m_transforms[m_currentIndex].position;
        m_audioEventDispatcher.Playaudio(AudioType.PlayerMovement);
    }
}
