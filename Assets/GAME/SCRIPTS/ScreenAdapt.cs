using UnityEngine;

public class ScreenAdapt : MonoBehaviour
{
    [Header("Reference Resolution")]
    [SerializeField] private float _referenceHeight = 1920f;
    [SerializeField] private float _pixelsPerUnit = 1f;

    [Header("Background")]
    [SerializeField] private Transform _background;

    [Header("Lane Setup")]
    [SerializeField] private Transform[] _lanePositions;
    [SerializeField] private float _laneMarginPercent = 0.15f;
    [SerializeField] private float _laneHeightFromBottom = 0.40f;

    [Header("Spawner")]
    [SerializeField] private CollectibleSpawner _spawner;

    [Header("Player")]
    [SerializeField] private Transform _player;

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
        float camX = _camera.transform.position.x;
        float camY = _camera.transform.position.y;

        if (_background != null)
        {
            _background.position = new Vector3(camX, camY, _background.position.z);

            SpriteRenderer sr = _background.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                float spriteW = sr.sprite.bounds.size.x;
                float spriteH = sr.sprite.bounds.size.y;
                float scale = Mathf.Max((halfWidth * 2f) / spriteW, (halfHeight * 2f) / spriteH);
                _background.localScale = new Vector3(scale, scale, 1f);
            }
        }

        float laneY = camY - halfHeight + halfHeight * _laneHeightFromBottom;

        if (_lanePositions != null && _lanePositions.Length > 0)
        {
            float usableLeft = camX - halfWidth + halfWidth * _laneMarginPercent;
            float usableRight = camX + halfWidth - halfWidth * _laneMarginPercent;
            float step = (usableRight - usableLeft) / Mathf.Max(1, _lanePositions.Length - 1);

            for (int i = 0; i < _lanePositions.Length; i++)
            {
                float x = _lanePositions.Length == 1 ? camX : usableLeft + step * i;
                _lanePositions[i].position = new Vector3(x, laneY, 0f);
            }
        }

        if (_player != null && _lanePositions.Length > 0)
            _player.position = _lanePositions[_lanePositions.Length / 2].position;

        if (_spawner != null)
            _spawner.SetSpawnY(camY + halfHeight + 50f);
    }
}
