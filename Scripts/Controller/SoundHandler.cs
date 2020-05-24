using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioSource ClickAudio;
    public AudioSource ClickErrorAudio;
    public AudioSource MatchAudio;
    public AudioSource WinAudio;
    public AudioSource LoseAudio;
    public AudioSource FlipCardAudio;
    public AudioSource FlipCardbackAudio;
    public AudioSource SwashAudio;
    public AudioSource GotScoreAudio;
    public AudioSource BadClickAudio;
    public AudioSource StartGameAudio;


    public void PlayClick() {
        ClickAudio.Play();
    }
    public void PlayError() {
        ClickErrorAudio.Play();
    }
    public void PlayMatch() {
        MatchAudio.Play();
    }
    public void PlayWin() {
        WinAudio.Play();
    }
    public void PlayLose() {
        LoseAudio.Play();
    }
    public void PlayFlipCard() {
        FlipCardAudio.Play();
    }
    public void PlayFlipCardBack() {
        FlipCardbackAudio.Play();
    }
    public void PlaySwash() {
        SwashAudio.Play();
    }
    public void PlayGotScore() {
        GotScoreAudio.Play();
    }
    public void PlayStartGame() {
        StartCoroutine(playStartSoundAfterXSeconds());
    }
    public void PlayBadClick() {
        BadClickAudio.Play();
    }



    IEnumerator playStartSoundAfterXSeconds() {
        yield return new WaitForSeconds(0.25f);
        StartGameAudio.Play();
    }







}
