// AudioManager.cs - จัดการเสียงเพลงและเอฟเฟกต์เสียง
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string sfxName;
        public AudioClip clip;
    }
    
    [System.Serializable]
    public class MusicTrack
    {
        public string trackName;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
    }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;
    
    [Header("Sound Effects")]
    [SerializeField] private List<SoundEffect> soundEffects;
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    
    [Header("Music")]
    [SerializeField] private List<MusicTrack> musicTracks;
    private Dictionary<string, MusicTrack> bgmDict = new Dictionary<string, MusicTrack>();
    
    [Header("Settings")]
    [SerializeField] private float bgmFadeTime = 2.0f;
    
    private Coroutine bgmFadeCoroutine;
    
    void Awake()
    {
        // สร้าง dictionary สำหรับค้นหาเสียงและเพลง
        foreach (SoundEffect sfx in soundEffects)
        {
            sfxDict[sfx.sfxName] = sfx.clip;
        }
        
        foreach (MusicTrack track in musicTracks)
        {
            bgmDict[track.trackName] = track;
        }
    }
    
    // เล่นเสียงเอฟเฟกต์
    public void PlaySFX(string sfxName)
    {
        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{sfxName}' not found");
        }
    }
    
    // เปลี่ยนเพลง BGM
    public void ChangeBGM(string trackName)
    {
        if (bgmDict.TryGetValue(trackName, out MusicTrack track))
        {
            if (bgmFadeCoroutine != null)
            {
                StopCoroutine(bgmFadeCoroutine);
            }
            
            bgmFadeCoroutine = StartCoroutine(FadeBGM(track.clip, track.volume));
        }
        else
        {
            Debug.LogWarning($"Music track '{trackName}' not found");
        }
    }
    
    // เฟดเพลง BGM
    private IEnumerator FadeBGM(AudioClip newClip, float targetVolume)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;
        
        // เฟดออกเพลงเดิม (ถ้ามี)
        if (bgmSource.isPlaying)
        {
            while (elapsed < bgmFadeTime / 2)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, elapsed / (bgmFadeTime / 2));
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        // เปลี่ยนเพลง
        bgmSource.clip = newClip;
        bgmSource.volume = 0;
        bgmSource.Play();
        
        // เฟดเข้าเพลงใหม่
        elapsed = 0f;
        while (elapsed < bgmFadeTime / 2)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, elapsed / (bgmFadeTime / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        bgmSource.volume = targetVolume;
        bgmFadeCoroutine = null;
    }
    
    // หยุดเพลง BGM
    public void StopBGM()
    {
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }
        
        bgmFadeCoroutine = StartCoroutine(FadeOutBGM());
    }
    
    private IEnumerator FadeOutBGM()
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;
        
        while (elapsed < bgmFadeTime / 2)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, elapsed / (bgmFadeTime / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        bgmSource.Stop();
        bgmSource.volume = startVolume;
        bgmFadeCoroutine = null;
    }
    
    // เล่นเสียงพูด
    public void PlayVoice(AudioClip voiceClip)
    {
        if (voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
        
        if (voiceClip != null)
        {
            voiceSource.clip = voiceClip;
            voiceSource.Play();
        }
    }
    
    // หยุดเสียงพูด
    public void StopVoice()
    {
        voiceSource.Stop();
    }
    
    // ปรับระดับเสียง
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    
    public void SetVoiceVolume(float volume)
    {
        voiceSource.volume = volume;
    }
}