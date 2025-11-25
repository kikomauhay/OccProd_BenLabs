using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorHandler : Singleton<FloorHandler>
{
    #region Members

    [Header("Floors"), Tooltip("0 = Lobby, 1 = 8F, 2 = 10F")]
    [SerializeField] private Floor[] _floors; // will change to enums once all floors are made
    
    [Header("Components")]
    [SerializeField] private ElevatorDoor _elevDoor;
    [SerializeField] private SoundEmitter _soundEmitter; // no Sound here since it'll come from _elevDoor
    [SerializeField] private Button[] _buttons;

    private GameManager _gameMgr;
    private BarManager _barMgr;

    private WaitForSeconds _disableDuration;

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

            if (_barMgr.MinigamePlaying)
                _barMgr.INT_DoGameOver(); // in case player immediatly leaves the bar

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
    private void EVENT_DisableInteraction()
    {
        IEnumerator CO_Disable()
        {
            foreach (Button btn in _buttons) 
                btn.interactable = false;

            _logger.Log("Disabled all buttons!", _isDevMode);

            yield return _disableDuration;

            foreach (Button btn in _buttons)
                btn.interactable = true;

            _logger.Log("Enabled all buttons!", _isDevMode);
        }

        StartCoroutine(CO_Disable());
    }

    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnterFloor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnterFloor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnterFloor(2);
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_floors.Length == 3, "Missing _rooms elements!", this);

        Debug.Assert(_elevDoor, "Missing _door reference!", this);
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", this);
        Debug.Assert(_buttons.Length == 17, "Missing _buttons elements!", this);
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _barMgr = BarManager.Instance;
        
        _disableDuration = new WaitForSeconds(5f);
    }

    #endregion
}