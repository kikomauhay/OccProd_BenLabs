using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler>
{
    #region Properties

    public bool CanPause { get; private set; }

    #endregion
    #region SerializeField

    [Space(10f), SerializeField] private string _startingScene; // must not be null

    #endregion
    #region Private

    private string _currentScene;
        
    #endregion

    #region Unity

    private void Start()
    {
        if (_startingScene == null)
        {
            Debug.LogWarning("<color=yellow>Missing scene to load!</color>"!, gameObject);
            return;
        }

        InitVariables();
        LoadStartingScene();
    }
    private void Update() => Test();

    #endregion
    #region Public

    public void BTN_LoadToScene(string sceneName)
    {
        if (sceneName == _currentScene)
        {
            if (_isDevMode)
                _logger?.Log("You cannot load the same scene!", gameObject, ColorType.RED);

            return;
        }
        if (!CanPause)
        {
            if (_isDevMode)
                _logger?.Log("You cannot load any scene at this time!", gameObject, ColorType.YELLOW);

            return;
        }

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(_currentScene);
        _currentScene = sceneName;

        GetComponent<SoundEmitter>()?.PlaySound();

        if (_isDevMode)
            _logger?.Log($"Loaded to {_currentScene}!", gameObject, ColorType.YELLOW);
    }

    #endregion
    #region Helpers

    private void InitVariables()
    {
        CanPause = true;
        _currentScene = null;
    } 
    private void LoadStartingScene()
    {
        SceneManager.LoadSceneAsync(_startingScene, LoadSceneMode.Additive);
        _currentScene = _startingScene;

        if (_isDevMode)
            _logger?.Log($"Loaded to {_currentScene}!", gameObject, ColorType.YELLOW);
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) BTN_LoadToScene("SCN_Lobby");
        if (Input.GetKeyDown(KeyCode.Alpha2)) BTN_LoadToScene("SCN_BarLab");
        if (Input.GetKeyDown(KeyCode.Alpha3)) BTN_LoadToScene("SCN_GDDLab");
    }
    

    #endregion
}