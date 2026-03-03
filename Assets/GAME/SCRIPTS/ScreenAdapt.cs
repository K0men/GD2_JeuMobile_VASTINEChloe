using UnityEngine;


public class ScreenAdapt : MonoBehaviour
{
    [Header("Reference Resolution")]
    [SerializeField] private float _referenceHeight = 1920f;
    [SerializeField] private float _pixelsPerUnit = 1f;

    [Header("Lane Setup")]
    [SerializeField] private Transform[] _lanePositions;
    [SerializeField] private float _laneMarginPercent = 0.15f; 

    [Header("Spawner")]
    [SerializeField] private CollectibleSpawner _spawner;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        AdaptToScreen();
    }

    private void AdaptToScreen()
    {

        _camera.orthographicSize = Screen.height / 2f / _pixelsPerUnit;

        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;

        // Reposition lanes evenly across visible width
        if (_lanePositions != null && _lanePositions.Length > 0)
        {
            float usableLeft = -halfWidth + halfWidth * _laneMarginPercent;
            float usableRight = halfWidth - halfWidth * _laneMarginPercent;
            float step = (usableRight - usableLeft) / Mathf.Max(1, _lanePositions.Length - 1);

            for (int i = 0; i < _lanePositions.Length; i++)
            {
                float x = _lanePositions.Length == 1
                    ? 0f
                    : usableLeft + step * i;
                _lanePositions[i].position = new Vector3(x, _lanePositions[i].position.y, 0f);
            }
        }

        if (_spawner != null)
        {
            _spawner.SetSpawnY(halfHeight + 50f);
        }
    }
}
