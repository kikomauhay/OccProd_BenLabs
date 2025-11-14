using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Properies

    public Player Player => _player;

    public bool IsFading { get; private set; }
    public bool CanPause { get; private set; }
    public bool InGame { get; private set; }

    #endregion
    #region SerializeField

    [Header("XR Player"), Tooltip("Needs the XR Origin Component")]
    [SerializeField] private Player _player;

    [Header("Waypoints")]
    [SerializeField] private Transform _gddWaypoint;
    [SerializeField] private Transform _r803Waypoint;

    #endregion
    #region Private 

    private SoundManager _sndMgr;

    private FadeScreen _fadeScreen;
    private WaitForSeconds _fadeDuration;

    #endregion

    #region Helpers

    protected override void AssertComponents()
    {
        Debug.Assert(_player, "Missing _player reference!", this);
     
        Debug.Assert(_r803Waypoint, "Missing _r803Waypoint reference!", this);
        Debug.Assert(_gddWaypoint, "Missing _gddWaypoint reference!", this);

        Debug.Assert(_fadeScreen, "Missing _fadeScreen reference!", this);
    }
    protected override void InitComponents()
    {
        _fadeScreen = _player.GetComponentInChildren<FadeScreen>();
    }
    protected override void InitVariables()
    {
        _sndMgr = SoundManager.Instance;

        IsFading = true;
        CanPause = true;
        InGame = false;

        _fadeDuration = new WaitForSeconds(_fadeScreen.FadeDuration);
    }

    #endregion
    #region Enumerators

    public IEnumerator CO_Enter(FloorType type)
    {
        Vector3 pos = Vector3.zero;

        switch (type)
        {
            case FloorType.GDD:
                pos = _gddWaypoint.localPosition;
                _sndMgr.PlaySound("SND_EnterVR");
                break;

            case FloorType.LOBBY: break;
            case FloorType.BAR:   break;
            default:              break;
        }

        _logger.Log($"Teleported Player to {pos}", _isDevMode);

        yield return StartCoroutine(CO_FadeIn());
        _player.transform.position = pos;

        _logger.Log($"Player Position: {_player.transform.position}", _isDevMode);
        _logger.Log($"Teleported Player to {type}!", _isDevMode);

        CO_FadeOut();
    }
    public IEnumerator CO_Exit(FloorType type)
    {
        yield return StartCoroutine(CO_FadeIn());

        switch (type)
        {
            case FloorType.LOBBY: // final part of the game
                // show the different logos
                break;

            case FloorType.GDD:
                _player.transform.position = _r803Waypoint.localPosition;
                _sndMgr.PlaySound("SND_ExitVR");
                StartCoroutine(CO_FadeOut());
                break;

            case FloorType.BAR: break;
            default:            break;
        }

        yield break;
    }

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

    #endregion
}
