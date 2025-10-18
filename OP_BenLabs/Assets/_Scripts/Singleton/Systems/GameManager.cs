using System.Collections;
using UnityEngine;

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

    private FadeScreen _fadeScreen;
    private WaitForSeconds _fadeDuration;

    #endregion

    #region Public

    public void TeleportPlayer(Vector3 pos, bool minigameStarting)
    {
        StartCoroutine(CO_FadeIn());
        _player.transform.position = pos;
        StartCoroutine(CO_FadeOut());

        // isStarting will be used to enable/disable TP after/before the game
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _fadeScreen = _player.GetComponentInChildren<FadeScreen>();
    }
    protected override void InitVariables()
    {
        IsFading = true;
        CanPause = true;

        _fadeDuration = new WaitForSeconds(_fadeScreen.FadeDuration);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_FadeIn()
    {
        _fadeScreen.gameObject.SetActive(true);
        IsFading = true;
        _fadeScreen.FadeOut();

        yield return _fadeScreen.FadeDuration;
        IsFading = false;
    }
    private IEnumerator CO_FadeOut()
    {
        IsFading = true;
        _fadeScreen.FadeIn();
        yield return _fadeScreen.FadeDuration;

        IsFading = false;
    }

    public IEnumerator CO_EnterLobby()
    {
        StartCoroutine(CO_FadeIn());

        // tp player to the lobby floor
        if (_isDevMode)
            _logger.Log("Teleported to Lobby!");

        yield return StartCoroutine(CO_FadeOut());
    }
    #endregion

}
