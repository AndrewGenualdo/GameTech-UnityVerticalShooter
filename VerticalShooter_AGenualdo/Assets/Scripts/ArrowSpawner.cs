using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
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

    //pretty easy introduction level
    private static int[] level1 = { 750, 2, UP, RIGHT, WAIT, LEFT, DOWN, DOWN, WAIT, RIGHT, LEFT, WAIT, DOWN, UP, RIGHT, LEFT};
    //Similar but faster (more challenging)
    private static int[] level2 = { 750, 5, RIGHT, DOWN, UP, LEFT, LEFT, RIGHT, LEFT, UP, DOWN, RIGHT, UP, RIGHT, DOWN};
    //Exposing the player to a lot of arrows while not in much real danger
    private static int[] level3 = { 350, 1, UP, RIGHT, DOWN, LEFT, DOWN, RIGHT, DOWN, LEFT, UP, RIGHT, DOWN, RIGHT, UP, DOWN, RIGHT, UP, LEFT, RIGHT, WAIT};
    //SPEEEED
    private static int[] level4 = { 400, 10, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, LEFT, UP, RIGHT, LEFT, UP, DOWN, DOWN, RIGHT, UP, RIGHT, DOWN, LEFT};
    //introduces the player to "rotating" arrows
    private static int[] level5 = { 2250, 2, UP_LEFT, RIGHT_LEFT, DOWN_LEFT, WAIT, UP_RIGHT, LEFT_RIGHT, DOWN_RIGHT, WAIT, UP_DOWN, DOWN_UP};

    

    public static int[][] levels =  { level1, level2, level3, level4, level5 };

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        level = 0;
        arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level][0] /1000.0f, levels[level][1]));
    }

    [SerializeField]
    public int distance = 10;

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
                rb.transform.rotation = Quaternion.Euler(0, 0, degreesToTurn + startDir*90);
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
            Debug.Log("Switched to level: " + level);
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
        Vector2 velocity = Vector2.zero;
        Color color = Color.blue;

        if(arrowType == WAIT)
        {
            return;
        }

        switch(arrowType/10)
        {
            case DIR_UP:
                {
                    spawnPos.y = -distance;
                    break;
                }
            case DIR_RIGHT:
                {
                    spawnPos.x = -distance;
                    break;
                }
            case DIR_DOWN:
                {
                    spawnPos.y = distance;
                    break;
                }
            case DIR_LEFT:
                {
                    spawnPos.x = distance;
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
