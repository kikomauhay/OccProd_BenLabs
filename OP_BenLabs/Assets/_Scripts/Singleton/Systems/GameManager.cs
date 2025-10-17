using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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

    private GDDManager _gddMgr;

    private SoundEmitter _soundEmitter;
    private FadeScreen _fadeScreen;
    private WaitForSeconds _fadeDuration;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        IEnumerator CO_DelayedBinding()
        {
            yield return null;

            GDDManager.Instance.OnGameStarted += TeleportPlayer;
            GDDManager.Instance.OnGameFinished += TeleportPlayer;
        }

        StartCoroutine(CO_DelayedBinding());
    }
    protected override void OnDisable()
    {
        GDDManager.Instance.OnGameStarted -= TeleportPlayer;
        GDDManager.Instance.OnGameFinished -= TeleportPlayer;
    }

    #endregion
    #region Private

    private void TeleportPlayer(Vector3 pos, bool isStarting)
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
        _soundEmitter = GetComponent<SoundEmitter>();
        _fadeScreen = _player.GetComponentInChildren<FadeScreen>();
    }
    protected override void InitVariables()
    {
        IsFading = true;
        CanPause = true;

        _fadeDuration = new WaitForSeconds(_fadeScreen.FadeDuration);
    }

    protected override void Test()
    {
        if (!_isDevMode) return;

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
