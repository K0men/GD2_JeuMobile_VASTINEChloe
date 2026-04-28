using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct SpriteEntry
{
    public Sprite sprite;
    public AudioClip sound;
    public bool isCorrect;
}

public class SpriteSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private AudioEventDispatcher _audioDispatcher;

    [Header("Sprites (assign 4)")]
    [SerializeField] private SpriteEntry[] _sprites;

    [Header("Layout")]
    [SerializeField] private float _spacing = 0f;

    [Header("Phases")]
    [SerializeField] private float _phase1Duration = 2f;
    [SerializeField] private float _phase2Duration = 1f;
    [SerializeField] private float _phase3Duration = 0.5f;

    [Header("Scenes")]
    [SerializeField] private string _winScene;
    [SerializeField] private string _loseScene;

    private const int SpriteCount = 4;
    private const int TouchesRequired = 3;

    private readonly List<SpriteSlot> _slots = new();
    private float _displayDuration;
    private int _correctCount;
    private int _wrongCount;

    private void Start()
    {
        _displayDuration = _phase1Duration;
        SpawnSprites();
        StartCoroutine(CycleSprites());
    }

    private void SpawnSprites()
    {
        if (_sprites.Length != SpriteCount)
        {
            Debug.LogError($"SpriteSpawner requires exactly {SpriteCount} sprites.");
            return;
        }

        List<int> order = ShuffledIndices(SpriteCount);

        for (int i = 0; i < SpriteCount; i++)
        {
            Vector3 pos = _spawnPosition.position + Vector3.right * (i - (SpriteCount - 1) / 2f) * _spacing;
            GameObject slot = Instantiate(_slotPrefab, pos, Quaternion.identity);
            int spriteIndex = order[i];
            SpriteSlot spriteSlot = slot.GetComponent<SpriteSlot>();
            spriteSlot.Initialize(
                _sprites[spriteIndex].sprite,
                _sprites[spriteIndex].sound,
                _sprites[spriteIndex].isCorrect,
                _audioDispatcher,
                OnSlotTouched
            );
            spriteSlot.SetVisible(false);
            _slots.Add(spriteSlot);
        }
    }

    private IEnumerator CycleSprites()
    {
        int lastIndex = -1;

        while (true)
        {
            int next;
            do { next = UnityEngine.Random.Range(0, SpriteCount); } while (next == lastIndex && SpriteCount > 1);

            for (int i = 0; i < _slots.Count; i++)
                _slots[i].SetVisible(i == next);

            lastIndex = next;
            yield return new WaitForSeconds(_displayDuration);
        }
    }

    private void OnSlotTouched(bool isCorrect)
    {
        if (isCorrect)
        {
            _correctCount++;
            if (_correctCount == 1)
            {
                _displayDuration = _phase2Duration;
                RestartCycle();
            }
            else if (_correctCount == 2)
            {
                _displayDuration = _phase3Duration;
                RestartCycle();
            }
            else if (_correctCount >= TouchesRequired)
            {
                SceneManager.LoadScene(_winScene);
            }
        }
        else
        {
            _wrongCount++;
            if (_wrongCount >= TouchesRequired) SceneManager.LoadScene(_loseScene);
        }
    }

    private void RestartCycle()
    {
        StopAllCoroutines();
        StartCoroutine(CycleSprites());
    }

    private List<int> ShuffledIndices(int count)
    {
        List<int> indices = new List<int>(count);
        for (int i = 0; i < count; i++) indices.Add(i);
        for (int i = count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        return indices;
    }
}