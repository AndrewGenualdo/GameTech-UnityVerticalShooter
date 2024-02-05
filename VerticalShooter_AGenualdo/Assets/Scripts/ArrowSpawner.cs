using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
public class ArrowSpawner : MonoBehaviour
{



    [SerializeField] public GameObject arrowPrefab;

    public static ArrowSpawner instance;

    const int WAIT = -10;

    const int DIR_UP = 0;
    const int DIR_RIGHT = 1;
    const int DIR_DOWN = 2;
    const int DIR_LEFT = 3;

    const int UP = 0;
    const int RIGHT = 11;
    const int DOWN = 22;
    const int LEFT = 33;

    //SHIFTERS (STARTDIRECTION_ENDDIRECTION)
    const int UP_UP = 00; //1
    const int UP_RIGHT = 01; //2
    const int UP_DOWN = 02; //3
    const int UP_LEFT = 03; //4

    const int RIGHT_UP = 10; //5
    const int RIGHT_RIGHT = 11; //6
    const int RIGHT_DOWN = 12; //7
    const int RIGHT_LEFT = 13; //8

    const int DOWN_UP = 20; //9
    const int DOWN_RIGHT = 21; //10
    const int DOWN_DOWN = 22; //11
    const int DOWN_LEFT = 23; //12

    const int LEFT_UP = 30; //13
    const int LEFT_RIGHT = 31; //14
    const int LEFT_DOWN = 32; //15
    const int LEFT_LEFT = 33; //16

    const int RANDOM = -8;

    //WAIT - 17-20

    private static int[] ARROW_TYPES = { UP, UP_RIGHT, UP_DOWN, UP_LEFT, RIGHT_UP, RIGHT_RIGHT, RIGHT_DOWN, RIGHT_LEFT, DOWN_UP, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT_UP, LEFT_RIGHT, LEFT_DOWN, LEFT, WAIT, WAIT};


    public Coroutine arrowSpawnerRoutine;

    public List<GameObject> arrows = new List<GameObject>();

    const float ANIMATION_LENGTH = 1000.0f;

    public int level;

    //levels[0] = ms between arrows
    //levels[1] = arrow speed

    //pretty easy introduction level (konami code lol)
    private static int[] level1 = { 750, 2, UP, UP, DOWN, DOWN, LEFT, RIGHT, LEFT, RIGHT, WAIT, WAIT, 
        UP, RIGHT, DOWN, LEFT}; //14 long


    //Similar but faster (more challenging)
    private static int[] level2 = { 750, 5, RIGHT, DOWN, UP, LEFT, LEFT, RIGHT, LEFT, UP, DOWN, RIGHT, 
        UP, RIGHT, DOWN}; //13 long


    //Exposing the player to a lot of arrows while not in much real danger
    private static int[] level3 = { 350, 1, UP, RIGHT, DOWN, LEFT, DOWN, RIGHT, DOWN, LEFT, UP, RIGHT, 
        DOWN, RIGHT, UP, DOWN, RIGHT, UP, LEFT, RIGHT, WAIT}; //19 long


    //SPEEEED
    private static int[] level4 = { 450, 10, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, LEFT, UP, RIGHT,
        LEFT, UP, DOWN, DOWN, RIGHT, UP, RIGHT, DOWN, LEFT}; //19 long


    //introduces the player to "rotating" arrows
    private static int[] level5 = { 2250, 2, UP_LEFT, RIGHT_LEFT, DOWN_LEFT, WAIT, UP_RIGHT, LEFT_RIGHT, DOWN_RIGHT, WAIT, UP_DOWN, DOWN_UP}; //10 long


    //slightly more challenging rotating arrows
    private static int[] level6 = { 750, 4, RIGHT_UP, LEFT_DOWN, RIGHT_UP, DOWN_RIGHT, DOWN_RIGHT, RIGHT_UP, LEFT_DOWN, UP_LEFT, LEFT_DOWN, RIGHT_UP,
        LEFT_DOWN, RIGHT_DOWN, LEFT_UP, DOWN_LEFT, RIGHT_DOWN, LEFT_UP, RIGHT, LEFT, RIGHT, 
        LEFT, LEFT}; //12 long


