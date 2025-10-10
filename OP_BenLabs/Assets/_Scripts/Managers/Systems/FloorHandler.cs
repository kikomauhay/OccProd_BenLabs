using UnityEngine;

public class FloorHandler : Singleton<FloorHandler>
{
    #region SerializeField

    [Header("Atrium Rooms"), Tooltip("0 = Tutorial, 1 = Lobby, 2 = 8F, 3 = 10F")]
    [SerializeField] private GameObject[] _floors; // disable all the rooms but the first one 

    #endregion
    #region Private

    private const int FLOOR_COUNT = 4;

    #endregion

    #region Unity

    private void Start()
    {
        Debug.Assert(_floors.Length != FLOOR_COUNT, "Missing _rooms elements!", gameObject);
    }
    private void Update() => Test();

    #endregion
    #region Public

    public void BTN_EnableRoom(uint idx)    
    {
        if (!GameManager.Instance.CanPause)
        {
            if (_isDevMode)
                _logger.Log("You cannot load any scene at this time!", ColorType.YELLOW);

            return;
        }            

        // only enables the selected room
        for (int i = 0; i < _floors.Length; i++)
            _floors[i].SetActive(i == idx);
    }

    #endregion
    #region Helpers

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnableRoom(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnableRoom(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnableRoom(2);
    }

    #endregion
}