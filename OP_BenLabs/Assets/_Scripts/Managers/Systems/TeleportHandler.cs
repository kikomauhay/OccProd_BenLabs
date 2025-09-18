using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine;

public class TeleportHandler : Singleton<TeleportHandler>
{
    #region Members

    [SerializeField] private GameObject _leftRay, _rightRay;
    public InputActionReference RightTeleport, LeftTeleport;

    #endregion

    #region Unity

    protected override void Awake()
    {
        base.Awake();

        RightTeleport.action.Enable();
        LeftTeleport.action.Enable();

        RightTeleport.action.performed += RightRayToggle;
        LeftTeleport.action.performed += LeftRayToggle;
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        RightTeleport.action.Disable();
        LeftTeleport.action.Disable();

        RightTeleport.action.performed -= RightRayToggle;
        LeftTeleport.action.performed -= LeftRayToggle;
    }

    void Start()
    {
        _leftRay.SetActive(false);
        _rightRay.SetActive(false);
    }

    #endregion

    #region Functions
    private void LeftRayToggle(InputAction.CallbackContext context)
    {
        _leftRay.SetActive(true);
    }

    private void RightRayToggle(InputAction.CallbackContext context)
    {
        _rightRay.SetActive(true);
    }

    public void DeactivateRay(SelectExitEventArgs args)
    {
        if (_leftRay.activeSelf)
        {
            _leftRay.SetActive(false);
        }
        if (_rightRay.activeSelf)
        {
            _rightRay.SetActive(false);
        }
    }
    #endregion
}
