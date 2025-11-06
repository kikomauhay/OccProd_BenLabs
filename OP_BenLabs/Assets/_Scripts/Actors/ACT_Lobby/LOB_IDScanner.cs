using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class LOB_IDScanner : Actor
{
    #region SerializeField

    [SerializeField] private Transform _leftGate, _rightGate;
    [SerializeField] private Vector3 _leftGateEndPos, _rightGateEndPos;
    [SerializeField] private float _cycleLength, _gateDelay;

    #endregion
    #region Private

    private SoundManager _sndMgr;

    private Renderer _rend;

    private Vector3 _leftGateStartPos, _rightGateStartPos;

    #endregion
    #region Unity

    protected override void OnEnable()
    {
        base.OnEnable();
        //add ID grab event
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        //remove ID grab event
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<ID>())
        { 
            _rend.material.color = Color.green;
            _sndMgr.PlaySound("SND_Correct");
        }
        else
        {
            _rend.material.color = Color.red;
            _sndMgr.PlaySound("SND_Wrong");
        }
    }

    #endregion
    #region Private

    private void OpenGates()
    {
        IEnumerator CO_OpenThenClose()
        {
            _leftGate.DOLocalRotate(_leftGateEndPos, _cycleLength);
            _rightGate.DOLocalRotate(_rightGateEndPos, _cycleLength);

            yield return new WaitForSeconds(_gateDelay);

            _leftGate.DOLocalRotate(_leftGateStartPos, _cycleLength);
            _rightGate.DOLocalRotate(_rightGateStartPos, _cycleLength);
        }

        StartCoroutine(CO_OpenThenClose());
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
    }
    protected override void InitVariables()
    {
        _sndMgr = SoundManager.Instance;

        _leftGateStartPos = _leftGate.localPosition;
        _rightGateStartPos = _rightGate.localPosition;
    }

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenGates();
        }
    }

    #endregion

}
