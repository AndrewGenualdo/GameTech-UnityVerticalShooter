using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{



    [SerializeField] public GameObject arrowPrefab;

    public static ArrowSpawner instance;

    const int WAIT = 0;
    const int UP = 1;
    const int RIGHT = 2;
    const int DOWN = 3;
    const int LEFT = 4;
    const int REVERSE_UP = 5;
    const int REVERSE_RIGHT = 6;
    const int REVERSE_DOWN = 7;
    const int REVERSE_LEFT = 8;

    private Coroutine arrowSpawnerRoutine;

    public List<GameObject> arrows = new List<GameObject>();

    public int level;

    //levels[0] = ms between arrows
    //levels[1] = arrow speed
    private static int[] level1 = { 750, 2, UP, RIGHT, WAIT, LEFT, DOWN, DOWN, WAIT, RIGHT, LEFT, WAIT, DOWN, UP, RIGHT, LEFT, WAIT, WAIT, WAIT };
    private static int[] level2 = { 750, 5, RIGHT, DOWN, UP, LEFT, LEFT, RIGHT, LEFT, UP, DOWN, RIGHT, UP, RIGHT, DOWN, WAIT, WAIT, WAIT };
    private static int[] level3 = { 350, 1, UP, RIGHT, DOWN, LEFT, DOWN, RIGHT, DOWN, LEFT, UP, RIGHT, DOWN, RIGHT, UP, DOWN, RIGHT, UP, LEFT, RIGHT, WAIT, WAIT, WAIT };
    private static int[] level4 = { 400, 10, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, LEFT, UP, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, RIGHT, DOWN, LEFT, WAIT, WAIT};

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
        arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level][0] / 1000.0f, levels[level][1]));
        Debug.Log("Switched to Level: " + level);
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

    void SpawnArrow(int arrowType, int speed)
    {
        //Debug.Log("Sparned New Arrow: Type: "+arrowType+", Speed: "+speed);
        int rotation = 0;
        Vector2Int spawnPos = Vector2Int.zero;
        Vector2 velocity = Vector2.zero;
        Color color = Color.white;

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
                    color = Color.red;
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
                    color = Color.green;
                    break;
                }
            case REVERSE_UP:
                {
                    break;
                }
             case REVERSE_RIGHT: 
                { 
                    break;
                }
             case REVERSE_DOWN:
                {
                    break;
                }
             case REVERSE_LEFT:
                {
                    break;
                }
        }
        GameObject spawnedArrow = Instantiate(arrowPrefab, new Vector3(spawnPos.x, spawnPos.y), Quaternion.Euler(0, 0, rotation));
        SpriteRenderer spriteRenderer = spawnedArrow.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        Rigidbody2D rb = spawnedArrow.GetComponent<Rigidbody2D>();
        rb.velocity = velocity * speed;
        arrows.Add(spawnedArrow);
    }

    IEnumerator Co_SpawnArrows(float delay, int speed)
    {
        for (int i = 2; i < levels[level].Length; i++)
        {
            SpawnArrow(levels[level][i], speed);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(distance/speed);
        SwitchToLevel(level+1);
    }
}
