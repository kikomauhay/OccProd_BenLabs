using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SoundEmitter))]
public class GDDManager : Singleton<GDDManager>, IGameHandler
{
    #region Properties

    public System.Action OnBuffWeapon { get; set; }

    #endregion
    #region SerializeField

    [Header("Spawning & Waves")]
    [SerializeField] private BoxCollider _collider;

    [Header("Wave Panel")]
    [SerializeField] private WaveHandler _waveHandler;
    [SerializeField] private List<CodeBlock> _availableBlocksList;
    [SerializeField] private GameObject _blockLabelsUI, _confirmButton, _cancelButton;

    [Header("VR Variables")]
    [SerializeField] private bool _usingLeftHand;
    [SerializeField] private InputActionReference _xrAButton, _xrXButton;
    [SerializeField] private GameObject[] _leftHandTools, _rightHandTools; // will be referenced in the scene

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveCountTXT;
    [SerializeField] private TextMeshProUGUI _playerLivesTXT, _killCountTXT;

    [Header("SFX")]
    [SerializeField] private Sound _startGameSFX;
    [SerializeField] private Sound _startWaveSFX, _allWavesDoneSFX;
    [SerializeField] private Sound _gameOverSFX, _healSFX, _dmgSFX;

    #endregion   
    #region Private

    private GameManager _gameMgr;
    private OnboardingHandler _onbHandlr;
    private SoundEmitter _soundEmitter;

    private const float SPAWN_INTERVAL = 0.5f;
    private const int MAX_WAVES = 3;

    private readonly float[] _prepTimes = new float[3] { 15f, 10f, 7f };
    private readonly int[] _enemiesToSpawn = new int[3] { 8, 16, 20 };
    private List<List<GhostBlock>> _ghostBlockGridList;

