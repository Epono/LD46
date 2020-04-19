using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public enum AudioClips
    {
        DoorsClose,
        DoorsOpen,
        ForkliftMoving,
        GameLose,
        GameWin,
        MenuSelect,
        MenuStart,
        ObjectFallsHeavy, // Multiple
        ObjectFallsLight, // Multiple
        ObjectPickUp,
        //ObjectPickUpAlternative,
        ObjectPutColor,
        ObjectPutWorkbench,
        ObjectRepair,
        //ObjectRepairAlternative,
        ObjectThrow, // Multiple
        PlayerRolling,
        //PlayerRollingLoopOriginal,
        ShipArriving,
        ShipConnectPiece,
        ShipCrashDoor,
        ShipLeaving,
        ShipDriveBy,
        ShipWarning, // Multiple
        ToolBlowtorch,
        ToolGet
    };

    public static SoundManagerScript instance = null;

    public List<AudioSource> musicSources;
    public AudioSource sfxSource;

    //public AudioSource player1Source;
    //public AudioSource player2Source;

    bool muteSFX = false;


    [Header("Game")]
    public AudioClip gameLose;
    public AudioClip gameWin;

    [Header("Menu")]
    public AudioClip menuSelect;
    public AudioClip menuStart;


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioSource source in musicSources)
        {
            source.volume = 0.0f;
        }
        //musicSource.loop = true;
        StartCoroutine(DelayPlayMusic());

        //player1Source.loop = true;
        //player1Source.clip = playerRolling;

        //player2Source.loop = true;
        //player2Source.clip = playerRolling;
    }

    private void Update()
    {
        foreach (AudioSource source in musicSources)
        {
            source.volume = 0.0f;
        }

        float currentHealth = ManagerManagerScript.Instance.goalScript.currentHealth;
        float volume = 0.1f;

        if (currentHealth < 15)
        {
            musicSources[0].volume = volume;
        }
        else if (currentHealth < 30)
        {
            musicSources[1].volume = volume;
        }
        else if (currentHealth < 45)
        {
            musicSources[2].volume = volume;
        }
        else if (currentHealth < 60)
        {
            musicSources[3].volume = volume;
        }
        else if (currentHealth < 75)
        {
            musicSources[4].volume = volume;
        }
        else if (currentHealth < 90)
        {
            musicSources[5].volume = volume;
        }
        else
        {
            musicSources[6].volume = volume;
        }
    }

    //public void StartPlaySounds()
    //{
    //    player1Source.Play();
    //    player2Source.Play();
    //}

    IEnumerator DelayPlayMusic()
    {
        yield return new WaitForSeconds(1.0f);
        PlayMusic();
    }

    public void PlayOneShotSound(AudioClips clipType)
    {
        AudioClip clip = null;
        switch (clipType)
        {

            case AudioClips.GameLose:
                clip = gameLose;
                break;
            case AudioClips.GameWin:
                clip = gameWin;
                break;
            case AudioClips.MenuSelect:
                clip = menuSelect;
                break;
            case AudioClips.MenuStart:
                clip = menuStart;
                break;
                //case AudioClips.ObjectFallsHeavy:
                //    clip = objectFallsHeavy[Random.Range(0, objectFallsHeavy.Count)];
                //    break;
        }
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic()
    {
        //musicSource.Play();
    }


    public void MuteSound()
    {
        //musicSource.mute = !musicSource.mute;
    }

    public void MuteSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

}