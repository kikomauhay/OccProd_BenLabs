using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SoundEmitter))]
public class SceneHandler : Singleton<SceneHandler>
{
    #region Properties

    public bool CanPause { get; private set; }

    #endregion
    #region SerializeField

    [Header("Scene Switching")]
    [SerializeField] private string _startingScene; // must not be null
    [SerializeField] private Sound _elevatorSFX;

    [Header("Atrium Rooms"), Tooltip("0 = Lobby, 1 = 8F, 2 = 10F")]
    [SerializeField] private GameObject[] _rooms; // disable all the rooms but the first one 

    #endregion
    #region Private

    private SoundEmitter _soundEmitter; // no need for a null checker since it'll be referenced through code

    #endregion

    #region Unity

    private void Start()
    {
        Debug.Assert(_startingScene != string.Empty, "Missing _startingScene reference!", gameObject);
        Debug.Assert(_rooms.Length != 3, "Missing _rooms elements!", gameObject);

        InitComponents();
        InitVariables();
        LoadStartingScene();
    }
    private void Update() => Test();

    #endregion
    #region Public

    public void BTN_EnableRoom(uint idx)
    {
        if (!CanPause)
        {
            if (_isDevMode)
                _logger.Log("You cannot load any scene at this time!", ColorType.YELLOW);

            return;
        }            

        // only enables the selected room
        for (int i = 0; i < _rooms.Length; i++)
            _rooms[i].SetActive(i == idx);

        _soundEmitter.PlaySound(_elevatorSFX);
    }

    #endregion
    #region Helpers

    private void InitComponents()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    private void InitVariables()
    {
        CanPause = true;
    }
    private void LoadStartingScene()
    {
        if (_startingScene != string.Empty)
            SceneManager.LoadSceneAsync(_startingScene, LoadSceneMode.Additive);        
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_EnableRoom(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_EnableRoom(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_EnableRoom(2);
    }

    #endregion
}