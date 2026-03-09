using System.Collections;
using UnityEngine;

public class OnboardingHandler : Singleton<OnboardingHandler> 
{
    #region Members

    [Header("VR Components"), Header("0 = Teleportation, 1 - Grabbing")]
    [SerializeField] private GameObject[] _vrOnbCanvas;
    [SerializeField] private GameObject _vrLeftHand;
    [SerializeField] private GameObject _leftQuestController; 
    [SerializeField] private GameObject _canvasPickUpID;

    #endregion

    #region Methods

    protected override void AssertReferences()
    {
        a_logger.AssertCollection(_vrOnbCanvas, this);
        a_logger.AssertReference(_vrLeftHand != null, this);
        a_logger.AssertReference(_leftQuestController != null, this);
        a_logger.AssertReference(_canvasPickUpID != null, this);
    }

    protected override void Start()
    {
        StartCoroutine(CO_StartOnboarding(5f));
        base.Start();
    }
    private void OnTriggerEnter(Collider other)
    {
        // To do when player approaches the ID on the table
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics") ||
           other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            StartCoroutine(CO_ToggleCanvas(_vrOnbCanvas[1], 7f));
            gameObject.GetComponent<BoxCollider>().enabled = false;
            _canvasPickUpID.SetActive(false);
        }
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_StartOnboarding(float timer)
    {
        // starts teleporation onboarding 
        yield return new WaitForSeconds(timer);
        StartCoroutine(CO_ToggleCanvas(_vrOnbCanvas[0], 7f));
    }
    private IEnumerator CO_ToggleCanvas(GameObject canvas, float timer)
    {
        // To turn off the canvas for the VR Onboarding, but can be used for other stuff

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
