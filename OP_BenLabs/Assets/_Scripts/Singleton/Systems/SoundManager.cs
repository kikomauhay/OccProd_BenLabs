using UnityEngine;
using System;

public class SoundManager : Singleton<SoundManager> 
{
    
    #region SerializeField

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _onboardingSource;
    [SerializeField] private AudioSource _soundSource, _musicSource;

    [Header("Audio Sources")]
    [SerializeField] private Sound[] _onb;
    [SerializeField] private Sound[] _sfx, _bgm;

    #endregion

    #region Helpers

    protected override void Test()
    {
        
    }

    #endregion
    #region Audio Playback

    public void StopAllSounds()
    {
        if (_soundSource.isPlaying)
            _soundSource.Stop();

        if (_musicSource.isPlaying)
            _musicSource.Stop();

        if (_onboardingSource.isPlaying)
            _onboardingSource.Stop();
    }
    public void PlaySound(string title)
    {
        Sound s = Array.Find(_sfx, i => i.name == title);

        if (s == null)
        {
            Debug.LogError($"{title} not found!");
            return;
        }
        else _soundSource.pitch = s.Pitch;

        _soundSource.volume = s.Volume;
        _soundSource.loop = s.Loop;
        _soundSource.clip = s.Clip;
        _soundSource.spatialBlend = 1f;

        if (s.Loop)
            _soundSource.Play();

        else
            _soundSource.PlayOneShot(s.Clip);

        _soundSource.pitch = 1f;
    }
    public void PlayMusic(string title) 
    {
        Sound s = Array.Find(_bgm, i => i.name == title);

        if (s == null) 
        {
            Debug.LogError($"{title} not found!");
            return;
        }

        // adds the properties of the Sound to the AudioSource
        _musicSource.volume = s.Volume;
        _musicSource.pitch = 1f; // s.Pitch;
        _musicSource.loop = s.Loop;
        _musicSource.clip = s.Clip;
        _musicSource.spatialBlend = 1f;

        _musicSource.Play();
        _musicSource.pitch = 1f;
    }
    public void PlayOnboarding(string title) 
    {
        Sound s = Array.Find(_onb, i => i.name == title);

        if (s == null) 
        {
            Debug.LogError($"{title} not found!");
            return;
        }

        // adds the properties of the Sound to the AudioSource
        _onboardingSource.volume = s.Volume;
        _onboardingSource.pitch = s.Pitch;      
        _onboardingSource.loop = s.Loop;
        _onboardingSource.clip = s.Clip;
        _onboardingSource.spatialBlend = 1f;

        _onboardingSource.Play();
        _onboardingSource.pitch = 1f;
    }
    
    public void StopMusic() => _musicSource.Stop();
    public void StopSound() => _soundSource.Stop();
    public void StopOnboarding() => _onboardingSource.Stop();

    #endregion
}