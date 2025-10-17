
using System.Collections;
using System.Globalization;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class ElevatorDoor : MonoBehaviour
{
    #region Properties

    public WaitForSeconds Delay { get; private set; } = new WaitForSeconds(5f);
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

    [Header("Sounds")]
    [SerializeField] private Sound _buttonSFX;
    [SerializeField] private Sound _bellSFX;

    #endregion
    #region Private 

    private readonly Vector3 _leftDoorEndpos = new Vector3(0f, -0.049999997f, -0.5f);
    private readonly Vector3 _rightDoorEndPos = new Vector3(0f, -0.049999997f, 3.5f);

    private Vector3 _leftDoorStartPos, _rightDoorStartPos;
    private SoundEmitter _soundEmitter;

    #endregion

    #region Unity

    private void Awake() => InitComponents();
    private void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);
        Debug.Assert(_leftDoor || _rightDoor, "<color=red>Missing Door references!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW);

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

        if (_isDevMode)
            _logger.Log($"Elevator closed: {IsClosed}", TextColor.GREEN);
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

        if (_isDevMode)
            _logger.Log($"Elevator closed: {IsClosed}", TextColor.GREEN);
    }

    #endregion
    #region Helpers

    private void InitComponents()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    private void InitVariables()
    {
        _leftDoorStartPos = _leftDoor.localPosition;
        _rightDoorStartPos = _rightDoor.localPosition;

        IsClosed = true;
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) BTN_Open();
        if (Input.GetKeyDown(KeyCode.RightArrow)) BTN_Close();
    }

    #endregion
}
