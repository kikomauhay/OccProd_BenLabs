using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorHandler : Singleton<FloorHandler>
{
    #region Members

    [Header("Floors"), Tooltip("0 = Lobby, 1 = 8F, 2 = 9F, 3 = 10F, 4 = Inaccesible Floors")]  
    [SerializeField] private Floor[] _floors; // refer to Floor.cs for the enum

    [Header("Elevator Buttons")]  
    [SerializeField] private Button[] _buttons;
    
    [Header("Components")]
    [SerializeField] private ElevatorDoor _elevDoor;
    [SerializeField] private SoundEmitter _soundEmitter; // no Sound here since it'll come from _elevDoor

    private const int FLOOR_COUNT = 5;
    private const int ELEVATOR_BUTTON_COUNT = 49; // 16 buttons * 3 panels + 2 buttons outside - 1 bell button on the side 
    
    private BarManager _barMgr;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnterFloor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnterFloor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnterFloor(2);
    }
    protected override void InitComponents()
    {
        _barMgr = BarManager.Instance;
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_floors.Length == FLOOR_COUNT, gameObject);
        a_logger.AssertReference(_buttons.Length == ELEVATOR_BUTTON_COUNT, gameObject);
        a_logger.AssertReference(_elevDoor, gameObject);
        a_logger.AssertReference(_soundEmitter, gameObject);
    }

    #endregion
    #region Unity

    protected override void OnEnable()
    {
        ElevatorButton.OnButtonPressed += EVENT_DisableInteraction;
    }
    protected override void OnDisable()
    {
        ElevatorButton.OnButtonPressed -= EVENT_DisableInteraction;
    }

    #endregion

    public void BTN_EmergencyBell()
    {
        // just ring bell
    }

    public void BTN_EnterFloor(int idx) // only accessed from inside
    {  
        IEnumerator CO_MoveToFloor()
        {
            if (!_elevDoor.IsClosed) _elevDoor.BTN_Close();

            if (a_gameMgr.AtriumActive) // in case the player didn't interact with Atrium
            {
                a_gameMgr.AtriumActive = false;
                Destroy(a_gameMgr.Atrium.gameObject);
            }

            _soundEmitter.PlaySound(_elevDoor.ButtonSFX);
            yield return _elevDoor.Delay;

            for (int i = 0; i < _floors.Length; i++)
            {
                _floors[i].gameObject.SetActive(i == idx);
                a_logger.Log($"Entering: {(FloorType)idx}", a_isDevMode);
            }

            a_gameMgr.SpawnAtrium((FloorType)idx);
            _elevDoor.BTN_Open();

            if (_barMgr.MinigamePlaying)
                _barMgr.INT_DoGameOver(); // in case player immediatly leaves the bar

            yield return _elevDoor.Delay;
            _elevDoor.BTN_Close();
        }

        if (idx < 0)
        {
            a_logger.Log($"{this} cannot go there!", a_isDevMode);
            return;
        }
        if (!GameManager.Instance.CanPause)
        {
            a_logger.Log("You cannot go to that floor at this time!", a_isDevMode);
            return;
        }

        StartCoroutine(CO_MoveToFloor());
    }
    private void EVENT_DisableInteraction()
    {
        IEnumerator CO_Disable()
        {
            foreach (Button btn in _buttons) 
                btn.interactable = false;

            a_logger.Log("Disabled all buttons!", a_isDevMode);

            yield return new WaitForSeconds(5f);

            foreach (Button btn in _buttons)
                btn.interactable = true;

            a_logger.Log("Enabled all buttons!", a_isDevMode);
        }

        StartCoroutine(CO_Disable());
    }
}