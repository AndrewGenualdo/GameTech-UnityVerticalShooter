using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    //All sounds were generated at https://sfxr.me/
    public static int HURT = 0;
    public static int HEAL = 1;
    public static int NEW_SHIELD = 2;
    public static int COLLIDE = 3;
    public static int NEXT_LEVEL = 4;

    public static SoundManager INSTANCE;

    [SerializeField]
    private AudioClip[] sounds;



    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        INSTANCE = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * volume is from 0.0 - 1.0
     */

    public void PlaySound(int sound)
    {
        Sound s = new Sound(sounds[sound]);
        source.PlayOneShot(s.clip, 1.0f);
    }

    public void PlaySound(int sound, float volume)
    {
        Sound s = new Sound(sounds[sound], volume);
        source.PlayOneShot(s.clip, volume);
    }
}

class Sound
{

    public string name;
    public AudioClip clip;
    //public int clipID;
    public float volume;
    //public List<int> sourceIDs = new List<int>();

    public Sound(AudioClip clip)
    {
        name = clip.name;
        this.clip = clip;
        this.volume = 1.0f;
    }

    public Sound(AudioClip clip, float volume)
    {
        name = clip.name;
        this.clip = clip;
        this.volume = volume;
    }
}