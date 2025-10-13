using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class FloorHandler : Singleton<FloorHandler>
{
    #region Properties

    public ReadOnlyArray<Floor> Floors => _floors;

    #endregion
    #region SerializeField

    [Header("Floors"), Tooltip("0 = Tutorial, 1 = Lobby, 2 = 8F, 3 = 10F")]
    [SerializeField] private Floor[] _floors; // disable all the rooms but the first one 
    [SerializeField] private ElevatorDoor _door;

    #endregion
    #region Private

    private const int FLOOR_COUNT = 4;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_floors.Length != FLOOR_COUNT, "Missing _rooms elements!", gameObject);
        Debug.Assert(_door, "Missing _door reference!", gameObject);
        
        base.Start();
    }

    #endregion
    #region Public

    public void BTN_EnableRoom(int idx)
    {
        if (!GameManager.Instance.CanPause)
        {
            if (_isDevMode)
                _logger.Log("You cannot load any scene at this time!", ColorType.YELLOW);

            return;
        }

        _door.BTN_OpenElevator();

        for (int i = 0; i < _floors.Length; i++)
            _floors[i].gameObject.SetActive(i == idx);
            
        _door.BTN_OpenElevator();
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnableRoom(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnableRoom(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnableRoom(2);
    }

    #endregion
}