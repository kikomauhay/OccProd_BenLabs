using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(SoundEmitter), typeof(BoxCollider))]
public class IDScanner : Actor
{
    #region SerializeField

    [SerializeField] private GameObject _invisibleWall;

    [Header("Gate Rotation")]
    [SerializeField] private float _cycleLength;
    [SerializeField] private float _gateDelay;
    [SerializeField] private Transform _leftGate, _rightGate;
    [SerializeField] private Quaternion _leftTargetRot, _rightTargetRot;

    [Header("Sounds")]
    [SerializeField] private Sound _idScanSFX;

    #endregion
    #region Private

    private Renderer _rend;
    private SoundEmitter _sndEmtr;
    private BoxCollider _col;

    private Quaternion _leftStartRot, _rightStartRot;
    private bool _coroutineRunning;

    #endregion
    
    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Delete)) RotateGates();
    }

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_invisibleWall != null, this);

        a_logger.AssertReference(_leftGate != null, this);
        a_logger.AssertReference(_rightGate != null, this);

        a_logger.AssertReference(_idScanSFX != null, this);
    }
    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _sndEmtr = GetComponent<SoundEmitter>();
        _col = GetComponent<BoxCollider>();
    }
    protected override void InitVariables()
    {
        _col.enabled = true;

        _leftStartRot = _leftGate.localRotation;
        _rightStartRot = _rightGate.localRotation;

        _coroutineRunning = false;
    }

    #endregion
    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ID>())
        {
            _rend.material.color = Color.green;
            _sndEmtr.PlaySound(_idScanSFX);
            
            RotateGates();

            if (a_audMgr.OnboardingPlaying)
                a_audMgr.StopOnboarding();

            a_logger.Log("Opened the gates!", TextColor.Lime, a_isDevMode);
        }
        else
        {
            _rend.material.color = Color.red;
            a_logger.Log("Wrong ID!", TextColor.Red, a_isDevMode);
        }
    }

    #endregion
    #region Private

    private void RotateGates()
    {        
        IEnumerator CO_RotateGate(Transform gate, Quaternion startRot, Quaternion endRot)
        {
            float t = 0f;

            while (t < _cycleLength)
            {
                t += Time.deltaTime * 2f;
                gate.localRotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
        }
        IEnumerator CO_GateMovement()
        {
            if (_coroutineRunning)
            {
                a_logger.Log("Coroutine is already running!", TextColor.Red, a_isDevMode);
                yield break;
            }

            _coroutineRunning = true;
            _invisibleWall.SetActive(false);
            StartCoroutine(CO_RotateGate(_leftGate, _leftStartRot, _leftTargetRot));
            StartCoroutine(CO_RotateGate(_rightGate, _rightStartRot, _rightTargetRot));

            yield return new WaitForSeconds(_gateDelay);

            StartCoroutine(CO_RotateGate(_leftGate, _leftTargetRot, _leftStartRot));
            StartCoroutine(CO_RotateGate(_rightGate, _rightTargetRot, _rightStartRot));
            _invisibleWall.SetActive(true);
            _coroutineRunning = false;
        }

        a_logger.Log("Rotating gates!", a_isDevMode);
        StartCoroutine(CO_GateMovement());
        a_logger.Log("Gates rotated!", a_isDevMode);
    }

    #endregion

}
