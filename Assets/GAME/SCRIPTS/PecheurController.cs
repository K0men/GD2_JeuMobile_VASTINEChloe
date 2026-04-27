using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class PecheurController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite pecheurIdle;
    [SerializeField] private Sprite pecheurFishing;
    [SerializeField] private Sprite pecheurCatch;

    [Header("Exclamation Mark")]
    [SerializeField] private GameObject exclamationMark;

    [Header("Timing")]
    [Tooltip("Min seconds before the '!' appears after fishing starts.")]
    [SerializeField] private float exclamationDelayMin = 1.5f;
    [Tooltip("Max seconds before the '!' appears after fishing starts.")]
    [SerializeField] private float exclamationDelayMax = 4f;
    [Tooltip("How long the '!' window stays open before closing.")]
    [SerializeField] private float exclamationWindowDuration = 1.5f;

    private enum FishermanState { Idle, Fishing, AlertWindow, Catch }
    private FishermanState _currentState = FishermanState.Idle;
    private Coroutine _exclamationRoutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Start() => EnterIdle();

    private void Update()
    {
        if (!DetectTap()) return;

        switch (_currentState)
        {
            case FishermanState.Idle:
                EnterFishing();
                break;
            case FishermanState.AlertWindow:
                EnterCatch();
                break;
        }
    }

    private void EnterIdle()
    {
        _currentState = FishermanState.Idle;
        spriteRenderer.sprite = pecheurIdle;
        SetExclamationVisible(false);
    }

    private void EnterFishing()
    {
        _currentState = FishermanState.Fishing;
        spriteRenderer.sprite = pecheurFishing;
        SetExclamationVisible(false);

        if (_exclamationRoutine != null) StopCoroutine(_exclamationRoutine);
        _exclamationRoutine = StartCoroutine(ExclamationRoutine());
    }

    private void EnterAlertWindow()
    {
        _currentState = FishermanState.AlertWindow;
        SetExclamationVisible(true);
        _exclamationRoutine = StartCoroutine(AlertWindowRoutine());
    }

    private void EnterCatch()
    {
        if (_exclamationRoutine != null)
        {
            StopCoroutine(_exclamationRoutine);
            _exclamationRoutine = null;
        }
        _currentState = FishermanState.Catch;
        spriteRenderer.sprite = pecheurCatch;
        SetExclamationVisible(false);
    }

    private IEnumerator ExclamationRoutine()
    {
        float delay = Random.Range(exclamationDelayMin, exclamationDelayMax);
        yield return new WaitForSeconds(delay);
        EnterAlertWindow();
    }

    private IEnumerator AlertWindowRoutine()
    {
        yield return new WaitForSeconds(exclamationWindowDuration);
        EnterFishing();
    }

    private bool DetectTap()
    {
        foreach (Touch touch in Touch.activeTouches)
            if (touch.phase == TouchPhase.Began) return true;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) return true;
#endif
        return false;
    }

    private void SetExclamationVisible(bool visible)
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(visible);
    }
}