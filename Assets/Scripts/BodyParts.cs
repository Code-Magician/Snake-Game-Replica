using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyParts : MonoBehaviour
{

    Vector2 deltaPosition;

    public BodyParts following = null;
    private bool isTail = false;
    private SpriteRenderer spriteRenderer = null;

    const int PARTSREMEMBERED = 10;
    public Vector3[] previousPositions = new Vector3[PARTSREMEMBERED];

    public int setIndex = 0;
    public int getIndex = -(PARTSREMEMBERED - 1);

    

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    //for glitch at level up
    public void resetMemory()
    {
        setIndex = 0;
        getIndex = -(PARTSREMEMBERED - 1);
    }
    virtual public void Update()
    {
        if (!GameController.instance.isAlive)
            return;

        //getting position of object to follow...
        Vector3 followingPositon;
        if(following != null)
        {
            if(following.getIndex > -1)
            {
                followingPositon = following.previousPositions[following.getIndex];
            }
            else
            {
                followingPositon = following.transform.position;
            }
        }
        else
        {
            followingPositon = gameObject.transform.position;
        }

        previousPositions[setIndex].x = gameObject.transform.position.x;
        previousPositions[setIndex].y = gameObject.transform.position.y;
        previousPositions[setIndex].z = gameObject.transform.position.z;

        setIndex++;
        if (setIndex >= PARTSREMEMBERED)
            setIndex = 0;

        getIndex++;
        if (getIndex >= PARTSREMEMBERED)
            getIndex = 0;

        //setting to follow that object
        if(following != null)
        {
            Vector3 newposition;
            if (following.getIndex > -1)
                newposition = followingPositon;
            else
                newposition = following.transform.position;

            newposition.z += 0.01f;

            //setting updating positions and directins of bodyparts
            setMoveMent(newposition - gameObject.transform.position);
            UpdateDirection();
            updatePosition();
        }
        

    }

    public void setMoveMent(Vector2 movement)
    {
        deltaPosition = movement;
    }

    public void updatePosition()
    {
        gameObject.transform.position += (Vector3)deltaPosition;
    }

    public void UpdateDirection()
    {
        if (deltaPosition.y > 0)
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        else if (deltaPosition.y < 0)
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
        else if (deltaPosition.x > 0)
            gameObject.transform.localEulerAngles = new Vector3(0, 0, -90);
        else if(deltaPosition.x < 0)
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
    }

    public void TurnIntoTail()
    {
        isTail = true;
        spriteRenderer.sprite = GameController.instance.tailSprite;
    }

    public void TurnIntoBodyPart()
    {
        isTail = false;
        spriteRenderer.sprite = GameController.instance.bodySprite;
    }
}
