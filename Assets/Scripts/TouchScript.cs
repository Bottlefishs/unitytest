using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScript : MonoBehaviour
{
    Transform boardHolder;
    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;
    Touch firstTouch;

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

    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;
        if (draggingItem)
        {
            draggedObject.transform.position = inputPosition + touchOffset;
        }
        else
        {
            RaycastHit2D[] objectsHit = Physics2D.RaycastAll(inputPosition, inputPosition, 50f);
            if (objectsHit.Length > 0)
            {
                var hit = objectsHit[0];
                if (hit.transform != null)
                {
                    draggingItem = true;
                    draggedObject = hit.transform.gameObject;
                    touchOffset = (Vector2)hit.transform.position - inputPosition;
                    //draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                }

            }
        }
    }

    void DropItem()
    {
        draggingItem = false;
    }



    //bool IsTap() {
    //    if (firstTouch.phase == TouchPhase.Began) {
    //        TouchTime = Time.time;​
    //    }
    //
    //    if (firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled)
    //    {
    //        if (Time.time - TouchTime <= 0.5)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }​
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        if (HasInput)
        {
            DragOrPickUp();
        }
        else
        {
            if (draggingItem) DropItem();
        }
        //boardHolder = GameObject.Find("board").transform;
        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
        //
        //    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
        //    {
        //        // get the touch position from the screen touch to world point
        //        Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
        //        // lerp and set the position of the current object to that of the touch, but smoothly over time.
        //        boardHolder.position += Vector3.Lerp(transform.position, touchedPos, Time.deltaTime);
        //
        //    }
        //}
    }
}
