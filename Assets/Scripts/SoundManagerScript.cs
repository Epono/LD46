using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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

    [SerializeField]
    float baseVolume = 0.1f;

    List<VolumeData> volumeDatas;

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
        StartCoroutine(DelayPlayMusic());

        VolumeData volumeData0 = new VolumeData(-1, -1, -1000, 10, 10, 20);
        VolumeData volumeData1 = new VolumeData(10, 20, 20, 25, 25, 35);
        VolumeData volumeData2 = new VolumeData(25, 35, 35, 40, 40, 50);
        VolumeData volumeData3 = new VolumeData(40, 50, 50, 55, 55, 65);
        VolumeData volumeData4 = new VolumeData(55, 65, 65, 70, 70, 80);
        VolumeData volumeData5 = new VolumeData(70, 80, 80, 85, 85, 95);
        VolumeData volumeData6 = new VolumeData(85, 95, 95, 100, -1, -1);

        volumeDatas = new List<VolumeData> { volumeData0, volumeData1, volumeData2, volumeData3, volumeData4, volumeData5, volumeData6 };
    }

    class VolumeData
    {
        public int minAscendingVolume;
        public int maxAscendingVolume;

        public int minFullVolume;
        public int maxFullVolume;

        public int minDescendingVolume;
        public int maxDescendingVolume;

        public VolumeData(int minAscendingVolume, int maxAscendingVolume,
            int minFullVolume, int maxFullVolume,
            int minDescendingVolume, int maxDescendingVolume)
        {
            this.minAscendingVolume = minAscendingVolume;
            this.maxAscendingVolume = maxAscendingVolume;

            this.minFullVolume = minFullVolume;
            this.maxFullVolume = maxFullVolume;

            this.minDescendingVolume = minDescendingVolume;
            this.maxDescendingVolume = maxDescendingVolume;
        }
    }

    float remap(float value, float minIn, float maxIn, float minOut, float maxOut)
    {
        return minOut + (value - minIn) * (maxOut - minOut) / (maxIn - minIn);
    }

    private void Update()
    {

        float currentHealth = ManagerManagerScript.Instance.goalScript.currentHealth;
        

        for (int i = 0; i < musicSources.Count; i++)
        {
            AudioSource musicSource = musicSources[i];
            VolumeData volumeData = volumeDatas[i];

            if (currentHealth > volumeData.minAscendingVolume && currentHealth < volumeData.maxAscendingVolume)
            {
                musicSource.volume = remap(currentHealth, volumeData.minAscendingVolume, volumeData.maxAscendingVolume, 0.0f, baseVolume);
            }
            else if (currentHealth > volumeData.minFullVolume && currentHealth < volumeData.maxFullVolume)
            {
                musicSource.volume = baseVolume;
            }
            else if (currentHealth > volumeData.minDescendingVolume && currentHealth < volumeData.maxDescendingVolume)
            {
                musicSource.volume = baseVolume - remap(currentHealth, volumeData.minDescendingVolume, volumeData.maxDescendingVolume, 0.0f, baseVolume);
            }
            else
            {
                //Debug.Log(currentHealth);
                musicSource.volume = 0.0f;
            }
        }
    }

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