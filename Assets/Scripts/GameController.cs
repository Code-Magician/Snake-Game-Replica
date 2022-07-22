using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Sprite tailSprite = null;
    public Sprite bodySprite = null;
    static public GameController instance = null;

    //speed of snake
    public float speed = 1f;

    //referece to body prefab
    public BodyParts snakeBodyPrefab = null;

    public SnakeHead snakeHead = null;

    public GameObject rockPrefab = null;

    //width and height from centre of canvas
    const float width = 3.3f;
    const float height = 7f;

    public GameObject eggPrefab = null;
    public GameObject goldenEggPrefab = null;
    public GameObject spikePrefab = null;


    public bool isAlive = false;

    public bool waitingToPlay = true;

    List<Egg> allEggs = new List<Egg>();
    List<Spike> allSpikes = new List<Spike>();

    int score = 0;
    int highscore = 0;

    //levels
    public int levelNo = 0;
    public int noOfEggsForNextLvl = 0;

    public Text scoreText = null;
    public Text highScoreText = null;
    public Text tapToPlay = null;
    public Text gameOver = null;
    public Text levelNoText = null;
    

    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Debug.Log("Game Starting...");

        createWalls();

        createEgg();
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingToPlay)
        {
            foreach (Touch x in Input.touches)
            {
                if (x.phase == TouchPhase.Ended)
                {
                    startGamePlay();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                startGamePlay();
            }
        }

        updateScore();
    }


    void createWalls()
    {
        Vector3 start = new Vector3(-width, -height, -1f);
        Vector3 finish = new Vector3(-width, +height, -1f);
        createWall(start, finish);

        start = new Vector3(-width, -height, -1f);
        finish = new Vector3(+width, -height, -1f);
        createWall(start, finish);

        start = new Vector3(+width, -height, -1f);
        finish = new Vector3(+width, +height, -1f);
        createWall(start, finish);

        start = new Vector3(-width, +height, -1f);
        finish = new Vector3(+width, +height, -1f);
        createWall(start, finish);

    }

    void createWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 3f);
        Vector3 delta = (finish - start) / noOfRocks;
        Vector3 position = start;
        for(int i = 0; i <= noOfRocks; i++)
        {
            float scale = Random.Range(1.5f, 2f);
            float rotation = Random.Range(0, 360f);
            createRock(position, scale, rotation);

            position += delta;
        }
    }

    void createRock(Vector3 pos, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, pos, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }


    void createEgg(bool isGolden = false)
    {
        float x = Random.Range(-width + 0.6f, width - 0.6f);
        float y = Random.Range(-height + 0.6f, height - 0.6f);

        Vector3 pos = new Vector3(x, y, -1f);
        Egg egg = null;
        if (isGolden)
            egg = Instantiate(goldenEggPrefab, pos, Quaternion.identity).GetComponent<Egg>();
        else
            egg = Instantiate(eggPrefab, pos, Quaternion.identity).GetComponent<Egg>();

        allEggs.Add(egg);
    }


    public void GameOver()
    {
        isAlive = false;
        waitingToPlay = true;
        gameOver.gameObject.SetActive(true);
        tapToPlay.gameObject.SetActive(true);
        if (score > highscore)
            highscore = score;
        
    }

    public void eggEaten(Egg egg)
    {
        score += 1;
        noOfEggsForNextLvl--;
        if (noOfEggsForNextLvl == 0)
        {
            score += 10;
            if (score > highscore)
            {
                highscore = score;
                updateHighScore();
            }
            levelUp();
        }
        else if (noOfEggsForNextLvl == 1)
            createEgg(true);
        else
            createEgg();

        updateScore();

        allEggs.Remove(egg);
        Destroy(egg.gameObject);
    }

    public void startGamePlay()
    {
        levelNo = 0;
        gameOver.gameObject.SetActive(false);
        tapToPlay.gameObject.SetActive(false);
        score = 0;
        updateScore();
        updateHighScore();
        waitingToPlay = false;
        isAlive = true;
        destroyAllEggs();
        destroyAllSpikes();

        levelUp();
    }

   void destroyAllEggs()
    {
        foreach(Egg x in allEggs)
        {
            Destroy(x.gameObject);
        }
        allEggs.Clear();
    }


    void levelUp()
    {
        levelNo++;

        destroyAllSpikes();
        createSpikes();
        noOfEggsForNextLvl = 4 + (levelNo * 2);

        speed = 1f + (levelNo)/4f;
        if (speed >= 6f)
            speed = 6f;

        snakeHead.resetMemory();
        snakeHead.ResetSnake();
        createEgg();

        levelNoText.gameObject.SetActive(true);
        levelNoText.text = "Level = " + levelNo;
        updateHighScore();
    }

    void updateScore()
    {
        scoreText.text = "SCORE ::::: " + score.ToString();
    }

    void updateHighScore()
    {
        highScoreText.text = "HIGHSCORE ::::: " + highscore.ToString();
    }


    //spikes
    void createSpikes()
    {
        int noOfSpikes = levelNo;
        if (noOfSpikes >= 5)
            noOfSpikes = 5;
        for(int i=1; i <= noOfSpikes ; i++)
        {
            float x = Random.Range(-width + 0.6f, width - 0.6f);
            float y = Random.Range(-height + 0.6f, height - 0.6f);

            Vector3 pos = new Vector3(x, y, -1f);
            Spike spike = null;
            spike = Instantiate(spikePrefab, pos, Quaternion.identity).GetComponent<Spike>();

            allSpikes.Add(spike);
        }
    }
    



    void destroyAllSpikes()
    {
        foreach (Spike x in allSpikes)
        {
            Destroy(x.gameObject);
        }
        allSpikes.Clear();
    }
}
