using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] private bool _enableGizmos;
    [SerializeField] private float _maxDistance;
    private AudioSource _source;

    #endregion
    #region Methods
    
    private void Awake() => _source = GetComponent<AudioSource>();
    private void Start()
    {
        Debug.Assert(_source.maxDistance > 0f, "Max distance is less then 0!", gameObject);
        
        _source.spatialBlend = 1f;
        _source.maxDistance = _maxDistance;
    }
    private void OnDrawGizmosSelected()
    {
        if (!_enableGizmos) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
    }

    public void PlaySound(Sound s) // can play the default sound
    {
        if (s != null)
        {
            _source.clip = s.Clip;
            _source.loop = s.Loop;
            _source.pitch = s.Pitch;
            _source.volume = s.Volume;
        }
        else throw new NullReferenceException("Missing Sound component!");

        if (s.Loop)
            _source.Play();

        else _source.PlayOneShot(_source.clip);
    }

    #endregion
}