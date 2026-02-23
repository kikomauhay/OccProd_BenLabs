using UnityEngine;
using System;

public class AudioManager : Singleton<AudioManager> 
{

    #region SerializeField

    public bool OnboardingPlaying => _sources[0].isPlaying;
    public bool MusicPlaying => _sources[2].isPlaying;

    [Header("Audio Sources"), Tooltip("0 = Onb, 1 = SFX, 2 = BGM")]
    [SerializeField] private AudioSource[] _sources;

    [Header("Audio Sources")]
    [SerializeField] private Sound[] _onb;
    [SerializeField] private Sound[] _sfx, _bgm;

    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_sources.Length == 3, this);

        a_logger.AssertReference(_onb.Length != 0, this);
        a_logger.AssertReference(_sfx.Length != 0, this);
        a_logger.AssertReference(_bgm.Length != 0, this);
    }
    protected override void InitVariables()
    {
        foreach (AudioSource src in _sources)
        {
            src.spatialBlend = 0f;
            src.maxDistance = 100f;
        }
    }

    #endregion
    #region Pubilc

    public void PlayOnboarding(string title)
    {
        Sound s = Array.Find(_onb, i => i.name == title);

        if (s == null)
        {
            Debug.LogError($"{title} not found!");
            return;
        }

        // adds the properties of the Sound to the AudioSource
        _sources[0].volume = s.Volume;
        _sources[0].pitch = s.Pitch;
        _sources[0].loop = s.Loop;
        _sources[0].clip = s.Clip;
        _sources[0].spatialBlend = 1f;

        _sources[0].Play();
        _sources[0].pitch = 1f;
    }
    public void PlaySound(string title)
    {
        Sound s = Array.Find(_sfx, i => i.name == title);

        if (s == null)
        {
            Debug.LogError($"{title} not found!");
            return;
        }
        else _sources[1].pitch = s.Pitch;

        _sources[1].volume = s.Volume;
        _sources[1].loop = s.Loop;
        _sources[1].clip = s.Clip;
        _sources[1].spatialBlend = 1f;

        if (s.Loop)
            _sources[1].Play();

        else
            _sources[1].PlayOneShot(s.Clip);

        _sources[1].pitch = 1f;
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
        _sources[2].volume = s.Volume;
        _sources[2].pitch = 1f; // s.Pitch;
        _sources[2].loop = s.Loop;
        _sources[2].clip = s.Clip;
        _sources[2].spatialBlend = 1f;

        _sources[2].Play();
        _sources[2].pitch = 1f;
    }
    
    public void StopMusic() => _sources[2].Stop();
    public void StopSound() => _sources[1].Stop();
    public void StopOnboarding() => _sources[0].Stop();
    public void StopAllSounds()
    {
        foreach (AudioSource src in _sources)
            if (src.isPlaying)
                src.Stop();
    }
    public void PlayCorrect() => PlaySound("SND_Correct");
    public void PlayWrong() => PlaySound("SND_Wrong");
    public void PlayUnsure() => PlaySound("SND_Unsure");
    
    #endregion
}