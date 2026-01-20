
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ElevatorDoor : Actor
{
    #region Properties

    public WaitForSeconds Delay { get; private set; }
    public Sound ButtonSFX => _buttonSFX;
    public bool IsClosed { get; private set; }

    #endregion 
    #region Inspector

    [Header("Tweening")]
    [SerializeField] private float _cycleLength;
    [SerializeField] private Transform _leftDoor, _rightDoor;
    [SerializeField] private Vector3 _leftDoorEndpos, _rightDoorEndPos;

    [Header("SFX")]
    [SerializeField] private SoundEmitter _soundEmitter;
    [SerializeField] private Sound _buttonSFX, _bellSFX;

    #endregion
    #region Private 

    private Vector3 _leftDoorStartPos, _rightDoorStartPos;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) BTN_Open();
        if (Input.GetKeyDown(KeyCode.RightArrow)) BTN_Close();
    }

    protected override void AssertComponents()
    {
        a_logger.AssertReference(_soundEmitter, this);
        a_logger.AssertReference(a_logger, this);
        a_logger.AssertReference(_leftDoor || _rightDoor, this);
    }
    protected override void InitVariables()
    {
        Delay = new WaitForSeconds(5f);
        IsClosed = true;

        _leftDoorStartPos = _leftDoor.localPosition;
        _rightDoorStartPos = _rightDoor.localPosition;
    }

    #endregion
    #region Public

    public void BTN_OpenFromOutside()
    {
        IEnumerator CO_RandomWaitTime()
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            BTN_Open();
        }

        StartCoroutine(CO_RandomWaitTime());
    }
    public void BTN_Open() // accessed from inside
    {
        IEnumerator CO_OpenThenClose()
        {
            _leftDoor.DOLocalMove(_leftDoorEndpos, _cycleLength);
            _rightDoor.DOLocalMove(_rightDoorEndPos, _cycleLength);
            
            IsClosed = false;
            yield return Delay;

            BTN_Close();
        }

        _soundEmitter.PlaySound(_buttonSFX);
        StartCoroutine(CO_OpenThenClose());

        a_logger.Log($"Elevator closed: {IsClosed}", TextColor.Lime, a_isDevMode);
    }
    public void BTN_Close(bool calledFromInside = false) // accessed from inside
    {
        if (calledFromInside)
            _soundEmitter.PlaySound(_buttonSFX);
        
        _leftDoor.DOLocalMove(_leftDoorStartPos, _cycleLength);
        _rightDoor.DOLocalMove(_rightDoorStartPos, _cycleLength);
        
        if (calledFromInside)
            _soundEmitter.PlaySound(_bellSFX);

        IsClosed = true;

        a_logger.Log($"Elevator closed: {IsClosed}", TextColor.Lime, a_isDevMode);
    }

    #endregion
}
