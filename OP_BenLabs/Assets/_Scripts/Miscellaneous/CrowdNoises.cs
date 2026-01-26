using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class CrowdNoises : Actor
{
    #region Members

    [SerializeField] private Sound _maleCrowdSFX;
    [SerializeField] private Sound _femaleCrowdSFX;
        
    private SoundEmitter _sndEmitter;

    #endregion
    #region Actor

    protected override void InitComponents()
    {
        _sndEmitter = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_maleCrowdSFX, this);
        a_logger.AssertReference(_femaleCrowdSFX, this);
    }

    #endregion
    #region Unity

    protected override void Start()  
    {
        _sndEmitter.PlaySound(Random.value > 0.5f ? _maleCrowdSFX : 
                                                    _femaleCrowdSFX);
    }

    protected override void OnDisable() => _sndEmitter.StopSound();
        
    #endregion
}
