using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class GameManager : Singleton<GameManager>
{
    #region Properies

    public GameObject Player => _player;
    public bool IsFading { get; private set; }
    public bool CanPause { get; private set; }

    #endregion
    #region SerializeField

    [Header("XR Player"), Tooltip("Needs the XR Origin Component")]
    [SerializeField] private GameObject _player;  

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
    private void Test()
    {
        if (!_isDevMode) return;   
    }

    #endregion
    #region Enumerators

    public IEnumerator CO_EnterLobby()
    {
        _fadeDuration = new WaitForSeconds(2f);

        _fadeScreen.gameObject.SetActive(true);
        IsFading = true;
        _fadeScreen.FadeOut();
        yield return _fadeDuration;

        IsFading = false;

        // tp player to the lobby floor
        if (_isDevMode)
            _logger.Log("Teleported to Lobby!");

        IsFading = true;
        _fadeScreen.FadeIn();
        yield return _fadeDuration;

        IsFading = false;
    }
    #endregion

}
