using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : BodyParts
{
    Vector2 movement;

    public BodyParts Tail = null;

    //time to add parts
    const float TIMETOADDBODYPARTS = 0.1f;
    float addtimer = TIMETOADDBODYPARTS;

    public int partsToAdd = 0;

    List<BodyParts> bodyParts = new List<BodyParts>();

    //audios
    public AudioSource[] gulpSounds = new AudioSource[3];
    public AudioSource dieSound = null;

    // Start is called before the first frame update
    void Start()
    {
        SwipeControls.OnSwipe += swipeDetection;
    }

    // Update is called once per frame
    //overide is used to run the content of both the update functions in bodyParts and SnakeHead...
    override public void Update()
    {
        if (!GameController.instance.isAlive)
            return;
        
        //only head is connected through snakeHead class so this is needed to stores heads last 10 frame positons in circular array.
        base.Update();

        setMoveMent(movement*Time.deltaTime);
        UpdateDirection();
        updatePosition();

        //add parts to game
        if(partsToAdd > 0)
        {
            addtimer -= Time.deltaTime;
            if(addtimer <= 0)
            {
                addtimer = TIMETOADDBODYPARTS;
                addBodyPart();
                partsToAdd--;
            }
        }

        
    }

    void swipeDetection(SwipeControls.SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeControls.SwipeDirection.Up:
                MoveUp();
                break;

            case SwipeControls.SwipeDirection.Down:
                MoveDown();
                break;

            case SwipeControls.SwipeDirection.Left:
                MoveLeft();
                break;

            case SwipeControls.SwipeDirection.Right:
                MoveRight();
                break;
        }
    }

    void MoveUp()
    {
        movement = Vector2.up * GameController.instance.speed;
    }

    void MoveDown()
    {
        movement = Vector2.down * GameController.instance.speed;
    }

    void MoveLeft()
    {
        movement = Vector2.left * GameController.instance.speed;
    }

    void MoveRight()
    {
        movement = Vector2.right* GameController.instance.speed;
    }

    void addBodyPart()
    {
        if(Tail == null)
        {
            Vector3 newposition = transform.position;
            newposition.z = newposition.z + 0.01f;
            BodyParts x = Instantiate(GameController.instance.snakeBodyPrefab, newposition, Quaternion.identity);
            x.following = this;
            Tail = x;
            x.TurnIntoTail();

            bodyParts.Add(x);
        }
        else
        {
            Vector3 newposition = Tail.transform.position;
            newposition.z = newposition.z + 0.01f;
            BodyParts x = Instantiate(GameController.instance.snakeBodyPrefab, newposition, Tail.transform.rotation);
            x.following = Tail;
            x.TurnIntoTail();
            Tail.TurnIntoBodyPart();
            Tail = x;

            bodyParts.Add(x);
        }

    }


    //reset snake
    public void ResetSnake()
    {
        foreach(BodyParts x in bodyParts)
        {
            Destroy(x.gameObject);
        }
        bodyParts.Clear();

        Tail = null;
        MoveUp();

        gameObject.transform.position = new Vector3(0, 0, -8);
        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

        addtimer = TIMETOADDBODYPARTS;
        partsToAdd = 5;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Egg egg = collision.GetComponent<Egg>();
        if (egg) {
            EatEgg(egg);
            int x = Random.Range(0, 3);
            gulpSounds[x].Play();
        }
        else {
            dieSound.Play();
            GameController.instance.GameOver();
        }
    }

    private void EatEgg(Egg egg)
    {
        partsToAdd = 5;
        addtimer = 0;
        GameController.instance.eggEaten(egg);
    }
}
