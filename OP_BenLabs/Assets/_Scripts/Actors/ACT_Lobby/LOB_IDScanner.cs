using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(SoundEmitter))]
public class LOB_IDScanner : Actor
{
    #region SerializeField

    [SerializeField] private GameObject _invisibleWall;

    [Header("Tweening")]
    [SerializeField] private float _cycleLength;
    [SerializeField] private float _gateDelay;
    [SerializeField] private Transform _leftGate, _rightGate;
    [SerializeField] private Vector3 _leftGateEndPos, _rightGateEndPos;

    [Header("Sounds")]
    [SerializeField] private Sound _idScanSFX;

    #endregion
    #region Private

    private SoundManager _sndMgr;

    private Renderer _rend;
    private SoundEmitter _soundEmitter;

    private Vector3 _leftGateStartPos, _rightGateStartPos;

    #endregion
    #region Unity

    private void OnCollisionEnter(Collision other)
    {
        IEnumerator CO_ToggleInvisibleWall() // prevents the player from skipping exit stage
        {
            _invisibleWall.SetActive(false);
            _logger.Log("Wall disabled!", _isDevMode);
            yield return new WaitForSeconds(10f);

            _invisibleWall.SetActive(true);
            _logger.Log("Wall enabled!", _isDevMode);
        }

        if (other.gameObject.GetComponent<ID>())
        { 
            _rend.material.color = Color.green;
            _soundEmitter.PlaySound(_idScanSFX);
            StartCoroutine(CO_ToggleInvisibleWall());
            OpenGates();

            _logger.Log("Openned the gates!", TextColor.GREEN, _isDevMode);
        }
        else
        {
            _rend.material.color = Color.red;
            _sndMgr.PlaySound("SND_Wrong");

            _logger.Log("Wrong ID!", TextColor.RED, _isDevMode);
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

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenGates();
        }
    }

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _sndMgr = SoundManager.Instance;

        _leftGateStartPos = _leftGate.localPosition;
        _rightGateStartPos = _rightGate.localPosition;
    }

    #endregion

}
