
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    #region Properties

    public WaitForSeconds Delay { get; private set; } = new WaitForSeconds(5f);
    public bool IsClosed { get; private set; }

    #endregion 
    #region SerializeField

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    [Header("Tweening")]
    [SerializeField] private float _cycleLength;
    [SerializeField] private Transform _leftDoor, _rightDoor;

    #endregion
    #region Private 

    private readonly Vector3 _leftDoorEndpos = new Vector3(0f, -0.049999997f, -0.5f);
    private readonly Vector3 _rightDoorEndPos = new Vector3(0f, -0.049999997f, 3.5f);

    private Vector3 _leftDoorStartPos, _rightDoorStartPos;

    #endregion

    #region Unity

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

        StartCoroutine(CO_OpenThenClose());

        if (_isDevMode)
            _logger.Log($"Elevator: {IsClosed}", TextColor.GREEN);
    }
    public void BTN_Close() // accessed from inside
    {     
        _leftDoor.DOLocalMove(_leftDoorStartPos, _cycleLength);
        _rightDoor.DOLocalMove(_rightDoorStartPos, _cycleLength);

        IsClosed = true;

        if (_isDevMode)
            _logger.Log($"Elevator: {IsClosed}", TextColor.GREEN);
    }
        
    #endregion
    #region Helpers
    
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
