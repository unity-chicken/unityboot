using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundServiceImpl : SingletonGameObject<SoundServiceImpl>, SoundService {
    private Dictionary<string, AudioClip> effectClipDic = new Dictionary<string, AudioClip>();
    private AudioSource effectSource;
    private AudioSource bgmSource;
    private AudioListener listener;
    private Logger logger = new Logger("SoundService");
    private float effectVolume = 0.5f;
    private float bgmVolume = 0.5f;
    private bool bgmVolumUpdate = false;
    private float bgmVolumnFrom = 0;
    private float bgmVolumnTo = 0;
    private float bgmVolumnProgress = 0;
    private float bgmVolumnDuration = 0.3f;

    public bool isBgmPlaying { get { return bgmSource.isPlaying; } }

    void Awake() {
        DontDestroyOnLoad(gameObject);
        effectSource = gameObject.AddComponent<AudioSource>();
        bgmSource = gameObject.AddComponent<AudioSource>();
        listener = gameObject.AddComponent<AudioListener>();
    }

    void Update() {
        if (bgmVolumUpdate == true) {
            bgmVolumnProgress += Time.deltaTime;
            bgmVolumnProgress = Mathf.Min(bgmVolumnProgress, bgmVolumnDuration);
            float progress = bgmVolumnProgress / bgmVolumnDuration;
            if (bgmSource != null) {
                bgmSource.volume = Mathf.Lerp(bgmVolumnFrom, bgmVolumnTo, progress);
            }

            if (progress >= 1f) {
                bgmVolumnProgress = 0;
                bgmVolumUpdate = false;
            }
        }
    }

    public void PlayBGM() {
        if (bgmSource.clip == null) {
            string path = "Sound/BGM/bgm";
            AudioClip clip = Resources.Load<AudioClip>(path);
            bgmSource.clip = clip;
        }
        
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM() {
        bgmSource.Stop();
    }

    public AudioClip PlayEffect(string name) {
        if (effectClipDic.ContainsKey(name) == true) {
            AudioClip clip = effectClipDic[name];
            effectSource.PlayOneShot(clip, effectVolume);
            return clip;
        }
        else {
            string path = "Sound/Effect/" + name;
            AudioClip clip = Resources.Load<AudioClip>(path);
            effectSource.clip = clip;
            effectSource.PlayOneShot(clip, effectVolume);
            effectClipDic.Add(name, clip);
            return clip;
        }

        return null;
    }

    public AudioClip PlayEffectAndMuteBGM(string name) {
        AudioClip clip = PlayEffect(name);
        if (clip != null) {
            StartCoroutine(EaseMuteBGM(clip.length));
        }

        return clip;
    }

    private IEnumerator EaseMuteBGM(float length) {
        float from = bgmSource.volume;
        bgmVolumnFrom = bgmSource.volume;
        bgmVolumnTo = 0;
        bgmVolumUpdate = true;

        float duration = length - bgmVolumnDuration;
        duration = Mathf.Max(0.1f, duration);
        yield return new WaitForSeconds(duration);

        bgmVolumnFrom = bgmSource.volume;
        bgmVolumnTo = from;
        bgmVolumUpdate = true;
    }

    public AudioSource GetEffectLoop(string name) {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.loop = true;

        if (effectClipDic.ContainsKey(name) == true) {
            source.clip = effectClipDic[name];
        }
        else {
            string path = "Sound/Effect/" + name;
            AudioClip clip = Resources.Load<AudioClip>(path);
            source.clip = clip;
        }

        return source;
    }
}
