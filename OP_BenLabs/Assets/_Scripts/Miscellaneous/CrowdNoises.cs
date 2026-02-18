using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class CrowdNoises : Actor
{
    #region Members

    [SerializeField] private Sound _softCrowdSFX;
    [SerializeField] private Sound[] _loudCrowdSFXs;
        
    private SoundEmitter _sndEmtr;

    #endregion
    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _sndEmtr.StopSound();
            TriggerCrowdSound();
        }
    }
    protected override void InitComponents()
    {
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_softCrowdSFX, this);
        a_logger.AssertReference(_loudCrowdSFXs.Length == 3, this);
    }

    #endregion
    #region Unity

    protected override void Start()
    {
        base.Start();
        TriggerCrowdSound();
    }
    protected override void OnDisable() => _sndEmtr.StopSound();
        
    #endregion
    #region Private

    private void TriggerCrowdSound()
    {
        _sndEmtr.PlaySound(Random.value > 0.5f ? _loudCrowdSFXs[Random.Range(0, _loudCrowdSFXs.Length)] : 
                                                 _softCrowdSFX);
    }
        
    #endregion
}
