using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AudioType
{
    Attack,
    Jump,
    Prerun,
    WallKick,
    Roll,
    SlashBullet,
    Die
}

[System.Serializable]
public struct AudioInfo
{
    public AudioType audioType;

    public AudioSource audioSource;
}

public class AudioManager : MonoBehaviour
{
    public AudioInfo[] audioSources;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }


    public void PlaySound(AudioType audioType, bool start)
    {

        for (int i = 0; i < audioSources.Length; i++)
        {
            if(audioType == audioSources[i].audioType)
            {
                if (start)
                {
                    if (!audioSources[i].audioSource.loop)
                    {
                        audioSources[i].audioSource.Stop();
                    }
                    if (!audioSources[i].audioSource.isPlaying)
                    {
                        
                        audioSources[i].audioSource.Play();
                    }
                }
                    
                else
                    audioSources[i].audioSource.Stop();
            }
        }
    }
}
