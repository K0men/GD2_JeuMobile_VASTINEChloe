using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PecheurController : MonoBehaviour
{
    public enum CatchType { None, Fish, Crab, Octopus }

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite pecheurIdle;
    [SerializeField] private Sprite pecheurFishing;
    [SerializeField] private Sprite pecheurCatchFish;
    [SerializeField] private Sprite pecheurCatchCrab;
    [SerializeField] private Sprite pecheurCatchOctopus;

    [Header("Exclamation Mark")]
    [SerializeField] private GameObject exclamationMark;

    [Header("Timing")]
    [Tooltip("Min seconds before the '!' appears after fishing starts.")]
    [SerializeField] private float exclamationDelayMin = 1.5f;
    [Tooltip("Max seconds before the '!' appears after fishing starts.")]
    [SerializeField] private float exclamationDelayMax = 4f;
    [Tooltip("How long the '!' window stays open before closing (in seconds).")]
    [SerializeField] private float exclamationWindowDuration = 1f;

    [Header("Catch Chances")]
    [Tooltip("Probability of catching a fish (0–1).")]
    [Range(0f, 1f)]
    [SerializeField] private float fishChance = 0.34f;
    [Tooltip("Probability of catching a crab (0–1). Octopus fills the rest.")]
    [Range(0f, 1f)]
    [SerializeField] private float crabChance = 0.33f;

    /// <summary>The result of the most recent catch attempt.</summary>
    public CatchType LastCatch { get; private set; } = CatchType.None;

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
            case FishermanState.Catch:
                EnterIdle();
                break;
        }
    }

    private void EnterIdle()
    {
        _currentState = FishermanState.Idle;
        spriteRenderer.sprite = pecheurIdle;
        LastCatch = CatchType.None;
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

        LastCatch = RollCatchType();
        _currentState = FishermanState.Catch;
        spriteRenderer.sprite = GetCatchSprite(LastCatch);
        SetExclamationVisible(false);
    }

    /// <summary>Rolls a random catch result based on the configured probabilities.</summary>
    private CatchType RollCatchType()
    {
        float roll = Random.value;
        if (roll < fishChance) return CatchType.Fish;
        if (roll < fishChance + crabChance) return CatchType.Crab;
        return CatchType.Octopus;
    }

    /// <summary>Returns the sprite associated with the given catch type.</summary>
    private Sprite GetCatchSprite(CatchType catchType)
    {
        return catchType switch
        {
            CatchType.Fish => pecheurCatchFish,
            CatchType.Crab => pecheurCatchCrab,
            CatchType.Octopus => pecheurCatchOctopus,
            _ => pecheurIdle
        };
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
        // Player missed the window — restart fishing
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
