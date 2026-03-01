using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputPlayerManagerCustom : MonoBehaviour
{
  public event Action OnMoveLeft; // event dispatcher left
  public event Action OnMoveRight; // event dispatcher right

  [SerializeField] private float  _tapDuration = 1.0f;
  private float _tapTimer = 0.0f;
  private bool _isTouching = false;
  private float width = 0.0f;
  private float height = 0.0f;
  
  private Vector2 startPosition;
  private Vector2 endPosition;

  private InputAction _tapAction;

  private void Start()
  {
    width = Screen.width;
    height = Screen.height;
    
    _tapAction = InputSystem.actions.FindAction("Tap");
  }

  //public void OnTap()
  //{
  //  Debug.Log("OnTap");
  //}

  private void Update()
  {
    if (Touch.activeTouches.Count <= 0)
    {
      return;
    }
    Touch touch = _tapAction.ReadValue<Touch>();
    if (touch.phase == TouchPhase.Began)
    {
      startPosition = touch.screenPosition;
    }
    else if (touch.phase == TouchPhase.Moved)
    {
      endPosition = touch.screenPosition;
      OnSwipe();
    }
    
    // if (Input.touchCount > 0)
    // {
    //   Touch firstTouch = Input.GetTouch(0);
    //
    //   if (firstTouch.phase == TouchPhase.Began)
    //   {
    //     _isTouching = true;
    //   }
    //   else if (firstTouch.phase == TouchPhase.Ended)
    //   {
    //     _isTouching = false;
    //     if (_tapTimer <= _tapDuration)
    //     {
    //       Debug.LogWarning($"Tap OK Touch at {firstTouch.position}");
    //
    //       if (firstTouch.position.x < width / 2)
    //       {
    //         MoveRight();
    //       }
    //       else
    //       {
    //         MoveLeft();
    //       }
    //     } 
    //     _tapTimer = 0.0f;
    //   }
    // }  

    if (_isTouching)
    {
      _tapTimer += Time.deltaTime;
    }

             
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      MoveRight();
    }

    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      MoveLeft();
    }
  }
           

  public void MoveLeft()
  {
    OnMoveLeft?.Invoke(); // appel de l'event dispatcher associé
  }

  public void MoveRight()
  {
    OnMoveRight?.Invoke(); // appel de l'event dispatcher associé
  }

  public void OnSwipe()
  {
    Vector2 delta = endPosition - startPosition;
    delta = delta.normalized;
               
    float dot = Vector2.Dot(delta, Vector2.right);

    if (Mathf.Abs(dot) > 0.7f)
    {
      if (dot < 0.0f)
      {
        MoveLeft();
      }
      else
      {
        MoveRight();
      }
    }
  }
}
         