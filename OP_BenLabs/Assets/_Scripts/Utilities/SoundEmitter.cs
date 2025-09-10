using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    #region SerializeField

    [SerializeField] private Sound _sound;
    [SerializeField] private AudioSource _source;

    [Header("Debugging")]
    [SerializeField] private float _testRadius;

    #endregion

    #region Unity

    private void Start()
    {
        if (!_source)
        {
            Debug.LogError("Missing AudioSource component!");
            return;
        }
        if (!_sound)
        {
            Debug.LogError("Missing Sound component!");
            return;
        }

        InitVariables();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _testRadius);
    }

    #endregion
    #region Helpers

    private void InitVariables()
    {
        _source.clip = _sound.Clip;
        _source.loop = _sound.Loop;
        _source.pitch = _sound.Pitch;
        _source.volume = _sound.Volume;
    }
    public void PlaySound()
    {
        if (_sound.Loop)
            _source.Play();

        else 
            _source.PlayOneShot(_sound.Clip);
    }
    
    #endregion
}
