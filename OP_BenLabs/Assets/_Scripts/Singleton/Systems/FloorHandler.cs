using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(SoundEmitter))]
public class FloorHandler : Singleton<FloorHandler>
{
    #region Properties

    public ReadOnlyArray<Floor> Floors => _floors;

    #endregion
    #region SerializeField

    [Header("Floors"), Tooltip("0 = Lobby, 1 = 8F, 2 = 10F")]
    [SerializeField] private Floor[] _floors; // will change to enums once all floors are made
    [SerializeField] private ElevatorDoor _elevDoor;

    [Header("Sounds")]
    [SerializeField] private SoundEmitter _soundEmitter;

    #endregion
    #region Private

    private const int FLOOR_COUNT = 3;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_floors.Length == FLOOR_COUNT, "Missing _rooms elements!", gameObject);
        Debug.Assert(_elevDoor, "Missing _door reference!", gameObject);

        base.Start();
    }

    #endregion
    #region Public

    public void BTN_EnterFloor(int idx) // accessed from inside
    {
        IEnumerator CO_MoveToFloor()
        {
            if (!_elevDoor.IsClosed)
                _elevDoor.BTN_Close();

            _soundEmitter.PlaySound(_elevDoor.ButtonSFX);
            yield return _elevDoor.Delay;

            for (int i = 0; i < _floors.Length; i++)
            {
                _floors[i].gameObject.SetActive(i == idx);
                Debug.Log($"Opening: {idx}");
            }
               

            _elevDoor.BTN_Open();
            yield return _elevDoor.Delay;
            _elevDoor.BTN_Close();
        }

        if (idx < 0)
        {
            if (_isDevMode)
                _logger.Log($"{this} cannot go there!", TextColor.RED);

            return;
        }
        if (!GameManager.Instance.CanPause)
        {
            if (_isDevMode)
                _logger.Log("You cannot go to that floor at this time!", TextColor.YELLOW);

            return;
        }

        StartCoroutine(CO_MoveToFloor());
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnterFloor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnterFloor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnterFloor(2);
    }

    #endregion
}