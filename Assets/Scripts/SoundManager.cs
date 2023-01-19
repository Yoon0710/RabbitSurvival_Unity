using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name; // song name
    public AudioClip clip; // song
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;

    public string[] PlaySoundName;

    [SerializeField]
    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        PlaySoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 AudioSource 사용 중.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (PlaySoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다");
    }
}