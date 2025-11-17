using System.Collections;
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
    [SerializeField] private Sound _wrongScanSFX;

    #endregion
    #region Private

    private SoundManager _sndMgr;

    private Renderer _rend;
    private SoundEmitter _soundEmitter;
    private BoxCollider _col;

    private Quaternion _leftStartRot, _rightStartRot;

    #endregion
    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ID>())
        {
            _rend.material.color = Color.green;
            _soundEmitter.PlaySound(_idScanSFX);
            RotateGates();

            _logger.Log("Opened the gates!", TextColor.GREEN, _isDevMode);
        }
        else
        {
            _rend.material.color = Color.red;
            _soundEmitter.PlaySound(_wrongScanSFX);
            _logger.Log("Wrong ID!", TextColor.RED, _isDevMode);
        }
    }

    #endregion
    #region Private

    private void RotateGates()
    {        
        IEnumerator CO_GateMovement()
        {
            _invisibleWall.SetActive(false);
            StartCoroutine(CO_RotateGate(_leftGate, _leftStartRot, _leftTargetRot));
            StartCoroutine(CO_RotateGate(_rightGate, _rightStartRot, _rightTargetRot));

            yield return new WaitForSeconds(_gateDelay);

            StartCoroutine(CO_RotateGate(_leftGate, _leftTargetRot, _leftStartRot));
            StartCoroutine(CO_RotateGate(_rightGate, _rightTargetRot, _rightStartRot));
            _invisibleWall.SetActive(true);
        }

        _logger.Log("Rotating gates!", _isDevMode);
        StartCoroutine(CO_GateMovement());
        _logger.Log("Gates rotated!", _isDevMode);
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Delete)) RotateGates();
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_invisibleWall, "Missing _invisibleWall reference!", this);

        Debug.Assert(_leftGate, "Missing _leftGate reference!", this);
        Debug.Assert(_rightGate, "Missing _rightGate reference!", this);

        Debug.Assert(_idScanSFX, "Missing _idScanSFX reference!", this);
        Debug.Assert(_wrongScanSFX, "Missing _wrongScanSFX reference!", this);
    }
    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _soundEmitter = GetComponent<SoundEmitter>();
        _col = GetComponent<BoxCollider>();
    }
    protected override void InitVariables()
    {
        _sndMgr = SoundManager.Instance;

        _col.enabled = true;

        _leftStartRot = _leftGate.localRotation;
        _rightStartRot = _rightGate.localRotation;
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_RotateGate(Transform gate, Quaternion startRot, Quaternion endRot)
    {
        float t = 0f;

        while (t < _cycleLength)
        {
            t += Time.deltaTime * 2f;
            gate.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }
        
    #endregion

}
