using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    public int health = 10;
    [SerializeField]
    private TMPro.TMP_Text healthText;

    public static PlayerManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        int level = ArrowSpawner.instance.level;
        if(level >= ArrowSpawner.levels.Length)
        {
            healthText.text = "You Win!\nScore: "+health+"\nPress 'R' to restart.";
        } else
        {
            healthText.text = "Level: " + (ArrowSpawner.instance.level + 1) + "\nHealth: " + health;
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Clone"))
        {
            health--;
            ShieldController.instance.score = 0;
            Destroy(collision.gameObject);
            if(health == 0)
            {
                ArrowSpawner.instance.SwitchToLevel(0);
            }
        }
    }
}
