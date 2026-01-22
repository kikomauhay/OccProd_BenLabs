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
        Debug.Assert(_leftHandTools.Length == 2, "Missing elements in _leftHandTools!", this);
        Debug.Assert(_rightHandTools.Length == 2, "Missing elements in _rightHandTools!", this);
        
        Debug.Assert(GetComponent<XROrigin>(), "Missing XROrigin reference!", this);

        base.AssertReferences();
    }
    protected override void InitVariables()
    {
        InsideVRSpace = false;
    }

    #endregion
}