    //harder
    private static int[] level7 = { 666, 3, RIGHT_UP, RIGHT_DOWN, UP, DOWN, LEFT_RIGHT, RIGHT_LEFT, RIGHT_DOWN, UP_RIGHT, LEFT_UP, DOWN_LEFT}; //10 long
    
    
    //harder yet
    private static int[] level8 = { 666, 4, RIGHT_LEFT, DOWN_UP, UP_LEFT, UP, LEFT_UP, RIGHT, RIGHT_UP, RIGHT_DOWN, DOWN_UP, UP_RIGHT,
        DOWN, DOWN_RIGHT, LEFT_RIGHT, WAIT, RIGHT_LEFT, RIGHT_UP, WAIT, LEFT_UP, UP, UP_LEFT }; //20 long


    //Lots of arrows but rotating now
    private static int[] level9 = { 325, 1, RIGHT_DOWN, WAIT, UP, DOWN_UP, RIGHT_DOWN, RIGHT, LEFT, LEFT_DOWN, WAIT, RIGHT_LEFT,
        RIGHT_LEFT, DOWN_RIGHT, UP_RIGHT, WAIT, DOWN, UP_LEFT, RIGHT, DOWN, UP_RIGHT, DOWN_UP,
        RIGHT_DOWN, LEFT_DOWN}; //22 long

    //final boss
    private static int[] level10 = { 500, 8, DOWN_RIGHT, LEFT_DOWN, RIGHT_DOWN, LEFT_RIGHT, DOWN_LEFT, LEFT_RIGHT, UP, LEFT_DOWN, WAIT, DOWN_LEFT,
        UP_LEFT, RIGHT_LEFT, UP_RIGHT, RIGHT_UP, DOWN_UP, UP, RIGHT, RIGHT_LEFT, LEFT_RIGHT, DOWN_RIGHT,
        LEFT_RIGHT, UP_LEFT, DOWN_UP, UP, RIGHT_LEFT, LEFT_UP, RIGHT_LEFT, LEFT_DOWN, RIGHT_DOWN, DOWN_RIGHT, 
        LEFT, RIGHT_UP, RIGHT_LEFT, DOWN_LEFT, LEFT_DOWN, UP_LEFT, DOWN_LEFT, LEFT_RIGHT, DOWN, RIGHT_UP, 
        LEFT_DOWN, LEFT_UP, RIGHT, RIGHT_DOWN, RIGHT_DOWN, DOWN_LEFT, UP_LEFT, LEFT_DOWN, LEFT_UP, RIGHT_LEFT}; //50 long

    private static int[] level11 = { 500, 6, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM, RANDOM };


