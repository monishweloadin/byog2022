using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip MUSIC;
    public AudioClip GIVEDAMAGE;
    public AudioClip TAKEDAMAGE;
    public AudioClip KILL;
    public AudioClip DEAD;
    public AudioClip UIBUTTON;

    [Header("Soureces")]
    public AudioSource BGM;
    public AudioSource SFX;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        BGM.clip = MUSIC;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    

}
