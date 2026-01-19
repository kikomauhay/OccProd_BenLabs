using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class GlassCleaner : Actor
{
    #region Members

    [Header("SFX")]
    [SerializeField] private Sound _waterSplashSFX;
    
    private SoundEmitter _sndEmitter;

    #endregion

    #region Actor

    protected override void AssertComponents()
    {
        // a_logger.AssertReference(_waterSplashSFX, this);
    }
    protected override void InitComponents() => _sndEmitter = GetComponent<SoundEmitter>();

    #endregion
    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Glass>())
        {
            other.GetComponent<Glass>().Washed();
            _sndEmitter.PlaySound(_waterSplashSFX);
        }

        if (other.GetComponent<Shaker>())
        {
            other.GetComponent<Shaker>().Washed();
            //_soundEmitter.PlaySound(_waterSplashSFX);
        }
    }

    #endregion
}
