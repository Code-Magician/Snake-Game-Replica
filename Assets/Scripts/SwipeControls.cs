using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeControls : MonoBehaviour
{
    Vector2 swipeStart;
    Vector2 swipeEnd;
    //minimum distance at which swipe will be considered valid
    float miniDist = 10;

    //it's a delegate(which is the way of storing funcitons) which can be used in other classes
    public static event System.Action<SwipeDirection> OnSwipe = delegate{};

    public enum SwipeDirection
    {
        Up, Down, Left, Right
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this is touches stores all the begin and ending touches on screen in phone
        foreach(Touch x in Input.touches)
        {
            // phase tells us weather touch was a begin or end in swipe
            if(x.phase == TouchPhase.Began)
            {
                swipeStart = x.position;
            }
            else if(x.phase == TouchPhase.Ended)
            {
                swipeEnd = x.position;
                //now we have both staring and ending positon of swipe so we process it now.
                processSwipe();
            }
        }


        //for swipe with mouse...
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
            processSwipe();
        }
    }

    //processes swipe
    void processSwipe()
    {
        float distance = Vector2.Distance(swipeStart, swipeEnd);
        if(distance > miniDist)
        {
            if (isVerticalSwipe())
            {
                if(swipeEnd.y > swipeStart.y)
                {
                    OnSwipe(SwipeDirection.Up);
                }
                else
                {
                    OnSwipe(SwipeDirection.Down);
                }
            }
            else
            {
                if (swipeEnd.x > swipeStart.x)
                {
                    OnSwipe(SwipeDirection.Right);
                }
                else
                {
                    OnSwipe(SwipeDirection.Left); ;
                }
            }
        }
    }

    //it checks if vertical distance is greater than horizontal then it return true.
    bool isVerticalSwipe()
    {
        //mathf.abs() makes number absolute ... or without sign..
        float horizontal = Mathf.Abs(swipeEnd.x - swipeStart.x);
        float vertical = Mathf.Abs(swipeEnd.y - swipeStart.y);

        if (vertical > horizontal)
            return true;
        return false;
    }
}
