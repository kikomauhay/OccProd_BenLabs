using System.Collections;
using UnityEngine;

public class FloorHandler : Singleton<FloorHandler>
{
    #region Members

    [Header("Floors"), Tooltip("0 = Lobby, 1 = 8F, 2 = 10F")]
    [SerializeField] private Floor[] _floors; // will change to enums once all floors are made
    
    [Header("Components")]
    [SerializeField] private ElevatorDoor _elevDoor;
    [SerializeField] private SoundEmitter _soundEmitter; // no Sound here since it'll come from _elevDoor

    private GameManager _gameMgr;

    #endregion

    #region Methods

    public void BTN_EnterFloor(int idx) // only accessed from inside
    {  
        IEnumerator CO_MoveToFloor()
        {
            if (!_elevDoor.IsClosed) _elevDoor.BTN_Close();

            if (_gameMgr.AtriumActive) // in case the player didn't interact with Atrium
            {
                _gameMgr.AtriumActive = false;
                Destroy(_gameMgr.Atrium.gameObject);
            }

            _soundEmitter.PlaySound(_elevDoor.ButtonSFX);
            yield return _elevDoor.Delay;

            for (int i = 0; i < _floors.Length; i++)
            {
                _floors[i].gameObject.SetActive(i == idx);
                _logger.Log($"Entering: {(FloorType)idx}", _isDevMode);
            }
            _gameMgr.SpawnAtrium((FloorType)idx);
               
            _elevDoor.BTN_Open();
            yield return _elevDoor.Delay;
            _elevDoor.BTN_Close();
        }

        if (idx < 0)
        {
            _logger.Log($"{this} cannot go there!", _isDevMode);
            return;
        }
        if (!GameManager.Instance.CanPause)
        {
            _logger.Log("You cannot go to that floor at this time!", _isDevMode);
            return;
        }

        StartCoroutine(CO_MoveToFloor());
    }

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnterFloor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnterFloor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnterFloor(2);
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", gameObject);
        Debug.Assert(_elevDoor, "Missing _door reference!", gameObject);
        Debug.Assert(_floors.Length == 3, "Missing _rooms elements!", gameObject);
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;   
    }

    #endregion
}