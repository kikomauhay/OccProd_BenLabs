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

    protected override void InitComponents()
    {
        _src = GetComponent<AudioSource>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_src.maxDistance > 0f, this);
    }
    protected override void InitVariables()
    {
        _src.spatialBlend = 1f;
        _src.minDistance = 0.3f;
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
            a_logger.AssertReference(s != null, this);
            a_audMgr.PlayWrong();
            return;
        }

        _src.clip = s.Clip;
        _src.loop = s.Loop;
        _src.pitch = s.Pitch;
        _src.volume = s.Volume;

        if (s.Loop) _src.Play();
        else        _src.PlayOneShot(_src.clip);
    }
    public void PlayRandomSound(Sound[] arr) => PlaySound(arr[Random.Range(0, arr.Length)]);
    public void StopSound() => _src.Stop();

    #endregion
}