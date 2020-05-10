using UnityEngine;
using System.Collections;

public enum AudioE{ press, collision, hover, select, unselect, grab, menu }

public class SoundController : MonoBehaviour
{
    //Instance de la classe
    private static SoundController instance = null;

    [SerializeField]
    private AudioClip pressSound;
    [Range(0.0f, 1.0f)]
    public float pressVolume = 0.5f;
    [Space(10)]

    [SerializeField]
    private AudioClip collisionSound;
    [Range(0.0f, 1.0f)]
    public float collisionVolume = 0.5f;
    [Space(10)]

    [SerializeField]
    private AudioClip hoverSound;
    [Range(0.0f, 1.0f)]
    public float hoverVolume = 0.5f;
    [Space(10)]

    [SerializeField]
    private AudioClip selectSound;
    [Range(0.0f, 1.0f)]
    public float selectVolume = 0.5f;
    [Space(10)]

    [SerializeField]
    private AudioClip unselectSound;
    [Range(0.0f, 1.0f)]
    public float unselectVolume = 0.5f;
    [Space(10)]

    [SerializeField]
    private AudioClip grabSound;
    [Range(0.0f, 1.0f)]
    public float grabVolume = 0.5f;
    [Space(10)]

    [SerializeField]
    private AudioClip menuSound;
    [Range(0.0f, 1.0f)]
    public float menuVolume = 0.5f;
    [Space(10)]

    private AudioSource pressAudio;
    private AudioSource hoverAudio;
    private AudioSource collisionAudio;
    private AudioSource selectAudio;
    private AudioSource unselectAudio;
    private AudioSource grabAudio;
    private AudioSource menuAudio;

    //pressSound = (AudioSource)Resources.Load("Assets/Sound/Press.wav");
    //AudioSource hoverAudio = (AudioSource)Resources.Load("Assets/Sound/Hover.wav");
    //selectSound = (AudioSource)Resources.Load("Assets/Sound/Select.wav");
    //unselectSound = (AudioSource)Resources.Load("Assets/Sound/Unselect.wav");
    //grabSound = (AudioSource)Resources.Load("Assets/Sound/Grab.wav");

    /*****************************************************
    * AWAKE
    *
    * INFO:    Initialise l'instance de cette classe ainsi
    *          que les sources audio.
    *           
    *****************************************************/
    void Start()
    {
        // Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        pressAudio = AddAudio(false, false, pressVolume);
        collisionAudio = AddAudio(false, false, collisionVolume);
        hoverAudio = AddAudio(false, false, hoverVolume);
        selectAudio = AddAudio(false, false, selectVolume);
        unselectAudio = AddAudio(false, false, unselectVolume);
        grabAudio = AddAudio(false, false, grabVolume);
        menuAudio = AddAudio(false, false, menuVolume);

        pressAudio.clip = pressSound;
        hoverAudio.clip = hoverSound;
        collisionAudio.clip = collisionSound;
        selectAudio.clip = selectSound;
        unselectAudio.clip = unselectSound;
        grabAudio.clip = grabSound;
        menuAudio.clip = menuSound;
    }

    /*****************************************************
    * ADD AUDIO
    *
    * INFO:    Initialise une nouvelle source audio avec 
    *          ses parametres.
    *           
    *****************************************************/
    public AudioSource AddAudio(bool _loop, bool _playAwake, float _volume)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        //newAudio.clip = clip; 
        newAudio.loop = _loop;
        newAudio.playOnAwake = _playAwake;
        newAudio.volume = _volume;
        return newAudio;
    }

    /*****************************************************
    * PLAY SOUND
    *
    * INFO:    Joue le son select le type recu en parametre.
    *
    *****************************************************/
    public void PlaySound(AudioE _audio)
    {
        switch (_audio)
        {
            case AudioE.press: pressAudio.Play();         break;
            case AudioE.hover: hoverAudio.Play();         break;
            case AudioE.collision: collisionAudio.Play(); break;
            case AudioE.select: selectAudio.Play();       break;
            case AudioE.unselect: unselectAudio.Play();   break;
            case AudioE.grab: grabAudio.Play();           break;
            case AudioE.menu: menuAudio.Play();           break;
            default: break;
        }
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Maintient a jour le volume des sources audio.
    *
    *****************************************************/
    void Update()
    {
        if(pressAudio.volume != pressVolume || hoverAudio.volume != hoverVolume ||
            selectAudio.volume != selectVolume || unselectAudio.volume != unselectVolume ||
            grabAudio.volume != grabVolume || menuAudio.volume != menuVolume || 
            collisionAudio.volume != collisionVolume)
        {
            pressAudio.volume = pressVolume;
            hoverAudio.volume = hoverVolume;
            collisionAudio.volume = collisionVolume;
            selectAudio.volume = selectVolume;
            unselectAudio.volume = unselectVolume;
            grabAudio.volume = grabVolume;
            menuAudio.volume = menuVolume;
        }
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static SoundController GetInstance()
    {
        return instance;
    }
}
