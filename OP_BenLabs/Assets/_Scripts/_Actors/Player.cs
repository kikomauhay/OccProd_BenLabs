using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(XROrigin))]
public class Player : StaticInstance<Player>
{
    #region Members

    public ReadOnlyArray<GameObject> LeftHandTools => _leftHandTools;
    public ReadOnlyArray<GameObject> RightHandTools => _rightHandTools;

    public bool InsideVRSpace { get; set; }

    [Header("Player Tools"), Tooltip("0 = Sword, 1 = Hammer")]
    [SerializeField] private GameObject[] _leftHandTools;
    [SerializeField] private GameObject[] _rightHandTools;

    #endregion

    #region Helpers

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_leftHandTools.Length == 2, this);
        a_logger.AssertReference(_rightHandTools.Length == 2, this);
        
        a_logger.AssertReference(GetComponent<XROrigin>(), this);

        base.AssertReferences();
    }
    protected override void InitVariables()
    {
        InsideVRSpace = true;
    }

    #endregion
}
