using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{



    [SerializeField] public GameObject arrowPrefab;

    public static ArrowSpawner instance;

    const int WAIT = -1;
    const int UP = 0;
    const int RIGHT = 11;
    const int DOWN = 22;
    const int LEFT = 33;
    //SHIFTERS (STARTDIRECTION_ENDDIRECTION)
    const int UP_UP = 00;
    const int UP_RIGHT = 01;
    const int UP_DOWN = 02;
    const int UP_LEFT = 03;

    const int RIGHT_UP = 10;
    const int RIGHT_RIGHT = 11;
    const int RIGHT_DOWN = 12;
    const int RIGHT_LEFT = 13;

    const int DOWN_UP = 20;
    const int DOWN_RIGHT = 21;
    const int DOWN_DOWN = 22;
    const int DOWN_LEFT = 23;

    const int LEFT_UP = 30;
    const int LEFT_RIGHT = 31;
    const int LEFT_DOWN = 32;
    const int LEFT_LEFT = 33;


    public Coroutine arrowSpawnerRoutine;

    public List<GameObject> arrows = new List<GameObject>();

    public int level;

    //levels[0] = ms between arrows
    //levels[1] = arrow speed
    private static int[] level1 = { 750, 2, UP, RIGHT, WAIT, LEFT, DOWN, DOWN, WAIT, RIGHT, LEFT, WAIT, DOWN, UP, RIGHT, LEFT, WAIT, WAIT, WAIT };
    private static int[] level2 = { 750, 5, RIGHT, DOWN, UP, LEFT, LEFT, RIGHT, LEFT, UP, DOWN, RIGHT, UP, RIGHT, DOWN, WAIT, WAIT, WAIT };
    private static int[] level3 = { 350, 1, UP, RIGHT, DOWN, LEFT, DOWN, RIGHT, DOWN, LEFT, UP, RIGHT, DOWN, RIGHT, UP, DOWN, RIGHT, UP, LEFT, RIGHT, WAIT, WAIT, WAIT };
    private static int[] level4 = { 400, 10, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, LEFT, UP, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, RIGHT, DOWN, LEFT, WAIT, WAIT};
    private static int[] level5 = { };

    public static int[][] levels =  { level1, level2, level3, level4 };
    /*private int[,] levels = new int[,]
        {
            ,
            
        };*/

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        level = 0;
        arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level][0] /1000.0f, levels[level][1]));
    }

    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
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
                rb.velocity = (-rb.position).normalized * levels[level][1];
                int direction = levels[level][arrow.GetComponent<ArrowData>().id];
                double dist = Mathf.Sqrt(Mathf.Pow(rb.position.x, 2) + Mathf.Pow(rb.position.y, 2));
                int activeDirection;
                double transition = 0.0;
                if (dist > this.distance * 2 / 3)
                {
                    activeDirection = direction / 10;
                } else if (dist > this.distance / 3)
                {
                    double start = this.distance * 2 / 3;
                    double end = this.distance / 3;
                    double length = start - end;
                    double wayThrough = dist - end;
                    transition = wayThrough / length;
                }
                {
                    activeDirection = direction % 10;
                    transition = 1.0;
                }
                
            }
        }
    }

    public void SwitchToLevel(int level)
    {
        if(level == 0)
        {
            PlayerManager.instance.health = 10;
        }
        StopCoroutine(arrowSpawnerRoutine);
        DeleteActiveArrows();
        this.level = level;
        if(level <  levels.Length)
        {
            arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level][0] / 1000.0f, levels[level][1]));
            Debug.Log("Switched to Level: " + level);
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

    [SerializeField]
    public int distance = 10;

    void SpawnArrow(int arrowType, int speed, int id)
    {
        //Debug.Log("Sparned New Arrow: Type: "+arrowType+", Speed: "+speed);
        int rotation = 0;
        Vector2Int spawnPos = Vector2Int.zero;
        Vector2 velocity = Vector2.zero;
        Color color = Color.white;

        float timeUntil = distance / speed;

        switch(arrowType)
        {
            case WAIT:
                {
                    return;
                }
            case UP:
                {
                    spawnPos.y = -distance;
                    rotation = 180;
                    velocity = Vector2.up;
                    color = Color.cyan;
                    break;
                }
            case RIGHT:
                {
                    spawnPos.x = -distance;
                    rotation = 270;
                    velocity = Vector2.right;
                    color = Color.magenta;
                    break;
                }
            case DOWN:
                {
                    spawnPos.y = distance;
                    rotation = 0;
                    velocity = Vector2.down;
                    color = Color.yellow;
                    break;
                }
            case LEFT:
                {
                    spawnPos.x = distance;
                    rotation = 90;
                    velocity = Vector2.left;
                    color = new Color(255, 165, 0); //orange
                    break;
                }
        }
        GameObject spawnedArrow = Instantiate(arrowPrefab, new Vector3(spawnPos.x, spawnPos.y), Quaternion.Euler(0, 0, rotation));
        SpriteRenderer spriteRenderer = spawnedArrow.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        Rigidbody2D rb = spawnedArrow.GetComponent<Rigidbody2D>();
        rb.velocity = velocity * speed;
        spawnedArrow.GetComponent<ArrowData>().id = id;
        arrows.Add(spawnedArrow);
    }

    IEnumerator Co_SpawnArrows(float delay, int speed)
    {
        for (int i = 2; i < levels[level].Length; i++)
        {
            SpawnArrow(levels[level][i], speed, i);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(distance/speed);
        SwitchToLevel(level+1);
    }
}
