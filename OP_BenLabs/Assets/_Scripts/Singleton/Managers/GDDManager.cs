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

    #endregion
    #region SerializeField

    [Header("Spawning & Waves")]
    [SerializeField] private BoxCollider _collider;

    [Header("Wave Panel")]
    [SerializeField] private WaveHandler _waveHandler;
    [SerializeField] private CodeBlock[] _codeBlocks;
    [SerializeField] private GameObject _blockLabelsUI, _confirmButton, _cancelButton;

    [Header("VR Variables")]
    [SerializeField] private bool _usingLeftHand;
    [SerializeField] private InputActionReference _xrAButton, _xrXButton;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveCountTXT;
    [SerializeField] private TextMeshProUGUI _playerLivesTXT, _killCountTXT, _modifierTXT;

    [Header("SFX")]
    [SerializeField] private Sound _startWaveSFX;
    [SerializeField] private Sound _waveDoneSFX ,_allWavesDoneSFX;
    [SerializeField] private Sound _gameOverSFX, _healSFX, _dmgSFX;

    #endregion   
    #region Private

    private GameManager _gameMgr;
    private SoundManager _sndMgr;
    private SoundEmitter _soundEmitter;

    private const float SPAWN_INTERVAL = 0.5f;
    private const int MAX_WAVE_INDEX = 2;

    private readonly float[] _prepTimes = new float[3] { 15f, 10f, 7f };
    private readonly int[] _enemiesToSpawn = new int[3] { 8, 16, 20 };
    private List<List<GhostBlock>> _ghostBlockGridList;

    [SerializeField, Tooltip("Visible for testing")] private List<GameObject> _enemyList; // test
    private Modifier _modifier;
    private int _unitCount, _killCount, _waveIndex;
    private float _currHP;
    private bool _coroutineRunning;

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
        UpdateAllUI();
    }

    #endregion
    #region Public

    public void INT_BTN_StartGame() // enters the VR Space (not start the actual game)
    {
        _gameMgr.EnterVR();

        // StartCoroutine(_gameMgr.CO_Enter(FloorType.GDD));
        _logger.Log("Game start!", TextColor.YELLOW, _isDevMode);
    }
    public void INT_DoGameOver() // only gets called once player gets 0 HP
    {
        _gameMgr.ExitVR();
        _soundEmitter.PlaySound(_gameOverSFX);
        _waveHandler.gameObject.SetActive(true);
        _logger.Log("Game over!", TextColor.YELLOW, _isDevMode);

        if (_coroutineRunning)
        {
            _coroutineRunning = false;
            _logger.Log("CO_Spawning() has stopped!", _isDevMode);

            StopCoroutine(CO_Spawning());
        }

        ResetWeapons();
        ClearAllEnemies();
    }

    public void BTN_Cancel()
    {
        ResetGame();
        _logger.Log("Reseting current grid!", _isDevMode);
    }
    public void BTN_Confirm()
    {
        StartWave();
        _logger.Log("Confirmed wave starting!", _isDevMode);
    }

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
            e.OnDeath += EVENT_CountEnemies;
            e.OnKilled += EVENT_AddToKills;
            e.OnKilled += EVENT_GainLife;

            e.SetGoal(_gameMgr.Player.transform);

            _enemyList.Add(e.gameObject);
            _logger.Log("Spawned new enemy!", _isDevMode);
        }

        GameObject newEnemy = Instantiate(_ghostBlockGridList[_waveIndex][(int)BlockType.ENEMY].
                                          EnemyPrefab, GetRandomPositionInBox(), Quaternion.identity);

        SetUpEnemy(newEnemy.GetComponent<Enemy>());
    }
   
    public void RemoveEnemy(GameObject e) => _enemyList.Remove(e);
    public void UnbindEvents(Enemy e)
    {
        e.OnDeath -= EVENT_CountEnemies;
        e.OnKilled -= EVENT_AddToKills;
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

        // _logger.Log($"HP: {_currHP}", _isDevMode);
        _logger.Log($"Enemies left: {_enemyList.Count}", _isDevMode);
    }
    public void EnableButtons(bool isActive)
    {
        _confirmButton.SetActive(isActive);
        _cancelButton.SetActive(isActive);
    }

    #endregion
    #region Private 

    private void EVENT_CountEnemies()
    {
        _logger.Log($"Enemies left: {_enemyList.Count}", TextColor.GREEN, _isDevMode);

        if (_enemyList.Count > 0)
        {
            _logger.Log($"There are {_enemyList.Count} remaining enemies left!", _isDevMode);
            return;
        }

        ResetWeapons();
        WaveComplete();
    }
    private void EVENT_AddToKills()
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
            _soundEmitter.PlaySound(_healSFX);

            UI_UpdatePlayerLife();
        }
    }

    private void UI_UpdateWaveIndex() => _waveCountTXT.text = $"Wave {_waveIndex + 1}";
    private void UI_UpdatePlayerLife() => _playerLivesTXT.text = $"Life: {_currHP}";
    private void UI_UpdateKllCount() => _killCountTXT.text = $"Kill Count: {_killCount}";
    private void UI_UpdateModifier() => _modifierTXT.text = $"{_modifier}";

    private void ToggleMainHand(InputAction.CallbackContext context)
    {
        _usingLeftHand = !_usingLeftHand;
        _logger.Log("Changed main hand!", _isDevMode);
    }
    private void ResetWeapons()
    {
        for (int i = 0; i < 2; i++)
        {
            _gameMgr.Player.LeftHandTools[i].SetActive(false);
            _gameMgr.Player.RightHandTools[i].SetActive(false);
        }

        _logger.Log("Weapons have been reset!", _isDevMode);
    }

    private void StartWave()
    {
        WeaponType preferredWeapon = _ghostBlockGridList[_waveIndex][(int)BlockType.WEAPON].WeaponType;

        // setup for the wave
        _gameMgr.Player.LeftHandTools[(int)preferredWeapon].SetActive(_usingLeftHand);
        _gameMgr.Player.RightHandTools[(int)preferredWeapon].SetActive(!_usingLeftHand);
        _unitCount = _enemiesToSpawn[_waveIndex];
        _modifier = _ghostBlockGridList[_waveIndex][(int)BlockType.MODIFIER].Modifier;
        _waveHandler.gameObject.SetActive(false);

        // implement modifier used
        switch (_modifier)
        {
            case Modifier.HEALTH:
                _currHP++;
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
            default:               break;
        }

        UpdateAllUI();

        _logger.Log($"{_prepTimes[_waveIndex]}s before enemy spawning!", TextColor.YELLOW, _isDevMode);
        _logger.Log($"{_gameMgr.Player} can start drawing!", TextColor.YELLOW, _isDevMode);

        StartCoroutine(CO_Spawning());
    }
    private void WaveComplete()
    {
        IEnumerator CO_FinishingActions()
        {
            if (_enemyList.Count > 0)
                ClearAllEnemies();

            _waveIndex++;
            _soundEmitter.PlaySound(_waveDoneSFX);
            _logger.Log($"Wave {_waveIndex + 1}", _isDevMode);

            UI_UpdateWaveIndex();

            yield return new WaitForSeconds(2f);

            if (_waveIndex == MAX_WAVE_INDEX)
            {
                _soundEmitter.PlaySound(_allWavesDoneSFX);
                _gameMgr.ExitVR();
                _logger.Log("All waves done!", _isDevMode);

                ResetGame();
            }
            else
            {
                StartWave();
                _logger.Log($"Starting wave: {_waveIndex}", TextColor.GREEN, _isDevMode);
            }
        }

        StartCoroutine(CO_FinishingActions());
    }
    private void ResetGame()
    {
        EnableButtons(false);
        _waveHandler.gameObject.SetActive(true);
        
        foreach (CodeBlock cb in _codeBlocks)
        {
            cb.ResetPosition();
            cb.gameObject.SetActive(true);
        }

        foreach (GhostBlock gb in _waveHandler.GhostBlocks)
            gb.ResetBlock();

        _logger.Log("Game has been reset!", _isDevMode);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_Spawning()
    {
        if (_coroutineRunning)
        {
            _logger.Log("Coroutine is already running!", _isDevMode);
            _sndMgr.PlaySound("SND_Unsure");
            yield break;
        }

        _coroutineRunning = true;
        _soundEmitter.PlaySound(_startWaveSFX);
        _blockLabelsUI.SetActive(false);

        yield return new WaitForSeconds(_prepTimes[_waveIndex]);

        // enemy spawning
        for (int i = 0; i < _unitCount; i++)
        {
            if (!_gameMgr.Player.InsideVRSpace)
            {
                _logger.Log("Player is now outside the VR Space!", _isDevMode);
                break;
            }

            SpawnEnemy();
            yield return new WaitForSeconds(1f / SPAWN_INTERVAL);
        }

        _coroutineRunning = false;
        _logger.Log("Finished spawning!", TextColor.YELLOW, _isDevMode);

        if (!_gameMgr.Player.InsideVRSpace)
            ClearAllEnemies();
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) ClearAllEnemies();
        if (Input.GetKeyDown(KeyCode.Space)) SpawnEnemy();
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_codeBlocks.Length == 11, "Missing elements in _codeBlockList!", this);
        Debug.Assert(_blockLabelsUI, "Missing _blockLabelsUI reference!", this);
        Debug.Assert(_confirmButton, "Missing _confirmButton reference!", this);
        Debug.Assert(_cancelButton, "Missing _cancelButton reference!", this);

        Debug.Assert(_waveCountTXT, "Missing _waveCountTXT reference!", this);
        Debug.Assert(_playerLivesTXT, "Missing _playerLivesTXT reference!", this);
        Debug.Assert(_killCountTXT, "Missing _killCountTXT reference!", this);

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
        _sndMgr = SoundManager.Instance;

        _enemyList = new List<GameObject>();

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

        _coroutineRunning = false;
    }

    private void UpdateAllUI()
    {
        UI_UpdateKllCount();
        UI_UpdatePlayerLife();
        UI_UpdateWaveIndex();
        UI_UpdateModifier();
    }
    private void ClearAllEnemies()
    {
        foreach (GameObject e in _enemyList)
            Destroy(e);

        _enemyList.Clear();
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