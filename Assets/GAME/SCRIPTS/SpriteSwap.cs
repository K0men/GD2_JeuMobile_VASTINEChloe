using UnityEngine;
public class SpriteSwapper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    private const int NoSprite = -1;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(int index)
    {
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("[SpriteSwapper] No sprites assigned.", this);
            return;
        }

        if (index == NoSprite)
        {
            spriteRenderer.sprite = null;
            return;
        }

        if (index < 0 || index >= sprites.Length)
        {
            Debug.LogWarning($"[SpriteSwapper] Index {index} is out of range (0–{sprites.Length - 1}).", this);
            return;
        }

        spriteRenderer.sprite = sprites[index];
    }

    public void ClearSprite() => SetSprite(NoSprite);
}