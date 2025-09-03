using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource bgmSource;
    public AudioClip normalBGM;
    public AudioClip battleBGM;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayNormalBGM()
    {
        if (bgmSource.clip == normalBGM) return;
        PlayBgm(normalBGM);

    }
    public void PlayBattleBGM()
    {
        if (bgmSource.clip == battleBGM) return;
        PlayBgm(battleBGM);
    }

    private void PlayBgm(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
