using System.Collections;
using Unity.XR.CoreUtils;
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
    [SerializeField] private GameObject _vrLeftHand;
    [SerializeField] private GameObject _leftQuestController;
    [SerializeField] private GameObject _canvasPickUpID;
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

        StartCoroutine(StartVROnb(5f));
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        //To do when player approaches the ID on the table
        VR_Hands player = other.gameObject.GetComponent<VR_Hands>();

        if(player != null)
        {
            StartCoroutine(ToggleCanvas(_vrOnbCanvas[1], 10f));
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            _canvasPickUpID.SetActive(false);
        }
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

    //To Start the VR Onb for teleporation 
    private IEnumerator StartVROnb(float timer)
    {
        yield return new WaitForSeconds(timer);
        StartCoroutine(ToggleCanvas(_vrOnbCanvas[0], 10f));
    }

    //To turn off the canvas for the VR Onboarding, but can be used for other stuff
    private IEnumerator ToggleCanvas(GameObject canvas,float timer)
    {
        _leftQuestController.SetActive(true);
        canvas.SetActive(true);
        _vrLeftHand.SetActive(false);
        yield return new WaitForSeconds(timer);
        canvas.SetActive(false);
        _leftQuestController.SetActive(false);
        _vrLeftHand.SetActive(true);
    }

    #endregion
}
