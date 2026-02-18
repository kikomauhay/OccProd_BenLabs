using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorHandler : Singleton<FloorHandler>
{
    #region Members

    [Header("Floors"), Tooltip("0 = Lobby, 1 = 8F, 2 = 9F, 3 = 10F, 4 = Inaccesible")]
    [SerializeField] private Floor[] _floors; // will change to enums once all floors are made
    
    [Header("Components")]
    [SerializeField] private ElevatorDoor _elevDoor;
    [SerializeField] private SoundEmitter _soundEmitter; // no Sound here since it'll come from _elevDoor
    [SerializeField] private Button[] _buttons;

    private BarManager _barMgr;
    private StampCard _stampCard;
    private WaitForSeconds _disableDuration;

    private const int FLOOR_COUNT = 5;
    private const int BUTTON_COUNT = 49; // 16 buttons * 3 panels + 2 outside - 1 bell button not present

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnterFloor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnterFloor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnterFloor(2);
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_floors.Length == FLOOR_COUNT, this);

        a_logger.AssertReference(_elevDoor, this);
        a_logger.AssertReference(_soundEmitter, this);
        a_logger.AssertReference(_buttons.Length == BUTTON_COUNT, this);
    }
    protected override void InitVariables()
    {
        _barMgr = BarManager.Instance;
        _stampCard = StampCard.Instance;

        _disableDuration = new WaitForSeconds(5f);
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
    #region Members
        
    public void BTN_EnterFloor(int idx) // only accessed from inside
    {  
        if (idx < 0)
        {
            a_logger.Log($"{this} cannot go there!", a_isDevMode);
            return;
        }
        if (!a_gameMgr.CanPause)
        {
            a_logger.Log("You cannot go to that floor at this time!", a_isDevMode);
            return;
        }
        StartCoroutine(CO_MoveToFloor(idx));
    }
    private void EVENT_DisableInteraction()
    {
        IEnumerator CO_Disable()
        {
            foreach (Button btn in _buttons) 
                btn.interactable = false;

            a_logger.Log("Disabled all buttons!", a_isDevMode);

            yield return _disableDuration;

            foreach (Button btn in _buttons)
                btn.interactable = true;

            a_logger.Log("Enabled all buttons!", a_isDevMode);
        }

        StartCoroutine(CO_Disable());
    }
    private IEnumerator CO_MoveToFloor(int idx)
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
            _floors[i].GetComponent<Floor>().HasVisited = true;
            a_logger.Log($"Entering: {(FloorType)idx}", a_isDevMode);

            if (!_floors[i].GetComponent<Floor>().HasVisited)
            {
                a_logger.Log($"{_floors[i].FloorType} has not been visited yet!", a_isDevMode);
                continue;
            }

            _barMgr.gameObject.SetActive((FloorType)idx != FloorType.Cafeteria);
            
            switch ((FloorType)idx)
            {
                case FloorType.Lobby:     _stampCard.Stamp(8); break;
                case FloorType.GDD:       _stampCard.Stamp(2); break;
                case FloorType.Cafeteria: _stampCard.Stamp(4); break;
                case FloorType.Bar:       _stampCard.Stamp(5); break;

                default: break;
            }
        }

        a_gameMgr.SpawnAtrium((FloorType)idx);
        _elevDoor.BTN_Open();

        if (_barMgr.MinigamePlaying)
            _barMgr.INT_DoGameOver(); // in case player immediatly leaves the bar

        yield return _elevDoor.Delay;
        _elevDoor.BTN_Close();
    }

    #endregion
}