    private List<GameObject> _enemyList; 
    private WaveState _waveState;
    private int _unitCount, _killCount, _waveIndex;
    private float _currHP;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        _xrAButton.action.performed += ToggleMainHand;
        _xrXButton.action.performed += ToggleMainHand;
    }
    protected override void OnDisable()
    {
        _xrXButton.action.performed -= ToggleMainHand;
        _xrXButton.action.performed -= ToggleMainHand;
    }
    protected override void Start()
    {
        base.Start();

        _xrAButton.action.Enable();
        _xrXButton.action.Enable();

        _waveHandler.gameObject.SetActive(true);
        _blockLabelsUI.SetActive(true);

        EnableButtons(false);
    }

    #endregion
    #region Public

    public void INT_BTN_StartGame()
    {
        StartCoroutine(_gameMgr.CO_Enter(FloorType.GDD));
        _logger.Log("Game start!", TextColor.YELLOW, _isDevMode);
    }
    public void INT_DoGameOver() // only be called once player gets 0 HP
    {
        StopAllCoroutines();

        foreach (GameObject e in _enemyList)
            Destroy(e);

        _enemyList.Clear();
        _soundEmitter.PlaySound(_gameOverSFX);
        _logger.Log("Game over!", TextColor.YELLOW, _isDevMode);
        
        StartCoroutine(_gameMgr.CO_Exit(FloorType.GDD));
    }

    public void BTN_Cancel()
    {
        // deleted old CodeBlocks
        // removed refrences from all the GhostBlocks
        // respawns new CodeBlocks
    }
    public void BTN_Confirm()
    {
        StartCoroutine(CO_SpawnEnemyWave());
        _logger.Log("Confirmed wave starting!", _isDevMode);
    }

    /*
    public void EnableSword(bool active)
    {
        _gameMgr.Player.LeftHandTools[0].SetActive(active && _usingLeftHand);
        _gameMgr.Player.RightHandTools[0].SetActive(active && !_usingLeftHand);
    }
    public void EnableHammer(bool active)
    {
        _gameMgr.Player.LeftHandTools[1].SetActive(active && _usingLeftHand);
        _gameMgr.Player.RightHandTools[1].SetActive(active && !_usingLeftHand);
    }
    */

    public void SpawnEnemy()
    {
        Vector3 GetRandomPositionInBox()
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

            e.SetGoal(_gameMgr.Player.transform);

            _enemyList.Add(e.gameObject);
        }

        GameObject newEnemy = Instantiate(_ghostBlockGridList[_waveIndex][(int)BlockType.ENEMY].
                                          EnemyPrefab, GetRandomPositionInBox(), Quaternion.identity);

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

        _logger.Log($"HP: {_currHP}", _isDevMode);
    }
    public void EnableButtons(bool isActive)
    {
        _confirmButton.SetActive(isActive);
        _cancelButton.SetActive(isActive);
    }

    #endregion
    #region Private 

    private void EVENT_CountRemainingEnemies(Enemy e)
    {
        _waveState = WaveState.COUNTING;
        _enemyList.Remove(e.gameObject);
        _logger.Log($"Enemies left: {_enemyList.Count}", TextColor.GREEN, _isDevMode);

        if (_enemyList.Count < 1)
        {
            _waveIndex++;
            _logger.Log($"Wave {_waveIndex + 1}", _isDevMode);

            if (_waveIndex < MAX_WAVES)
            {
                UI_UpdateWaveIndex();
                StopAllCoroutines();
                StartCoroutine(CO_SpawnEnemyWave());

                _logger.Log($"Current Wave: {_waveIndex}", TextColor.GREEN, _isDevMode);
            }
            else
            {
                _soundEmitter.PlaySound(_allWavesDoneSFX);
                _logger.Log("All waves done!", _isDevMode);
            }
        }
    }
    private void EVENT_IncrementKillCount()
    {
        _killCount++;
        _logger.Log($"Kill Count: {_killCount}", this, TextColor.YELLOW, _isDevMode);
        
        UI_UpdateKllCount();
    }
    private void EVENT_GainLife()
    {
        if (Random.value < 0.1f)
        {
            _currHP++;
            UI_UpdatePlayerLife();
        }
    }

    private void UI_UpdateWaveIndex() => _waveCountTXT.text = $"Wave {_waveIndex + 1}";
    private void UI_UpdatePlayerLife() => _playerLivesTXT.text = $"Life: {_currHP}";
    private void UI_UpdateKllCount() => _killCountTXT.text = $"Kill Count: {_killCount}";

    private void ToggleMainHand(InputAction.CallbackContext context) => _usingLeftHand = !_usingLeftHand;

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

    protected override void AssertComponents()
    {
        Debug.Assert(_availableBlocksList.Count == 11, "Missing elements in _codeBlockList!", this);
        Debug.Assert(_blockLabelsUI, "Missing _blockLabelsUI reference!", this);
        Debug.Assert(_confirmButton, "Missing _confirmButton reference!", this);
        Debug.Assert(_cancelButton, "Missing _cancelButton reference!", this);

        Debug.Assert(_leftHandTools.Length == 2, "Missing elements in _leftHandTools!", this);
        Debug.Assert(_rightHandTools.Length == 2, "Missing elements in _rightHandTools!", this);

        Debug.Assert(_waveCountTXT, "Missing _waveCountTXT reference!", this);
        Debug.Assert(_playerLivesTXT, "Missing _playerLivesTXT reference!", this);
        Debug.Assert(_killCountTXT, "Missing _killCountTXT reference!", this);

        Debug.Assert(_startGameSFX, "Missing _startGameSFX reference!", this);
        Debug.Assert(_startWaveSFX, "Missing _startWaveSFX reference!", this);
        Debug.Assert(_allWavesDoneSFX, "Missing _allWavesDoneSFX reference!", this);
        Debug.Assert(_gameOverSFX, "Missing _gameOverSFX reference!", this);
        Debug.Assert(_healSFX, "Missing _healSFX reference!", this);
        Debug.Assert(_dmgSFX, "Missing _dmgSFX reference!", this);
    }
    protected override void InitComponents()
    {        
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _onbHandlr = OnboardingHandler.Instance;

        _enemyList = new List<GameObject>();
        _waveState = WaveState.WAITING;

        _killCount = 0;
        _waveIndex = 0;
        _currHP = 5f;

        _ghostBlockGridList = new List<List<GhostBlock>>()
        {
            new() { _waveHandler.GhostBlocks[0], 
                    _waveHandler.GhostBlocks[1], 
                    _waveHandler.GhostBlocks[2] },
            
            new() { _waveHandler.GhostBlocks[3], 
                    _waveHandler.GhostBlocks[4], 
                    _waveHandler.GhostBlocks[5] },
            
            new() { _waveHandler.GhostBlocks[6], 
                    _waveHandler.GhostBlocks[7], 
                    _waveHandler.GhostBlocks[8] }
        };
    }

    #endregion
    #region Enumerators    

    private IEnumerator CO_SpawnEnemyWave()
    {        
        void PrepareWave() // prep time for the player to "draw" a weapon
        {
            // play StartTimer.sfx
            
            WeaponType preferredWeapon = _ghostBlockGridList[_waveIndex][(int)BlockType.WEAPON].WeaponType;

            if (_waveIndex < 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    _gameMgr.Player.LeftHandTools[i].SetActive(false);
                    _gameMgr.Player.RightHandTools[i].SetActive(false);
                }
            }
            _waveHandler.gameObject.SetActive(false);

            // only enables the correct weapon in hand
            _gameMgr.Player.LeftHandTools[(int)preferredWeapon].SetActive(_usingLeftHand);
            _gameMgr.Player.RightHandTools[(int)preferredWeapon].SetActive(!_usingLeftHand);

            // setup modifiers per wave
            switch (_ghostBlockGridList[_waveIndex][(int)BlockType.MODIFIER].Modifier)
            {
                case Modifier.HEALTH:
                    _currHP++;
                    UI_UpdatePlayerLife();
                    _logger.Log("Increased HP!", _isDevMode);
                    break;

                case Modifier.DAMAGE:
                    OnBuffWeapon?.Invoke();
                    _logger.Log("Increased damage!", _isDevMode);
                    break;

                case Modifier.REDUCED_ENEMIES:
                    _unitCount--;
                    _logger.Log($"Reduced enemy count from {_enemiesToSpawn[_waveIndex]} to {_unitCount}!", _isDevMode);
                    break;

                case Modifier.DEFAULT: break;
                default: break;
            }

            _logger.Log($"{_prepTimes[_waveIndex]}s before enemy spawning!", TextColor.YELLOW, _isDevMode);
            _logger.Log($"{_gameMgr.Player} can start drawing!", TextColor.YELLOW, _isDevMode);
        }
        
        if (_waveState == WaveState.SPAWNING) // prevents wave spawning overlaps
        {
            _logger.Log("Still spawning enemies!", this, TextColor.RED, _isDevMode);
            yield break;
        }

        _unitCount = _enemiesToSpawn[_waveIndex];

        EnableButtons(false);
        PrepareWave();
        yield return new WaitForSeconds(_prepTimes[_waveIndex]);
        
        StartCoroutine(CO_DoEnemySpawning());
    }
    private IEnumerator CO_DoEnemySpawning()
    {
        UI_UpdateKllCount();
        UI_UpdatePlayerLife();
        UI_UpdateWaveIndex();

        _soundEmitter.PlaySound(_startWaveSFX);
        yield return new WaitForSeconds(1f);

        if (_availableBlocksList.Count > 0)
        {
            foreach (CodeBlock cb in _availableBlocksList)
                Destroy(cb.gameObject);

            _availableBlocksList.Clear();
            _logger.Log("Removed remaining blocks!", _isDevMode);
        }

        _waveState = WaveState.SPAWNING;
        _blockLabelsUI.SetActive(false);

        for (int i = 0; i < _unitCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f / SPAWN_INTERVAL);
        }
        _waveState = WaveState.FINISHED;
        _logger.Log("Finished spawning!", TextColor.YELLOW, _isDevMode);
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