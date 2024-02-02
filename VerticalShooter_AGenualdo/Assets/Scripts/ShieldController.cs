using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShieldController : MonoBehaviour
{

    [SerializeField] public int score = 0;

    [SerializeField] public GameObject shieldPrefab;
    public static ShieldController instance = null;
    private bool isClone;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            isClone = false;
        } else
        {
            setClone();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!this.isClone)
        {
            if (Input.GetKeyDown(KeyCode.W)) { SetDirection(ROTATION_UP); }
            else if (Input.GetKeyDown(KeyCode.D)) { SetDirection(ROTATION_RIGHT); }
            else if (Input.GetKeyDown(KeyCode.S)) { SetDirection(ROTATION_DOWN); }
            else if (Input.GetKeyDown(KeyCode.A)) { SetDirection(ROTATION_LEFT); }
        }
    }

    const int ROTATION_UP = 0;
    const int ROTATION_RIGHT = 1;
    const int ROTATION_DOWN = 2;
    const int ROTATION_LEFT = 3;

    [SerializeField]
    public int rotation;

    [SerializeField]
    public float distance = 0.3f;
    void SetDirection(int rotation)
    {
        if(PlayerManager.instance.health > 0)
        {
            this.rotation = rotation;
            switch (rotation)
            {
                case ROTATION_UP:
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                        gameObject.transform.position = new Vector3(0, distance, 0);
                        break;
                    }
                case ROTATION_RIGHT:
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                        gameObject.transform.position = new Vector3(distance, 0, 0);
                        break;
                    }
                case ROTATION_DOWN:
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                        gameObject.transform.position = new Vector3(0, -distance, 0);
                        break;
                    }
                case ROTATION_LEFT:
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                        gameObject.transform.position = new Vector3(-distance, 0, 0);
                        break;
                    }
            }
            if (!isClone)
            {
                GameObject sc = Instantiate(shieldPrefab);
            }
        }
    }

    [SerializeField]
    public float speed;

    public void setClone()
    {
        isClone = true;
        gameObject.transform.position = instance.transform.position;
        gameObject.transform.rotation = instance.transform.rotation;
        SetDirection(instance.rotation);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        switch(instance.rotation)
        {
            case ROTATION_UP:
                {
                    rb.velocity = Vector2.up * speed;
                    break;
                }
            case ROTATION_RIGHT:
                {
                    rb.velocity = Vector2.right * speed;
                    break;
                }
            case ROTATION_DOWN:
                {
                    rb.velocity = Vector2.down * speed;
                    break;
                }
            case ROTATION_LEFT:
                {
                    rb.velocity = Vector2.left * speed;
                    break;
                }


        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.name == "Arrow(Clone)")
        {
            instance.score++;
            if (instance.score % 10 == 0)
            {
                PlayerManager.instance.GainHealth();
            }
            if (isClone)
            {
                Destroy(gameObject);
            }
            Destroy(collision.gameObject);
        }
    }
}
