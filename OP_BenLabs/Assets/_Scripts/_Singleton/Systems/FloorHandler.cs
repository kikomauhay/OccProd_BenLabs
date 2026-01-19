using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorHandler : Singleton<FloorHandler>
{
    #region Members

    [Header("Floors"), Tooltip("0 = Lobby, 1 = 8F, 2 = 10F, 3 = Inaccesible Floors")]  
    [SerializeField] private Floor[] _floors; // refer to Floor.cs for the enum
    
    [Header("Components")]
    [SerializeField] private ElevatorDoor _elevDoor;
    [SerializeField] private SoundEmitter _soundEmitter; // no Sound here since it'll come from _elevDoor
    [SerializeField] private Button[] _buttons;

    private BarManager _barMgr;
    private WaitForSeconds _disableDuration;

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
    protected override void AssertComponents()
    {
        a_logger.AssertReference(_floors.Length == System.Enum.GetValues(typeof(FloorType)).Length);
        a_logger.AssertReference(_elevDoor);
        a_logger.AssertReference(_soundEmitter);
        a_logger.AssertReference(_buttons.Length == 17);
    }
    protected override void InitVariables()
    {        
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

            yield return _disableDuration;

            foreach (Button btn in _buttons)
                btn.interactable = true;

            a_logger.Log("Enabled all buttons!", a_isDevMode);
        }

        StartCoroutine(CO_Disable());
    }
}