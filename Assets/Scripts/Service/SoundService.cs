using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface SoundService {
    bool isBgmPlaying { get; }

    void PlayBGM();
    void StopBGM();
    AudioClip PlayEffect(string name);
    AudioClip PlayEffectAndMuteBGM(string name);
    AudioSource GetEffectLoop(string name);
}
