using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SoundEmitter))]
public class GDDManager : Singleton<GDDManager>, IGameHandler
{
    #region Properties

    public System.Action OnBuffWeapon { get; set; }
    public string PreferredWeapon { get; private set; }

    public Logger Logger => _logger;
    public int WaveIndex => _waveIndex;

    #endregion
    #region SerializeField

    [Header("Spawning & Waves")]
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _spawnArea; // must be Vec3.zero so the child GOs will have normal scale
    [SerializeField] private Transform _testGoal; // will remove one the GDD game is more structured

    [Header("Wave Panel")]
    [SerializeField] private WaveHandler _waveHandler;
    [SerializeField] private GameObject _blockLabelsUI;
    [SerializeField] private List<CodeBlock> _availableBlocksList;

    [Header("Trace Mechanic")]
    [SerializeField] private GameObject _drawingCanvas;
    [SerializeField] private GameObject _swordImage, _hammerImage;
    [SerializeField, Space(10f)] private WeaponSpawner _weaponSpawner;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveCountTXT;
    [SerializeField] private TextMeshProUGUI _playerLivesTXT, _killCountTXT;
    
    [Header("SFX")]
    [SerializeField] private Sound _startGameSFX;
    [SerializeField] private Sound _gameOverSFX, _healSFX, _dmgSFX;

    [Header("VR Variables")]
    [SerializeField] private bool _usingLeftHand;
    [SerializeField] private InputActionReference _xrAButton, _xrXButton;

    #endregion
    #region Private

    private GameManager _gameMgr;
    private OnboardingHandler _onbHandlr;
    private SoundEmitter _soundEmitter;

    private const float GRACE_PERIOD = 15f;
    private const float SPAWN_INTERVAL = 0.5f;

    private readonly float[] _prepTimes = new float[3] { 15f, 10f, 7f };
    private readonly int[] _enemiesToSpawn = new int[3] { 8, 16, 20 };
    private List<List<GhostBlock>> _ghostBlockGridList;

