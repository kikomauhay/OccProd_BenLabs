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
    private int _currentStep;

    #endregion

    #region Unity

    protected override void Start()
    {
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
        _currentStep = 0;
    }


    #endregion
}
