using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : Actor
{
    #region Members

    [Header("Debugging")]
    [SerializeField] private bool _enableGizmos;
    [SerializeField] private float _maxDistance;
    
    private AudioSource _src;

    #endregion
    #region Actor

    protected override void AssertComponents()
    {
        // a_logger.Assert(_src.maxDistance > 0f, "Max distance is less then 0!", this);
    }
    protected override void InitComponents()
    {
        _src = GetComponent<AudioSource>();
    }
    protected override void InitVariables()
    {
        _src.spatialBlend = 1f;
        _src.maxDistance = _maxDistance;
    }

    #endregion
    #region Unity

    private void OnDrawGizmosSelected()
    {
        if (!_enableGizmos) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
    }

    #endregion
    #region Public

    public void PlaySound(Sound s) // can play the default sound
    {
        if (s == null)
        {
            a_logger.AssertReference(s, this);
            return;
        }

        _src.clip = s.Clip;
        _src.loop = s.Loop;
        _src.pitch = s.Pitch;
        _src.volume = s.Volume;

        if (s.Loop)
            _src.Play();

        else _src.PlayOneShot(_src.clip);
    }

    #endregion
}