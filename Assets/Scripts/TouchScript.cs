using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchScript : MonoBehaviour
{
    GameObject board;
    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;
    Touch firstTouch;
    private int touchState;
    private Animator blankBlockAnimator;
    float acumTime;
    float holdTime;
    float moveThreshold = 1.5f;
    bool isitfirstBlock=true;
    private BoardManager boardScript;


    #region VibrateBlackbox
    public static class Vibration
    {

#if (UNITY_ANDROID && !UNITY_EDITOR)
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#endif

        public static void Vibrate()
        {
            if (isAndroid())
                vibrator.Call("vibrate");
            else
                Handheld.Vibrate();
        }


        public static void Vibrate(long milliseconds)
        {
            if (isAndroid())
                vibrator.Call("vibrate", milliseconds);
            else
                Handheld.Vibrate();
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            if (isAndroid())
                vibrator.Call("vibrate", pattern, repeat);
            else
                Handheld.Vibrate();
        }

        public static bool HasVibrator()
        {
            return isAndroid();
        }

        public static void Cancel()
        {
            if (isAndroid())
                vibrator.Call("cancel");
        }

        private static bool isAndroid()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
	return true;
#else
            return false;
#endif
        }
    }

    #endregion


    // Use this for initialization
    void Start()
    {
    }
    Vector2 CurrentTouchPosition
    {
        get
        {
            firstTouch = Input.GetTouch(0);
            Vector2 inputPos;
            //inputPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            inputPos = Camera.main.ScreenToWorldPoint(new Vector2(firstTouch.position.x, firstTouch.position.y));

            return inputPos;
        }
    }

    private bool HasInput
    {
        get
        {
            return Input.touchCount > 0;
        }
    }

    private void DragBoard()
    {
        board = GameObject.Find("Board");
        var inputPosition = CurrentTouchPosition;
        if (draggingItem)
        {
            draggedObject.transform.position = inputPosition + touchOffset;
        }
        else
        {
            // this always runs before the first if so dragged object will be defined
            //RaycastHit2D[] objectsHit = Physics2D.RaycastAll(inputPosition, inputPosition, 50f);
            //if (objectsHit.Length > 0)
            //{
            //    var hit = objectsHit[0];
            //    if (hit.transform != null)
            //    {
            //        draggingItem = true;
            //        
            //        draggedObject = hit.transform.gameObject;
            //        touchOffset = (Vector2)hit.transform.position - inputPosition;
            //        //draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            //    }
            //
            //}
            draggingItem = true;
            draggedObject = board;
            touchOffset = (Vector2)board.transform.position - inputPosition;
        }
    }

    void DropItem()
    {
        draggingItem = false;
    }


  
    int IsTapDragOrPress()
    {
        // once a touch is determined as tap drag or press it cant be anything else again
        holdTime = 0.5f;
        firstTouch = Input.GetTouch(0);

        if (firstTouch.phase == TouchPhase.Moved&& Mathf.Abs(firstTouch.deltaPosition.x) > moveThreshold && Mathf.Abs(firstTouch.deltaPosition.y) > moveThreshold)
        {
            //Debug.Log("im dragging");
            touchState = 2;
            
        }
        else
        {
            if (acumTime >= holdTime)
            {
                touchState = 3;
            }
            else
            {

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                   ResetTime();
                   touchState = 1;
                   
                }
            }
        }
        return touchState;
    }
    float AccuminlateTime()
    {
        acumTime += Input.GetTouch(0).deltaTime;
        return acumTime;
    }

    void ResetTime()
    {
        acumTime = 0;
    }
    RaycastHit2D[] RaycastObjectsTouched()
    {
        var inputPosition = CurrentTouchPosition;
        RaycastHit2D[] objectsHit = Physics2D.RaycastAll(inputPosition, inputPosition, 1f);
        return objectsHit;

    }
    //Update is called once per frame
    void BlankBlockAnimation(int touchState_)// returns array of objects hit
    {
        if (touchState_ != 0)
        {
            var hit =RaycastObjectsTouched()[0];
            
            blankBlockAnimator = hit.transform.gameObject.GetComponent<Animator>();
            if (touchState_ == 1)
            {

                //if (isitfirstBlock)
                //{
                //    boardScript = GetComponent<BoardManager>();
                //    boardScript.LayoutMines(10, hit.transform.position);// TODO add from GUI text shouldnt be here anyway
                //    isitfirstBlock=false;
                //}
                blankBlockAnimator.SetBool("TempClick", true);
            }
            else if (touchState_ == 3)
            {
                //Vibration.Vibrate(500);
                //Handheld.Vibrate();
                switch (blankBlockAnimator.GetInteger("HoldClick"))
                {
                    case 3:
                        blankBlockAnimator.SetInteger("HoldClick", 1);
                        break;
                    case 1:
                        blankBlockAnimator.SetInteger("HoldClick", 2);
                        break;
                    case 2:
                        blankBlockAnimator.SetInteger("HoldClick", 3);
                        break;
                }
                
            }
        }
    }
    void Update()
    {
        if (HasInput)
        {
            AccuminlateTime();
            if (touchState == 0 || touchState==1)// check if touch started or it's a tap (not yet anything else)
            {
                IsTapDragOrPress(); //assign touchState to a value
                if (isitfirstBlock)
                {
                    
                    boardScript = GetComponent<BoardManager>();
                    boardScript.InitialiseList();
                    boardScript.LayoutMines(10);// TODO add from GUI text
                    isitfirstBlock = false;
                }
                BlankBlockAnimation(touchState);
            }
            
            if (touchState == 2)
            {
                Debug.Log("im dragging");
                DragBoard();
            }
            
        }
        else
        {
            if (draggingItem) DropItem();// sets draggingitem to false and stops the loop
            if (touchState != 0)
            {
                touchState = 0;
                blankBlockAnimator.SetBool("TempClick", false);
            }
                
            if (acumTime!=0)ResetTime();
        }
    }
}