    public static int[][] levels =  { level1, level2, level3, level4, level5, level6, level7, level8, level9, level10, level11 };

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        level = 0;
        //arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level][0] /1000.0f, levels[level][1]));
    }

    [SerializeField]
    public int distance = 10;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchToLevel(0);
        } 
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToLevel((level+1)%levels.Length);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchToLevel((level + levels.Length - 1) % levels.Length);
        }

        foreach(GameObject arrow in arrows)
        {
            if(arrow != null)
            {
                Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
                if (Environment.TickCount - arrow.GetComponent<ArrowData>().spawnTime > ANIMATION_LENGTH)
                {
                    float dist = Mathf.Sqrt(Mathf.Pow(rb.position.x, 2) + Mathf.Pow(rb.position.y, 2));
                    rb.velocity = (-rb.position).normalized * levels[level][1];
                    int direction = levels[level][arrow.GetComponent<ArrowData>().id];
                    int startDir = direction / 10;
                    int endDir = direction % 10;
                    float percent = (distance - dist) / distance;
                    float transitionPercent = (Mathf.Clamp((percent * 9.0f) - 4.0f, -1.0f, 2.0f) + 1.0f) / 3.0f;
                    float degreesToTurn = ((((startDir - endDir) * 90) + 360) % 360) * transitionPercent;
                    float startX = startDir == DIR_RIGHT ? 10 : startDir == DIR_LEFT ? -10 : 0;
                    float startY = startDir == DIR_UP ? 10 : startDir == DIR_DOWN ? -10 : 0;
                    float unrotatedX = (startX - (startX * percent));
                    float unrotatedY = (startY - (startY * percent));
                    float finalX = (unrotatedX * Mathf.Cos(Mathf.Deg2Rad * degreesToTurn)) - (unrotatedY * Mathf.Sin(Mathf.Deg2Rad * degreesToTurn));
                    float finalY = (unrotatedY * Mathf.Cos(Mathf.Deg2Rad * degreesToTurn)) + (unrotatedX * Mathf.Sin(Mathf.Deg2Rad * degreesToTurn));
                    rb.transform.position = new Vector3(finalX, finalY);
                    rb.transform.rotation = Quaternion.Euler(0, 0, degreesToTurn + startDir * 90);
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
    }

    public void SwitchToLevel(int level)
    {
        if(level == 0)
        {
            PlayerManager.gameStarted = true;
            PlayerManager.instance.health = 10;
        } else if(level == 10)
        {
            for(int i = 2; i < levels[10].Length;i++)
            {
                levels[10][i] = RANDOM;
            }
        } 
        else
        {
            SoundManager.INSTANCE.PlaySound(SoundManager.NEXT_LEVEL, 0.5f);
        }
        if(arrowSpawnerRoutine != null)
        {
            StopCoroutine(arrowSpawnerRoutine);
        }
        DeleteActiveArrows();
        this.level = level;
        if(level <  levels.Length)
        {
            arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level][0] / 1000.0f, levels[level][1]));
            //Debug.Log("Switched to level: " + level);
        }
    }

    public void DeleteActiveArrows()
    {
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();
    }

    void SpawnArrow(int arrowType, int speed, int id)
    {
        //Debug.Log("Sparned New Arrow: Type: "+arrowType+", Speed: "+speed);
        int rotation = 0;
        Vector2Int spawnPos = Vector2Int.zero;
        Color color = Color.blue;

        if(arrowType == WAIT)
        {
            return;
        }

        if (arrowType == RANDOM)
        {
            arrowType = ARROW_TYPES[UnityEngine.Random.Range(0, 18)];
            levels[level][id] = arrowType;
        }

        switch (arrowType/10)
        {
            case DIR_UP:
                {
                    spawnPos.y = -distance;
                    rotation = 0;
                    break;
                }
            case DIR_RIGHT:
                {
                    spawnPos.x = distance;
                    rotation = 90;
                    break;
                }
            case DIR_DOWN:
                {
                    spawnPos.y = distance;
                    rotation = 180;
                    break;
                }
            case DIR_LEFT:
                {
                    spawnPos.x = -distance;
                    rotation = 270;
                    break;
                }
        }
        switch(arrowType%10)
        {
            case DIR_UP:
                {
                    color = new Color(0.358f, 0.884f, 0.960f); //cyan
                    break;
                }
            case DIR_RIGHT:
                {
                    color = new Color(0.476f, 0.358f, 0.960f); //blurple
                    break;
                }
            case DIR_DOWN:
                {
                    color = new Color(0.960f, 0.358f, 0.990f); //pink
                    break;
                }
            case DIR_LEFT:
                {
                    color = new Color(0.960f, 0.629f, 0.358f); //orange
                    break;
                }
        }

        GameObject spawnedArrow = Instantiate(arrowPrefab, new Vector3(spawnPos.x, spawnPos.y), Quaternion.Euler(0, 0, rotation));
        SpriteRenderer spriteRenderer = spawnedArrow.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        spawnedArrow.GetComponent<ArrowData>().id = id;
        spawnedArrow.GetComponent <ArrowData>().spawnTime = Environment.TickCount;
        arrows.Add(spawnedArrow);
    }

    IEnumerator Co_SpawnArrows(float delay, int speed)
    {
        for (int i = 2; i < levels[level].Length; i++)
        {
            SpawnArrow(levels[level][i], speed, i);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(distance/speed + (ANIMATION_LENGTH / 1000.0f));
        SwitchToLevel(level+1);
    }
}
