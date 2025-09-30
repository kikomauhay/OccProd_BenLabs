using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Sound")] 
public class Sound : ScriptableObject 
{
    #region Properties
    
    public AudioSource Source { get; set; }
    public AudioClip Clip => _clip;
    public float Volume => _volume;
    public float Pitch => _pitch;
    public bool Loop => _loop;

    #endregion
    #region SerializeField

    [SerializeField] private AudioClip _clip;
    [SerializeField] private bool _loop;

    [Space(10f)]
    [SerializeField, Range(0f, 1f)] private float _volume = 0.8f;
    [SerializeField, Range(1f, 3f)] private float _pitch = 1f;

    #endregion
}