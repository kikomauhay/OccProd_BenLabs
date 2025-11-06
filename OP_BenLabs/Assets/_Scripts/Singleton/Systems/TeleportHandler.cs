using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine;

public class TeleportHandler : Singleton<TeleportHandler>
{
    #region Members

    [Header("Teleportation Components")]
    [SerializeField] private GameObject _leftRay;
    [SerializeField] private GameObject _rightRay;
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
    protected override void Start()
    {
        _leftRay.SetActive(false);
        _rightRay.SetActive(false);
    }
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        RightTeleport.action.Disable();
        LeftTeleport.action.Disable();

        RightTeleport.action.performed -= RightRayToggle;
        LeftTeleport.action.performed -= LeftRayToggle;
    }

    #endregion
    #region Public

    public void DeactivateRay(SelectExitEventArgs args)
    {
        if (_leftRay.activeSelf)
            _leftRay.SetActive(false);

        if (_rightRay.activeSelf)
            _rightRay.SetActive(false);
    }
        
    #endregion
    #region Private
    
    private void LeftRayToggle(InputAction.CallbackContext context)
    {
        _leftRay.SetActive(true);
        _logger.Log("Left Ray Enabled!", _isDevMode);
    }
    private void RightRayToggle(InputAction.CallbackContext context)
    {
        _rightRay.SetActive(true);
        _logger.Log("Right Ray Enabled!", _isDevMode);
    }

    #endregion
}