using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Properies

    public Player Player => _player;
    public Atrium Atrium { get; set; }

    public bool IsFading { get; private set; }
    public bool CanPause { get; private set; }
    public bool InGame { get; private set; }
    public bool AtriumActive { get; set; }

    #endregion
    #region SerializeField

    [Header("XR Player"), Tooltip("Needs the XR Origin Component")]
    [SerializeField] private Player _player;

    [Header("Waypoints")]
    [SerializeField] private Transform _gddWaypoint;
    [SerializeField] private Transform _r803Waypoint;

    [Header("Atrium Spanwpoints")]
    [SerializeField] private GameObject _atriumPrefab;
    [SerializeField] private Transform[] _lobbySpawnpoints, _gddSpawnpoints, _barLabSpawnpoints;

    [Header("VFXs")]
    [SerializeField] private GameObject _poofVFXPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _txt_TotalGDDScore;
    [SerializeField] private GameObject _logo;
    [SerializeField] private FadeScreen _fadeScreen;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();
        SpawnAtrium(FloorType.Lobby);
    }
        
    #endregion
    #region Public

    public void DisbaleLogo()
    {
        _logo.SetActive(false);
    }

    public void UI_UpdateGDDHiScore(int score)
    {
        _txt_TotalGDDScore.text = score.ToString();
    }
    public void SpawnAtrium(FloorType floorType)
    {
        void Spawn(Transform t) 
        {
            GameObject atrium = Instantiate(_atriumPrefab, t.position, t.rotation);
            AtriumActive = true;
            Atrium = atrium.GetComponent<Atrium>();
        }

        if (AtriumActive || Random.value < 0.2f) return;

        switch (floorType)
        {
            case FloorType.Lobby:     
                Spawn(_lobbySpawnpoints[Random.Range(0, _lobbySpawnpoints.Length)]);
                break;

            case FloorType.GDD:
                Spawn(_gddSpawnpoints[Random.Range(0, _gddSpawnpoints.Length)]);
                break;

            case FloorType.Bar:
                Spawn(_barLabSpawnpoints[Random.Range(0, _barLabSpawnpoints.Length)]);
                break;     
            
            default: break;
        }
    }
    public void EnterVR()
    {
        IEnumerator CO_Enter(float duration)
        {
            yield return StartCoroutine(CO_FadeIn(duration));

            _player.transform.position = _gddWaypoint.position;
            _player.InsideVRSpace = true;
            a_audMgr.PlaySound("SND_EnterVR");

            yield return StartCoroutine(CO_FadeOut(duration));
        }

        StartCoroutine(CO_Enter(2f));
    }
    public void ExitVR()
    {
        IEnumerator CO_Exit(float duration)
        {
            yield return StartCoroutine(CO_FadeIn(duration));

            _player.transform.position = _r803Waypoint.position;
            _player.InsideVRSpace = false;
            a_audMgr.PlaySound("SND_ExitVR");

            yield return StartCoroutine(CO_FadeOut(duration));
        }

        StartCoroutine(CO_Exit(2f));
    }
    public void Poof(Transform t) => Destroy(Instantiate(_poofVFXPrefab, t.position, t.rotation), 2f);

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < _lobbySpawnpoints.Length; i++)
            {
                Instantiate(_atriumPrefab, 
                            _lobbySpawnpoints[i].position,
                            _lobbySpawnpoints[i].rotation);
            }   
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 0; i < _gddSpawnpoints.Length; i++)
            {
                Instantiate(_atriumPrefab, 
                            _gddSpawnpoints[i].position,
                            _gddSpawnpoints[i].rotation);
            }   
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            for (int i = 0; i < _barLabSpawnpoints.Length; i++)
            {
                Instantiate(_atriumPrefab, 
                            _barLabSpawnpoints[i].position,
                            _barLabSpawnpoints[i].rotation);
            }   
        }
    }

    protected override void AssertReferences()
    {
        Debug.Assert(_player, "Missing _player reference!", this);
     
        Debug.Assert(_r803Waypoint, "Missing _r803Waypoint reference!", this);
        Debug.Assert(_gddWaypoint, "Missing _gddWaypoint reference!", this);

        Debug.Assert(_fadeScreen, "Missing _fadeScreen reference!", this);

        Debug.Assert(_lobbySpawnpoints.Length != 0, "Missing _lobbySpawnpoints elements!", this);
        Debug.Assert(_gddSpawnpoints.Length != 0, "Missing _gddSpawnpoints elements!", this);
        Debug.Assert(_barLabSpawnpoints.Length != 0, "Missing _barLabSpawnpoints elements!", this);
    }
    protected override void InitComponents()
    {
        _fadeScreen = _player.GetComponentInChildren<FadeScreen>();
    }
    protected override void InitVariables()
    {
        a_audMgr = AudioManager.Instance;

        IsFading = true;
        CanPause = true;
        InGame = false;
        AtriumActive = false;
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_FadeIn(float duration)
    {
        _fadeScreen.gameObject.SetActive(true);
        IsFading = true;
        _fadeScreen.FadeOut();

        yield return new WaitForSeconds(duration);
        IsFading = false;
    }
    private IEnumerator CO_FadeOut(float duration)
    {
        IsFading = true;
        _fadeScreen.FadeIn();
        yield return new WaitForSeconds(duration);

        IsFading = false;
    }

    #endregion
}
