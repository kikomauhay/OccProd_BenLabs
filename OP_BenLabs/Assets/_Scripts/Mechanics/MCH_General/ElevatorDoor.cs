
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    #region Properties

    public WaitForSeconds Delay { get; private set; }
    public Sound ButtonSFX => _buttonSFX;
    public bool IsClosed { get; private set; }

    #endregion 
    #region SerializeField

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    [Header("Tweening")]
    [SerializeField] private float _cycleLength;
    [SerializeField] private Transform _leftDoor, _rightDoor;
    [SerializeField] private Vector3 _leftDoorEndpos, _rightDoorEndPos;

    [Header("Sounds")]
    [SerializeField] private Sound _buttonSFX;
    [SerializeField] private Sound _bellSFX;
    [SerializeField] private SoundEmitter _soundEmitter;

    #endregion
    #region Private 

    private Vector3 _leftDoorStartPos, _rightDoorStartPos;

    #endregion

    #region Unity

    private void Start()
    {
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", gameObject);
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);
        Debug.Assert(_leftDoor || _rightDoor, "<color=red>Missing Door references!</color>", gameObject);

        _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW, _isDevMode);

        InitVariables();
    }
    private void Update() => Test();

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

        _logger.Log($"Elevator closed: {IsClosed}", TextColor.GREEN, _isDevMode);
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
        _logger.Log($"Elevator closed: {IsClosed}", TextColor.GREEN, _isDevMode);
    }

    #endregion
    #region Helpers

    private void InitVariables()
    {
        Delay = new WaitForSeconds(5f);
        IsClosed = true;

        _leftDoorStartPos = _leftDoor.localPosition;
        _rightDoorStartPos = _rightDoor.localPosition;
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) BTN_Open();
        if (Input.GetKeyDown(KeyCode.RightArrow)) BTN_Close();
    }

    #endregion
}
