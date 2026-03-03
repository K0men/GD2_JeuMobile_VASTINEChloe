using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int _pointValue = 10;
    [SerializeField] private float _fallSpeed = 3f;
    [SerializeField] private float _destroyBelowY = -10f;

    private void Update()
    {
        transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime);

        float bottomBound = Camera.main.transform.position.y - Camera.main.orthographicSize - 50f;
        if (transform.position.y < bottomBound)
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ScoreManager.Instance.AddPoints(_pointValue);
        Destroy(gameObject);
    }
}