    private List<GameObject> _enemyList;
    private WaveState _waveState;
    private int _killCount, _waveIndex;
    private float _currHP;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        _waveHandler.OnAllBlocksFilled += EVENT_StartWave;
        _xrAButton.action.performed += ToggleMainHand;
        _xrXButton.action.performed += ToggleMainHand;
    }
    protected override void OnDisable()
    {
        _waveHandler.OnAllBlocksFilled -= EVENT_StartWave;
        _xrXButton.action.performed -= ToggleMainHand;
        _xrXButton.action.performed -= ToggleMainHand;
    }
    protected override void Start()
    {
        base.Start();

        _xrAButton.action.Enable();
        _xrXButton.action.Enable();

        UI_UpdateKllCount();
        UI_UpdateWaveIndex();
        UI_UpdatePlayerLife();
    }

    #endregion
    #region Public

    public void INT_BTN_StartGame()
    {
        StartCoroutine(_gameMgr.CO_Enter(FloorType.GDD));
        _logger.Log("Game start!", TextColor.YELLOW, _isDevMode);
    }
    public void INT_BTN_StartTutorial()
    {
        // TP player to the GDD area
        // _soundEmitter.PlaySound(_colliderCheck.WrongSFX);

        _logger.Log("No tutorial mode yet!", TextColor.RED, _isDevMode);
    }

    public void INT_DoGameOver() // only be called once player gets 0 HP
    {
        _soundEmitter.PlaySound(_gameOverSFX);
        StartCoroutine(_gameMgr.CO_Exit(FloorType.GDD));

        _logger.Log("Game over!", TextColor.YELLOW, _isDevMode);
    }

    private void ActivateMarker()
    {
        Player.Instance.LeftHandTools[0].SetActive(_usingLeftHand);
        Player.Instance.RightHandTools[0].SetActive(!_usingLeftHand);
    }
    public void ActivateSword()
    {
        Player.Instance.LeftHandTools[1].SetActive(_usingLeftHand);
        Player.Instance.RightHandTools[1].SetActive(!_usingLeftHand);
    }
    public void ActivateHammer()
    {
        Player.Instance.LeftHandTools[2].SetActive(_usingLeftHand);
        Player.Instance.RightHandTools[2].SetActive(!_usingLeftHand);
    }

    private void EnableMarker(bool active)
    {
        Player.Instance.LeftHandTools[0].SetActive(active && _usingLeftHand);
        Player.Instance.RightHandTools[0].SetActive(active && !_usingLeftHand);
    }


    public void SpawnEnemy()
    {
        Vector3 RandomPositionInBox()
        {
            Vector3 size = _collider.size;
            Vector3 localPosition = new Vector3(Random.Range(-size.x / 2f, size.x / 2f),
                                                Random.Range(-size.y / 2f, size.y / 2f),
                                                Random.Range(-size.z / 2f, size.z / 2f));

            return _collider.transform.TransformPoint(_collider.center + localPosition);
        }
        void SetUpEnemy(Enemy e)
        {
            e.OnDeath += EVENT_CountRemainingEnemies;
            e.OnKilled += EVENT_IncrementKillCount;
            e.OnKilled += EVENT_GainLife;

            e.SetGoal(_isDevMode ? _gameMgr.Player.transform : _testGoal);

            _enemyList.Add(e.gameObject);
        }

        GameObject newEnemy = Instantiate(_ghostBlockGridList[_waveIndex][(int)BlockType.ENEMY].
                                          EnemyPrefab, RandomPositionInBox(), Quaternion.identity);

        SetUpEnemy(newEnemy.GetComponent<Enemy>());
        _logger.Log("Spawned new enemy!", _isDevMode);
    }
   
    public void RemoveBlock(CodeBlock cb) => _availableBlocksList.Remove(cb);
    public void RemoveEnemy(GameObject e) => _enemyList.Remove(e);
    public void UnbindEvents(Enemy e)
    {
        e.OnDeath -= EVENT_CountRemainingEnemies;
        e.OnKilled -= EVENT_IncrementKillCount;
        e.OnKilled -= EVENT_GainLife;
    }
    public void TakeDamage()
    {
        _currHP--;
        _soundEmitter.PlaySound(_dmgSFX);
        UI_UpdatePlayerLife();

        if (_currHP < 1f)
        {
            _currHP = 0f;
            INT_DoGameOver();
        }
    }

    #endregion
    #region Private 

    private void EVENT_CountRemainingEnemies()
    {
        _waveState = WaveState.COUNTING;

        _logger.Log($"Enemies left: {_enemyList.Count}", TextColor.GREEN, _isDevMode);

        if (_enemyList.Count == 0)
        {
            _waveIndex++;
            UI_UpdateWaveIndex();

            if (_waveIndex < 2)
            {
                StartCoroutine(CO_SpawnEnemyWave());
                _logger.Log($"Current Wave: {_waveIndex}", TextColor.GREEN, _isDevMode);
            }
        }
    }
    private void EVENT_IncrementKillCount()
    {
        _killCount++;
        UI_UpdateKllCount();
        _logger.Log($"Kill Count: {_killCount}", this, TextColor.YELLOW, _isDevMode);
    }
    private void EVENT_GainLife()
    {
        if (Random.value < 0.1f)
        {
            _currHP++;
            _soundEmitter.PlaySound(_healSFX);
            _logger.Log("Gained life!", _isDevMode);

            UI_UpdatePlayerLife();
        }
    }
    private void EVENT_StartWave() => StartCoroutine(CO_SpawnEnemyWave());

    private void UI_UpdateWaveIndex()
    {
        IEnumerator CO_ClearWaveTxt()
        {
            _waveCountTXT.gameObject.SetActive(true);
            _waveCountTXT.text = $"Wave {_waveIndex + 1}";

            yield return new WaitForSeconds(GRACE_PERIOD);
            _waveCountTXT.gameObject.SetActive(false);
        }

        StartCoroutine(CO_ClearWaveTxt());
    }
    private void UI_UpdatePlayerLife() => _playerLivesTXT.text = $"Life: {_currHP}";
    private void UI_UpdateKllCount() => _killCountTXT.text = $"Kill Count: {_killCount}";

    private void ToggleMainHand(InputAction.CallbackContext context)
    {
        _usingLeftHand = !_usingLeftHand;

        for (int i = 0; i < 3; i++)
        {
            Player.Instance.LeftHandTools[i].SetActive(_usingLeftHand);
            Player.Instance.RightHandTools[i].SetActive(!_usingLeftHand);
        }

        _logger.Log($"Using left hand: {_usingLeftHand}!", _isDevMode);
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (GameObject e in _enemyList)
                Destroy(e);

            _enemyList.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Space)) SpawnEnemy();
        if (Input.GetKeyDown(KeyCode.Return)) StartCoroutine(CO_SpawnEnemyWave());
    }

    protected override void InitComponents()
    {
        _drawingCanvas.SetActive(false);
        _soundEmitter = GetComponent<SoundEmitter>();

        base.InitComponents();
    }
    protected override void AssertComponents()
    {
        Debug.Assert(_availableBlocksList.Count == 11, "Missing elements in _codeBlockList!", gameObject);

        Debug.Assert(_drawingCanvas, "Missing _drawingCanvas reference!", gameObject);
        Debug.Assert(_weaponSpawner, "Missing _weaponSpawner reference!", gameObject);
        Debug.Assert(_blockLabelsUI, "Missing _blockLabelsUI reference!", gameObject);

        base.AssertComponents();
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _onbHandlr = OnboardingHandler.Instance;

        _enemyList = new List<GameObject>();
        _waveState = WaveState.WAITING;

        _killCount = 0;
        _waveIndex = 0;
        _currHP = 0f;

        _ghostBlockGridList = new List<List<GhostBlock>>()
        {
            new List<GhostBlock> { _waveHandler.GhostBlocks[0], 
                                  _waveHandler.GhostBlocks[1], 
                                  _waveHandler.GhostBlocks[2] },
            
            new List<GhostBlock> { _waveHandler.GhostBlocks[3], 
                                  _waveHandler.GhostBlocks[4], 
                                  _waveHandler.GhostBlocks[5] },
            
            new List<GhostBlock> { _waveHandler.GhostBlocks[6], 
                                  _waveHandler.GhostBlocks[7], 
                                  _waveHandler.GhostBlocks[8] }
        };

        _blockLabelsUI.SetActive(true);
    }

    #endregion
    #region Enumerators    

    private IEnumerator CO_SpawnEnemyWave()
    {        
        Modifier modifier = Modifier.DEFAULT;
        int unitCount = _enemiesToSpawn[_waveIndex];

        void PrepareWave() // prep time for the player to "draw" a weapon
        {
            // play StartTimer.sfx

            _waveHandler.gameObject.SetActive(false);
            _drawingCanvas.SetActive(true);
            // ActivateMarker();
            EnableMarker(true);

            // variable assignment
            PreferredWeapon = _ghostBlockGridList[_waveIndex][(int)BlockType.WEAPON].WeaponContent;
            modifier =        _ghostBlockGridList[_waveIndex][(int)BlockType.MODIFIER].Modifier;

            if      (PreferredWeapon == "Sword")  _swordImage.SetActive(true);
            else if (PreferredWeapon == "Hammer") _hammerImage.SetActive(true);
      
            switch (modifier)
            {
                case Modifier.HEALTH: 
                    _currHP += 5f;
                    _logger.Log("Increased HP!", _isDevMode);
                    break;                
                
                case Modifier.DAMAGE:
                    OnBuffWeapon?.Invoke();
                    _logger.Log("Increased damage!", _isDevMode);
                    break;                
                
                case Modifier.REDUCED_ENEMIES: 
                    unitCount = _enemiesToSpawn[_waveIndex] - 1;
                    _logger.Log("Reduced enemies!", _isDevMode);
                    break;                
                
                case Modifier.DEFAULT: break;                
                default:               break;
            }

            _logger.Log($"{GRACE_PERIOD}s before enemy spawning!", TextColor.YELLOW, _isDevMode);
            _logger.Log($"{_gameMgr.Player} can start drawing!", TextColor.YELLOW, _isDevMode);
            
        }
        IEnumerator CO_DoEnemySpawning()
        {
            // play StopTimer.sfx
            
            if (_availableBlocksList.Count > 0)
            {
                foreach (CodeBlock cb in _availableBlocksList)
                    Destroy(cb.gameObject);

                _availableBlocksList.Clear();
                _logger.Log("Removed remaining blocks!", _isDevMode);
            }

            _waveState = WaveState.SPAWNING;
            _drawingCanvas.SetActive(false);
            _blockLabelsUI.SetActive(false);
            EnableMarker(false);


            for (int i = 0; i < unitCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(1f / SPAWN_INTERVAL);
            }
            _waveState = WaveState.FINISHED;
            _logger.Log("Finished spawning!", TextColor.YELLOW, _isDevMode);
        }

        if (_waveState == WaveState.SPAWNING) // prevents wave spawning overlaps
        {
            _logger.Log("Still spawning enemies!", this, TextColor.RED, _isDevMode);
            yield break;
        }

        PrepareWave();
        yield return new WaitForSeconds(GRACE_PERIOD);
        
        StartCoroutine(CO_DoEnemySpawning());
    }

    #endregion
}

public enum WaveState
{
    SPAWNING = 0,
    WAITING = 1,
    COUNTING = 2,
    FINISHED = 3
};