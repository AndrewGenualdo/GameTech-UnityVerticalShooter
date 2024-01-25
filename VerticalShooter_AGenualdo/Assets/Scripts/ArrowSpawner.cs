using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{

    [SerializeField] private GameObject arrowPrefab;

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

    private int level;

    //levels[0] = ms between arrows
    //levels[1] = arrow speed
    private int[,] levels = new int[,]
        {
            {1000, 10, UP, RIGHT, WAIT, LEFT, DOWN, DOWN, WAIT, RIGHT, LEFT, WAIT, DOWN, UP, RIGHT, LEFT}
        };

    // Start is called before the first frame update
    void Start()
    {
        level = 0;
        arrowSpawnerRoutine = StartCoroutine(Co_SpawnArrows(levels[level, 0]/1000.0f, levels[level, 1]));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StopCoroutine(arrowSpawnerRoutine);
        }
    }

    const int distance = 10;

    void SpawnArrow(int arrowType, int speed)
    {
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
        Destroy(spawnedArrow, 1f/speed*distance*3f);
    }

    IEnumerator Co_SpawnArrows(float delay, int speed)
    {
        while(true)
        {
            for(int i = 2; i < levels.GetLength(1); i++) {
                SpawnArrow(levels[level,i], speed);
                yield return new WaitForSeconds(delay);
            }
            
        }
       
    }
}
