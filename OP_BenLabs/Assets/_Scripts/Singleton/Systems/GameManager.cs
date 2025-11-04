using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Properies

    public GameObject Player => _player;

    public bool IsFading { get; private set; }
    public bool CanPause { get; private set; }
    public bool InGame { get; private set; }

    #endregion
    #region SerializeField

    [Header("XR Player"), Tooltip("Needs the XR Origin Component")]
    [SerializeField] private GameObject _player;

    [Header("Waypoints")]
    [SerializeField] private Transform _lobbyWaypoint;
    [SerializeField] private Transform _gddWaypoint, _r803Waypoint;

    #endregion
    #region Private 

    private FadeScreen _fadeScreen;
    private WaitForSeconds _fadeDuration;

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
            case FloorType.TUTORIAL: break;
            case FloorType.BAR:      break;

            case FloorType.LOBBY:
                pos = _lobbyWaypoint.localPosition;
                break;

            case FloorType.GDD:
                pos = _gddWaypoint.localPosition;
                break;

            default: break;
        }

        Debug.Log($"Teleported Player to {pos}"); 

        StartCoroutine(CO_FadeIn());
        _player.transform.position = pos;

        Debug.Log($"Player POS: {_player.transform.position}");

        if (_isDevMode)
            _logger.Log($"Teleported Player to {type}!");

        yield return CO_FadeOut();
    }
    public IEnumerator CO_Exit(FloorType type)
    {
        StartCoroutine(CO_FadeIn());

        switch (type)
        {
            case FloorType.TUTORIAL: break;
            case FloorType.BAR:      break;

            case FloorType.LOBBY: // final part of the game
                // show the different logos
                break;

            case FloorType.GDD:
                _player.transform.position = _r803Waypoint.localPosition;
                StartCoroutine(CO_FadeOut());
                break;

            default: break;
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
