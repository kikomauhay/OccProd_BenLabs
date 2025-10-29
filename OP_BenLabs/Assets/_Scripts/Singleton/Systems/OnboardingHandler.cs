using System.Collections;
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


    [Header("VROnboarding Variables")]
    [SerializeField] private GameObject _leftQuestController;
    [SerializeField] private GameObject[] _vrOnbCanvas; //0-TpOnb, 1-GrabOnb
    
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
        //To turn on VR Onb
        StartCoroutine(StartVROnb(20f));
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

    #region IEnumerators

    //To Start the VR Onb for both teleporation and grabbing all in one timer
    private IEnumerator StartVROnb(float timer)
    {
        StartCoroutine(ToggleCanvas(_vrOnbCanvas[0], 15f));
        yield return new WaitForSeconds(timer);
        StartCoroutine(ToggleCanvas(_vrOnbCanvas[1], 15f));
    }

    //To turn off the canvas for the VR Onboarding, but can be used for other stuff
    private IEnumerator ToggleCanvas(GameObject canvas,float timer)
    {
        _leftQuestController.SetActive(true);
        canvas.SetActive(true);
        yield return new WaitForSeconds(timer);
        canvas.SetActive(false);
        _leftQuestController.SetActive(false);
    }

    #endregion
}
