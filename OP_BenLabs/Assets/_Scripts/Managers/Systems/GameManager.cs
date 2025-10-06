using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SoundEmitter))]
public class GameManager : Singleton<GameManager>
{
    #region Properies

    public Action OnBarGameStart { get; set; }
    public Action OnGDDGameStart { get; set; }
    public GameObject Player => _player;
    public bool IsFading { get; private set; }
    public bool CanPause { get; private set; }

    #endregion
    #region SerializeField

    [SerializeField] private GameObject _player; // Kiko needs to have to setup this

    [Header("Scene Switching")]
    [SerializeField] private string _startingScene; // must not be null   

    #endregion
    #region Private 

    private SoundEmitter _soundEmitter;
    private FadeScreen _fadeScreen;
    private WaitForSeconds _fadeDuration;
        
    #endregion

    #region Unity

    private void Start()
    {
        InitComponents();
        InitVariables();

        Debug.Assert(_startingScene != string.Empty, "Missing _startingScene reference!", gameObject);

        LoadStartingScene();
    }
    private void Update() => Test();

    #endregion
    #region Helpers

    private void InitComponents() 
    {
        _soundEmitter = GetComponent<SoundEmitter>();
        _fadeScreen = GetComponent<FadeScreen>();
    }
    private void InitVariables() 
    {
        IsFading = true;
        CanPause = true;

        _fadeDuration = new WaitForSeconds(_fadeScreen.FadeDuration);
    }
    private void LoadStartingScene()
    {
        if (_startingScene == string.Empty) return;

        SceneManager.LoadSceneAsync(_startingScene, LoadSceneMode.Additive);
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Space)) OnBarGameStart?.Invoke();        
    }

    #endregion
    #region Enumerators

    public IEnumerator CO_EnterLobby()
    {
        _fadeScreen.gameObject.SetActive(true);
        IsFading = true;
        _fadeScreen.FadeOut();
        yield return _fadeDuration;

        IsFading = false;

        // tp player to the lobby floor

        if (_isDevMode)
            _logger.Log("Teleported to Lobby!");

        /*
        if (sceneName == "MainGameScene")
        {
            SceneManager.UnloadSceneAsync("TrainingScene");
            SceneManager.LoadSceneAsync("MainGameScene", LoadSceneMode.Additive);
        }
        else if (sceneName == "TrainingScene")
        {
            SceneManager.UnloadSceneAsync("MainGameScene");
            SceneManager.LoadSceneAsync("TrainingScene", LoadSceneMode.Additive);
        }
        else
        {
            // SoundManager.Instance.PlaySound("wrong");
            Debug.LogError("Wrong scene named!");
        }
        */

        IsFading = true;
        _fadeScreen.FadeIn();
        yield return _fadeDuration;

        IsFading = false;
    }
    #endregion

}
