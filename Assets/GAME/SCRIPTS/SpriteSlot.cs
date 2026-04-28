using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteSlot : MonoBehaviour, IPointerDownHandler
{
    private AudioClip _sound;
    private AudioEventDispatcher _audioDispatcher;
    private Action<bool> _onTouched;
    private bool _isCorrect;

    public void Initialize(Sprite sprite, AudioClip sound, bool isCorrect, AudioEventDispatcher dispatcher, Action<bool> onTouched)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        _sound = sound;
        _isCorrect = isCorrect;
        _audioDispatcher = dispatcher;
        _onTouched = onTouched;
    }

    public void SetVisible(bool visible)
    {
        GetComponent<SpriteRenderer>().enabled = visible;
        GetComponent<BoxCollider2D>().enabled = visible;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _audioDispatcher.PlayClip(_sound);
        _onTouched?.Invoke(_isCorrect);
    }
}
