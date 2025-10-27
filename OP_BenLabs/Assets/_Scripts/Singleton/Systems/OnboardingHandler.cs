using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class OnboardingHandler : Singleton<OnboardingHandler> 
{
    #region Properties

    #endregion
    #region SerializeField

    [Header("Voice Lines")]
    [SerializeField] private Sound[] _vrOnboardingLines;
    [SerializeField] private Sound[] _barOnboardingLines, _gddOnboardingLines;

    #endregion
    #region Private

    private SoundEmitter _soundEmitter;

    #endregion

    #region Unity

    protected override void Start()
    {
        // Debug.Assert(_vrOnboardingLines, "Missing elements in _vrOnboardingLines!", gameObject);
        // Debug.Assert(_barOnboardingLines, "Missing elements in _barOnboardingLines!", gameObject);
        // Debug.Assert(_gddOnboardingLines, "Missing elements in _gddOnboardingLines!", gameObject);

        base.Start();
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {

    }

    #endregion
}
