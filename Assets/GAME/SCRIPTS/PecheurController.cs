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

    [Header("Audio")]
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;

    [Header("Timing")]
    [Tooltip("Min seconds before the '!' appears after fishing starts.")]
    [SerializeField] private float exclamationDelayMin = 1.5f;
    [Tooltip("Max seconds before the '!' appears after fishing starts.")]
    [SerializeField] private float exclamationDelayMax = 4f;
    [Tooltip("How long the '!' window stays open before closing (in seconds).")]
    [SerializeField] private float exclamationWindowDuration = 1f;
    [Tooltip("How long the catch sprite is displayed before returning to Idle.")]
    [SerializeField] private float catchDisplayDuration = 2f;

    [Header("Catch Chances")]
    [Tooltip("Probability of catching a fish (0-1).")]
    [Range(0f, 1f)]
    [SerializeField] private float fishChance = 0.34f;
    [Tooltip("Probability of catching a crab (0-1). Octopus fills the rest.")]
    [Range(0f, 1f)]
    [SerializeField] private float crabChance = 0.33f;

    public CatchType LastCatch { get; private set; } = CatchType.None;

    private enum FishermanState { Idle, Fishing, AlertWindow, Catch }
    private FishermanState _currentState = FishermanState.Idle;
    private Coroutine _activeRoutine;

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
        LastCatch = CatchType.None;
        SetExclamationVisible(false);
    }

    private void EnterFishing()
    {
        _currentState = FishermanState.Fishing;
        spriteRenderer.sprite = pecheurFishing;
        SetExclamationVisible(false);
        StartManagedCoroutine(ExclamationRoutine());
    }

    private void EnterAlertWindow()
    {
        _currentState = FishermanState.AlertWindow;
        SetExclamationVisible(true);
        StartManagedCoroutine(AlertWindowRoutine());
    }

    private void EnterCatch()
    {
        StopManagedCoroutine();

        LastCatch = RollCatchType();
        _currentState = FishermanState.Catch;
        spriteRenderer.sprite = GetCatchSprite(LastCatch);
        SetExclamationVisible(false);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddPoints(GetCatchPoints(LastCatch));

        if (_audioEventDispatcher != null)    
            _audioEventDispatcher.Playaudio(AudioType.Collect); 

        StartManagedCoroutine(CatchDisplayRoutine());
    }
private CatchType RollCatchType()
    {
        float roll = Random.value;
        if (roll < fishChance) return CatchType.Fish;
        if (roll < fishChance + crabChance) return CatchType.Crab;
        return CatchType.Octopus;
    }

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

    /// <summary>Returns the point value awarded for each catch type.</summary>
    private int GetCatchPoints(CatchType catchType)
    {
        return catchType switch
        {
            CatchType.Fish => 1,
            CatchType.Crab => 5,
            CatchType.Octopus => 10,
            _ => 0
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
        EnterFishing();
    }

    private IEnumerator CatchDisplayRoutine()
    {
        yield return new WaitForSeconds(catchDisplayDuration);
        EnterIdle();
    }

    private void StartManagedCoroutine(IEnumerator routine)
    {
        StopManagedCoroutine();
        _activeRoutine = StartCoroutine(routine);
    }

    private void StopManagedCoroutine()
    {
        if (_activeRoutine != null)
        {
            StopCoroutine(_activeRoutine);
            _activeRoutine = null;
        }
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
