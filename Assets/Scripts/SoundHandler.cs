using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public static SoundHandler instance;
    public AudioClip mainAudio;
    public AudioClip reelSpin;
    public AudioClip reelEnd;
    public AudioClip winAudio;
    public AudioClip loseAudio;
    public AudioClip scatterAudio;
    public AudioClip buttonClick;

    public AudioSource audioSource;
    public AudioSource oneShotSource;

    private void Awake()
    {
        if(instance != null) Destroy(instance);

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMainAudio()
    {
        audioSource.clip= mainAudio;
        audioSource.volume = 0.5f;
        audioSource.loop=true;
        audioSource.Play();
    }
    public void PlayReelsSpinAudio()
    {
        audioSource.clip = reelSpin;
        audioSource.volume = 1f;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopReelsSpinAudio()
    {
        audioSource.Stop();
    }
    public void PlayReelsStopAudio()
    {
        oneShotSource.volume = 1f;
        oneShotSource.PlayOneShot(reelEnd);
    }
    public void PlayWinAudio()
    {
        oneShotSource.volume = 1f;
        oneShotSource.PlayOneShot(winAudio);
    }
    public void PlayLoseAudio()
    {
        oneShotSource.volume = 1f;
        oneShotSource.PlayOneShot(loseAudio);
    }
    public void PlayScatterAudio()
    {
        oneShotSource.volume = 1f;
        oneShotSource.PlayOneShot(scatterAudio);
    }
    public void PlayBtnClickAudio()
    {
        oneShotSource.volume = 0.5f;
        oneShotSource.PlayOneShot(buttonClick);
    }
}
