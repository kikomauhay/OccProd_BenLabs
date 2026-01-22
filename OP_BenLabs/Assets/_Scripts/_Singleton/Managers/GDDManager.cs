using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;

[RequireComponent(typeof(SoundEmitter))]
public class GDDManager : Singleton<GDDManager>, IGameHandler
{
    #region SerializeField

    [Header("Spawning & Waves")]
    [SerializeField] private BoxCollider _col;

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

    private const float SPAWN_INTERVAL = 0.5f;
    private const int MAX_WAVE_INDEX = 2;

    private readonly float[] _prepTimes = new float[3] { 15f, 10f, 7f };
    private readonly int[] _enemiesToSpawn = new int[3] { 8, 16, 20 };
    private List<List<GhostBlock>> _ghostBlockGridList;

    private SoundEmitter _sndEmitter;

    [SerializeField, Tooltip("Visible for testing")] 
    private List<GameObject> _enemyList;
    
    private Modifier _modifier;
    private int _unitCount, _killCount, _waveIndex;
    private float _currHP;
    private bool _coroutineRunning;

    #endregion

    #region Actor
    
    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) ClearAllEnemies();
        if (Input.GetKeyDown(KeyCode.Space)) SpawnEnemy();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_codeBlocks.Length == 11, this);
        a_logger.AssertReference(_blockLabelsUI, this);
        a_logger.AssertReference(_confirmButton, this);
        a_logger.AssertReference(_cancelButton, this);

        a_logger.AssertReference(_waveCountTXT, this);
        a_logger.AssertReference(_playerLivesTXT, this);
        a_logger.AssertReference(_killCountTXT, this);

        a_logger.AssertReference(_startWaveSFX, this);
        a_logger.AssertReference(_allWavesDoneSFX, this);
        a_logger.AssertReference(_gameOverSFX, this);
        a_logger.AssertReference(_healSFX, this);
        a_logger.AssertReference(_dmgSFX, this);
    }
    protected override void InitComponents()
    {        
        _sndEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
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
        a_gameMgr.EnterVR();

        // StartCoroutine(_gameMgr.CO_Enter(FloorType.GDD));
        a_logger.Log("Game start!", TextColor.Yellow, a_isDevMode);
    }
    public void INT_DoGameOver() // only gets called once player gets 0 HP
    {
        IEnumerator CO_GameOver()
        {
            a_gameMgr.ExitVR();
            _sndEmitter.PlaySound(_gameOverSFX);
            a_logger.Log("Game over!", TextColor.Yellow, a_isDevMode);

            if (_coroutineRunning)
            {
                _coroutineRunning = false;
                a_logger.Log("CO_Spawning() has stopped!", a_isDevMode);

                StopCoroutine(CO_Spawning());
            }

            ResetGame();
            ResetWeapons();
            ClearAllEnemies();

            yield return new WaitForSeconds(1f);

            _waveHandler.gameObject.SetActive(true);
            _blockLabelsUI.SetActive(true);
            a_audMgr.StopMusic();
        }

        StartCoroutine(CO_GameOver());
    }
    public void INT_ResetValues()
    {
        _killCount = 0;
        _waveIndex = 0;
        _currHP = 5f;

        a_logger.Log("Values have been reset!", a_isDevMode);
    }

    public void BTN_Cancel()
    {
        ResetGame();
        a_logger.Log("Reseting current grid!", a_isDevMode);
    }
    public void BTN_Confirm()
    {
        StartWave();

        if (a_audMgr.OnboardingPlaying)
            a_audMgr.StopOnboarding();
        
        a_logger.Log("Confirmed wave starting!", a_isDevMode);
    }

    public void SpawnEnemy()
    {
        Vector3 GetRandomPositionInBox()
        {
            Vector3 size = _col.size;
            Vector3 localPosition = new Vector3(Random.Range(-size.x / 2f, size.x / 2f),
                                                Random.Range(-size.y / 2f, size.y / 2f),
                                                Random.Range(-size.z / 2f, size.z / 2f));

            return _col.transform.TransformPoint(_col.center + localPosition);
        }
        void SetUpEnemy(Enemy e)
        {
            e.OnDeath += EVENT_CountEnemies;
            e.OnKilled += EVENT_AddToKills;
            e.OnKilled += EVENT_GainLife;

            e.SetGoal(a_gameMgr.Player.transform);

            _enemyList.Add(e.gameObject);
            a_logger.Log("Spawned new enemy!", a_isDevMode);
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
        _sndEmitter.PlaySound(_dmgSFX);
        UI_UpdatePlayerLife();

        if (_currHP < 1f)
        {
            _currHP = 0f;
            INT_DoGameOver();
        }

        // _logger.Log($"HP: {_currHP}", _isDevMode);
        a_logger.Log($"Enemies left: {_enemyList.Count}", a_isDevMode);
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
        a_logger.Log($"Enemies left: {_enemyList.Count}", TextColor.Lime, a_isDevMode);

        if (_enemyList.Count > 0)
        {
            a_logger.Log($"There are {_enemyList.Count} remaining enemies left!", TextColor.Yellow, a_isDevMode);
            return;
        }

        ResetWeapons();
        WaveComplete();
    }
    private void EVENT_AddToKills()
    {
        _killCount++;
        a_logger.Log($"Kill Count: <color={TextColor.Yellow}>{_killCount}</color>", a_isDevMode);
        
        UI_UpdateKllCount();
    }
    private void EVENT_GainLife()
    {
        if (Random.value < 0.1f)
        {
            _currHP++;
            _sndEmitter.PlaySound(_healSFX);

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
        a_logger.Log("Changed main hand!", a_isDevMode);
    }
    private void ResetWeapons()
    {
        ReadOnlyArray<GameObject> weapons = _usingLeftHand ? 
                                            a_gameMgr.Player.LeftHandTools : 
                                            a_gameMgr.Player.RightHandTools;
        
        foreach (GameObject w in weapons)
        {
            w.GetComponent<Weapon>().ResetWeapon();
            w.SetActive(false);
        }

        a_logger.Log("Weapons have been reset!", a_isDevMode);
    }

    private void StartWave()
    {
        WeaponType preferredWeapon = _ghostBlockGridList[_waveIndex][(int)BlockType.WEAPON].WeaponType;
        GameObject mainWeapon = _usingLeftHand ?
                                a_gameMgr.Player.LeftHandTools[(int)preferredWeapon] :
                                a_gameMgr.Player.RightHandTools[(int)preferredWeapon];
        
        foreach (CodeBlock cb in _codeBlocks)
            cb.gameObject.SetActive(false);

        // setup for the wave
        mainWeapon.SetActive(true);
        _unitCount = _enemiesToSpawn[_waveIndex];
        _modifier = _ghostBlockGridList[_waveIndex][(int)BlockType.MODIFIER].Modifier;
        _waveHandler.gameObject.SetActive(false);


        // implement modifier used
        switch (_modifier)
        {
            case Modifier.HEALTH:
                _currHP++;
                a_logger.Log("Increased HP!", a_isDevMode);
                break;

            case Modifier.DAMAGE:
                mainWeapon.GetComponent<Weapon>().BuffWeapon();
                a_logger.Log("Increased damage!", a_isDevMode);
                break;

            case Modifier.REDUCED_ENEMIES:
                _unitCount--;
                a_logger.Log($"Reduced enemy count from {_enemiesToSpawn[_waveIndex]} to {_unitCount}!", a_isDevMode);
                break;

            case Modifier.DEFAULT: break;
            default:               break;
        }

        UpdateAllUI();

        a_audMgr.PlayMusic("SND_GDD_BGM");
        a_logger.Log($"{_prepTimes[_waveIndex]}s before enemy spawning!", TextColor.Yellow, a_isDevMode);
        a_logger.Log($"{a_gameMgr.Player} can start drawing!", TextColor.Yellow, a_isDevMode);

        StartCoroutine(CO_Spawning());
    }
    private void WaveComplete()
    {
        IEnumerator CO_FinishingActions()
        {
            if (_enemyList.Count > 0)
                ClearAllEnemies();

            _waveIndex++;
            _sndEmitter.PlaySound(_waveDoneSFX);
            a_logger.Log($"Wave {_waveIndex + 1}", a_isDevMode);


            yield return new WaitForSeconds(2f);

            if (_waveIndex > MAX_WAVE_INDEX)
            {
                _sndEmitter.PlaySound(_allWavesDoneSFX);
                a_gameMgr.UI_UpdateGDDHiScore(_killCount);
                a_gameMgr.ExitVR();
                a_audMgr.StopMusic();
                a_logger.Log("All waves done!", a_isDevMode);

                ResetGame();
            }
            else
            {
                UI_UpdateWaveIndex();
                StartWave();
                a_logger.Log($"Starting wave: {_waveIndex}", TextColor.Lime, a_isDevMode);
            }
        }

        StartCoroutine(CO_FinishingActions());
    }
    private void ResetGame()
    {
        EnableButtons(false);
        INT_ResetValues();
        UpdateAllUI();

        _waveHandler.gameObject.SetActive(true);
        _blockLabelsUI.SetActive(true);
        
        foreach (CodeBlock cb in _codeBlocks)
        {
            cb.ResetPosition();
            cb.gameObject.SetActive(true);
        }

        foreach (GhostBlock gb in _waveHandler.GhostBlocks)
            gb.ResetBlock();

        GhostBlock.FilledBlocks = 0;
        a_logger.Log("Game has been reset!", a_isDevMode);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_Spawning()
    {
        if (_coroutineRunning)
        {
            a_logger.Log("Coroutine is already running!", a_isDevMode);
            a_audMgr.PlaySound("SND_Unsure");
            yield break;
        }

        _coroutineRunning = true;
        _sndEmitter.PlaySound(_startWaveSFX);
        _blockLabelsUI.SetActive(false);

        yield return new WaitForSeconds(_prepTimes[_waveIndex]);

        // enemy spawning
        for (int i = 0; i < _unitCount; i++)
        {
            if (!a_gameMgr.Player.InsideVRSpace)
            {
                a_logger.Log("Player is now outside the VR Space!", a_isDevMode);
                break;
            }

            SpawnEnemy();
            yield return new WaitForSeconds(1f / SPAWN_INTERVAL);
        }

        _coroutineRunning = false;
        a_logger.Log("Finished spawning!", TextColor.Yellow, a_isDevMode);

        if (!a_gameMgr.Player.InsideVRSpace)
            ClearAllEnemies();
    }

    #endregion
    #region Helpers    

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
    Spawning = 0,
    Waiting = 1,
    Counting = 2,
    Finished = 3
};