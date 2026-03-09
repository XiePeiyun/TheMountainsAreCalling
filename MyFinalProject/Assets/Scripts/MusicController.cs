using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource music1; // Preparation
    public AudioSource music2; // Movement title1
    public AudioSource music3;
    public AudioSource music4;
    public AudioSource music5;
    public AudioSource music6;

    [Header("BirdKing Reference")]
    public BirdKing birdKing;
    public Title title;

    private string currentTitle = "";


    private void Start()
    {
        if (birdKing != null)
        {
            birdKing.OnStateChanged += HandleStateChanged;
        }

        if (title != null)
            title.OnTitleChanged += HandleTitleChanged;
    }

    private void OnDestroy()
    {
        if (birdKing != null)
            birdKing.OnStateChanged -= HandleStateChanged;

        if (title != null)
            title.OnTitleChanged -= HandleTitleChanged;
    }

    private void HandleTitleChanged(string newTitle)
    {
        currentTitle = newTitle;
    }

    private void HandleStateChanged(BirdKing.GameState state)
    {
        switch (state)
        {
            case BirdKing.GameState.Preparation:
                PlayMusic(music1);
                break;
            case BirdKing.GameState.Movement:
                PlayTitleMusic();
                break;
            case BirdKing.GameState.Ended:
                StopAllMusic();
                break;
        }
    }

    private void PlayTitleMusic()
    {
        switch (currentTitle)
        {
            case "Nobody":
                PlayMusic(music2);
                break;
            case "Whispered Name":
                PlayMusic(music3);
                break;
            case "Rising Figure":
                PlayMusic(music4);
                break;
            case "Local Star":
                PlayMusic(music5);
                break;
            case "Legend":
                PlayMusic(music6);
                break;
            default:
                PlayMusic(music2); // fallback
                break;
        }
    }

    private void PlayMusic(AudioSource source)
    {
        StopAllMusic();
        if (source != null)
        {
            source.Play();
        }
    }

    private void StopAllMusic()
    {
        if (music1) music1.Stop();
        if (music2) music2.Stop();
        if (music3) music3.Stop();
        if (music4) music4.Stop();
        if (music5) music5.Stop();
        if (music6) music6.Stop();
    }

}
