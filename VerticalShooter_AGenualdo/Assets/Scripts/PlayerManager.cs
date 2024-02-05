using System;
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

    private long lastColorSwap = 0;
    private bool startHurt = false;
    private bool startHeal = false;

    public static bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        lastColorSwap = Environment.TickCount;
    }

    // Update is called once per frame
    void Update()
    {

        if(startHurt)
        {
            SetColor(Color.red, true);
            startHurt = false;
        } else if(startHeal)
        {
            SetColor(Color.green, true);
            startHeal = false;
        } else
        {
            SetColor(Color.white, false);
        }

        int level = ArrowSpawner.instance.level;
        if(gameStarted)
        {
            if (level >= ArrowSpawner.levels.Length)
            {
                healthText.text = "You Win!\nScore: " + health + "\nPress [SPACE] to restart.";
                healthText.color = Color.green;
            }
            else if (health <= 0)
            {
                healthText.text = "Level: " + (ArrowSpawner.instance.level + 1) + "\nHealth: " + health + "\nYou lose!\nPress [SPACE] to restart.";
                healthText.color = Color.red;
                ArrowSpawner.instance.StopCoroutine(ArrowSpawner.instance.arrowSpawnerRoutine);
                ShieldController.instance.gameObject.SetActive(false);
            }
            else
            {
                healthText.text = "Level: " + (ArrowSpawner.instance.level + 1) + "\nHealth: " + health + HealthBar();
                ShieldController.instance.gameObject.SetActive(true);
            }
        }
    }

    private String HealthBar()
    {
        String bar = "";
        for(int i=0;i<10;i++)
        {
            if(ShieldController.instance.score%10 > i)
            {
                bar += "|";
            } else
            {
                bar += ".";
            }
        }

        return "\n[" + bar + "]";
    }

    private void SetColor(Color color, bool resetTimer)
    {
        if(Environment.TickCount - lastColorSwap > 100)
        {
            healthText.color = color;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            if(resetTimer)
            {
                lastColorSwap = Environment.TickCount;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Clone"))
        {
            LoseHealth(collision);
        }
    }

    public void GainHealth()
    {
        health++;
        StartHeal();
        SoundManager.INSTANCE.PlaySound(SoundManager.HEAL);
    }

    public void LoseHealth(Collider2D collision)
    {
        StartHurt();
        health--;
        ShieldController.instance.score = 0;
        Destroy(collision.gameObject);
        SoundManager.INSTANCE.PlaySound(SoundManager.HURT, 0.5f);
    }

    private void StartHeal()
    {
        startHeal = true;
    }

    private void StartHurt()
    {
        startHurt = true;
    }
}
