using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    #region Properties

    public WaitForSeconds DoorDelay { get; private set; } = new WaitForSeconds(5f);
        
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

    #region Unty

    private void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);
        Debug.Assert(_leftDoor || _rightDoor, "<color=red>Missing Door references!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, ColorType.YELLOW);
        
        InitVariables();
    }
    private void Update() => Test();

    #endregion
    #region Public

    public void BTN_OpenElevator() // used when the player will enter the elevator
    {
        IEnumerator CO_OpenElevator()
        {
            BTN_OpenDoor();
            yield return DoorDelay;
            BTN_CloseDoor();
        }

        StartCoroutine(CO_OpenElevator());

        if (_isDevMode)
            _logger.Log("Opened the elevator!", ColorType.YELLOW);
    }
    public void BTN_OpenDoor() // used when the player is inside the elevator
    {
        _leftDoor.DOLocalMove(_leftDoorEndpos, _cycleLength);
        _rightDoor.DOLocalMove(_rightDoorEndPos, _cycleLength);

        if (_isDevMode)
            _logger.Log("Opened the doors!", ColorType.YELLOW);
    }
    public void BTN_CloseDoor() // used when the player is inside the elevator
    {     
        _leftDoor.DOLocalMove(_leftDoorStartPos, _cycleLength);
        _rightDoor.DOLocalMove(_rightDoorStartPos, _cycleLength);

        if (_isDevMode)
            _logger.Log("Closed the doors!", ColorType.YELLOW);
    }
        
    #endregion
    #region Helpers
    
    private void InitVariables()
    {
        _leftDoorStartPos = _leftDoor.localPosition;
        _rightDoorStartPos = _rightDoor.localPosition;
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) BTN_OpenDoor();
        if (Input.GetKeyDown(KeyCode.RightArrow)) BTN_CloseDoor();

        if (Input.GetKeyDown(KeyCode.UpArrow)) BTN_OpenElevator();
    }
        
    #endregion
}
