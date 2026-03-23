using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(SoundEmitter))]
public class GDDManager : Singleton<GDDManager>, IGameHandler
{
    #region Inspector

    [SerializeField] private bool _enableDamage;

    [Header("Wave System")]
    [SerializeField] private WaveHandler _waveHandlr;
    [SerializeField] private BoxCollider _spawnpointBounds;
    [SerializeField] private GameObject _blockLabelsUI, _confirmButton, _cancelButton;
    [SerializeField] private CodeBlock[] _codeBlocks;
    [SerializeField] private GameObject[] _enemyPrefabs;

    [Header("VR Variables")]
    [SerializeField] private bool _usingLeftHand;
    [SerializeField] private InputActionReference _xrAButton, _xrXButton;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveCountTXT;
    [SerializeField] private TextMeshProUGUI _playerLivesTXT, _killCountTXT, _modifierTXT;

    [Header("SFXs")]
    [SerializeField] private Sound _startWaveSFX;
    [SerializeField] private Sound _waveDoneSFX, _allWavesDoneSFX;
    [SerializeField] private Sound _gameOverSFX, _playerHealSFX, _playerDamagedSFX;

    #endregion   
    #region Private

    private const int CODE_BLOCK_COUNT = 8;
    private const int STARTING_HEALTH = 5;
    private const int TOTAL_WAVE_COUNT = 2;
    private const int MAX_WEAPON_COUNT = 2;

    private SoundEmitter _sndEmtr;

    private Modifier _currentModifier; // used in UI & DoWavePreparations()
    private WaitForSeconds _gracePeriod, _second;

    private List<GhostBlock[]> _ghostBlockGridList;
    private List<GameObject> _enemyList;
    private float[] _enemyTimers; // also acts as the amount of enemies per wave

    private int _killCount, _waveIndex;
    private float _currHP, _timer;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _waveIndex++;
            a_logger.Log($"Wave index: {_waveIndex}", TextColor.Yellow, a_isDevMode);
        }

        if (Input.GetKeyDown(KeyCode.Backslash)) Instantiate(_enemyPrefabs[2]);
        if (Input.GetKeyDown(KeyCode.Return)) SpawnEnemy();
        if (Input.GetKeyDown(KeyCode.Space)) BTN_Confirm();
    }
    protected override void InitComponents()
    {
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_waveHandlr != null, this);
        a_logger.AssertReference(_spawnpointBounds != null, this);
        a_logger.AssertReference(_blockLabelsUI != null, this);
        a_logger.AssertReference(_confirmButton != null, this);
        a_logger.AssertReference(_cancelButton != null, this);
        a_logger.AssertCollection(_codeBlocks, this);
        a_logger.AssertCollection(_enemyPrefabs, this);

        a_logger.AssertReference(_xrAButton != null, this);
        a_logger.AssertReference(_xrXButton != null, this);

        a_logger.AssertReference(_waveCountTXT != null, this);
        a_logger.AssertReference(_playerLivesTXT != null, this);
        a_logger.AssertReference(_killCountTXT != null, this);
        a_logger.AssertReference(_modifierTXT != null, this);

        a_logger.AssertReference(_startWaveSFX != null, this);
        a_logger.AssertReference(_waveDoneSFX != null, this);
        a_logger.AssertReference(_allWavesDoneSFX != null, this);
        a_logger.AssertReference(_gameOverSFX != null, this);
        a_logger.AssertReference(_playerHealSFX != null, this);
        a_logger.AssertReference(_playerDamagedSFX != null, this);
    }
    protected override void InitVariables()
    {
        GhostBlock[] Take(int start, int length) => _waveHandlr.GhostBlocks[start..length];

        _currentModifier = Modifier.Default;
        _gracePeriod = new(2.5f);
        _second = new(1f);

        _ghostBlockGridList = new() { Take(0, 2), Take(2, 4), Take(4, 6) };
        _enemyList = new();
        _enemyTimers = new[] { 10f, 15f, 20f };

        _timer = 0;
        _killCount = 0;
        _waveIndex = 0;
        _currHP = STARTING_HEALTH;
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

        _waveHandlr.gameObject.SetActive(true);
        _blockLabelsUI.SetActive(true);

        EnableButtons(false);
        UpdateAllUI();
    }

    #endregion
    #region Public

    public void INT_BTN_StartGame() // enters the VR Space (not start the actual game)
    {
        a_gameMgr.EnterVR();
        a_logger.Log("Game start!", TextColor.Yellow, a_isDevMode);
    }
    public void INT_DoGameOver() // only gets called once player gets 0 HP
    {
        StopCoroutine(CO_StartEnemySpawning());
        ClearAllEnemies();
        
        a_audMgr.StopMusic();
        a_logger.Log("GDD Music Stopped!", TextColor.Yellow, a_isDevMode);

        a_gameMgr.ExitVR();
        a_gameMgr.UI_UpdateGDDScore(_killCount);
        a_logger.Log("Game over!", TextColor.Yellow, a_isDevMode);
        _sndEmtr.PlaySound(_gameOverSFX);

        ResetGame(isGameOver: true);
        ResetWeapons();

        StampCard.Instance.Stamp(3);
        a_gameMgr.EnableFinalNPCs();
    }
    public void INT_ResetValues()
    {
        _killCount = 0;
        _waveIndex = 0;
        _currHP = STARTING_HEALTH;
    }

    public void BTN_Cancel()
    {
        ResetGame(isGameOver: false);
        a_logger.Log("Nareset ang grid!", a_isDevMode);
    }
    public void BTN_Confirm()
    {
        if (a_audMgr.OnboardingPlaying)
            a_audMgr.StopOnboarding();

        foreach (CodeBlock cb in _codeBlocks)
            cb.gameObject.SetActive(false);

        a_audMgr.PlayMusic("SND_GDD_BGM");

        DoWavePreparations();
        StartCoroutine(CO_StartEnemySpawning());
    }

    public void RemoveEnemy(GameObject e) => _enemyList.Remove(e);
    public void UnbindEvents(Enemy e)
    {
        e.OnDeath -= EVENT_CountRemainingEnemies;
        e.OnKilled -= EVENT_AddToKills;
        e.OnKilled -= EVENT_GainLife;
    }

    public void TakeDamage()
    {
        if (_enableDamage)
        {
            _currHP--;
            _sndEmtr.PlaySound(_playerDamagedSFX);
            UI_UpdatePlayerLife();
        }

        if (_currHP < 1f)
        {
            _currHP = 0f;
            INT_DoGameOver();
        }

        // a_logger.Log($"HP: {_currHP}", a_isDevMode);
        // a_logger.Log($"Enemies left: {_enemyList.Count}", a_isDevMode);
    }
    public void EnableButtons(bool isActive)
    {
        _confirmButton.SetActive(isActive);
        _cancelButton.SetActive(isActive);
    }

    #endregion
    #region Private 

    private void EVENT_CountRemainingEnemies() // enemy.OnDeath
    {
        if (_enemyList.Count > 0)
        {
            // a_logger.Log($"There are {_enemyList.Count} remaining enemies left!", TextColor.Yellow, a_isDevMode);
            return;
        }

        _waveIndex++;
        UI_UpdateWaveIndex();
        ResetWeapons();

        if (_waveIndex > TOTAL_WAVE_COUNT)
        {
            a_audMgr.StopMusic();
            _sndEmtr.PlaySound(_allWavesDoneSFX);
            a_gameMgr.UI_UpdateGDDScore(_killCount);
            a_gameMgr.ExitVR();
            a_logger.Log("All waves done!", a_isDevMode);

            ResetGame(isGameOver: true);
            return;
        }

        ClearAllEnemies();
        DoWavePreparations();
        StartCoroutine(CO_StartEnemySpawning());
    }
    private void EVENT_AddToKills() // enemy.OnKilled
    {
        _killCount++;
        a_logger.Log($"Kill Count: <color={TextColor.Yellow}>{_killCount}</color>", a_isDevMode);

        UI_UpdateKillCount();
    }
    private void EVENT_GainLife() // enemy.OnKilled
    {
        if (Random.value < 0.1f)
        {
            _currHP++;
            _sndEmtr.PlaySound(_playerHealSFX);

            UI_UpdatePlayerLife();
        }
    }

    private void DoWavePreparations()
    {
        // assigns values based on the current wave
        WeaponType weaponType = _ghostBlockGridList[_waveIndex][(int)BlockType.Weapon].WeaponType;
        GameObject weapon = _usingLeftHand ? a_gameMgr.Player.LeftHandTools[(int)weaponType] :
                                            a_gameMgr.Player.RightHandTools[(int)weaponType];

        // setup before enemy spawning
        weapon.SetActive(true);
        _timer = _enemyTimers[_waveIndex];
        _currentModifier = _ghostBlockGridList[_waveIndex][(int)BlockType.Modifier].Modifier;
        _waveHandlr.gameObject.SetActive(false);

        // implement modifier used
        switch (_currentModifier)
        {
            case Modifier.Health:
                _currHP++;
                a_logger.Log("Increased HP!", a_isDevMode);
                break;

            case Modifier.Damage:
                weapon.GetComponent<Weapon>().BuffWeapon();
                a_logger.Log("Increased damage!", a_isDevMode);
                break;

            default: break;
        }

        UpdateAllUI();
    }
    private void ResetWeapons()
    {
        for (int i = 0; i < MAX_WEAPON_COUNT; i++)
        {
            a_gameMgr.Player.LeftHandTools[i].GetComponentInChildren<Weapon>().ResetWeapon();
            a_gameMgr.Player.LeftHandTools[i].SetActive(false);

            a_gameMgr.Player.RightHandTools[i].GetComponentInChildren<Weapon>().ResetWeapon();
            a_gameMgr.Player.RightHandTools[i].SetActive(false);
        }
    }
    private void ResetGame(bool isGameOver)
    {
        IEnumerator CO_DelayedEnable(bool isGameOver)
        {
            yield return new WaitForSeconds(isGameOver ? 3f : 0f);

            foreach (CodeBlock cb in _codeBlocks)
            {
                cb.ResetPosition();
                cb.gameObject.SetActive(true);
            }

            foreach (GhostBlock gb in _waveHandlr.GhostBlocks)
                gb.ResetBlock();

            _waveHandlr.gameObject.SetActive(true);
            _blockLabelsUI.SetActive(true);
            a_logger.Log("Blocks have been reenabled!", a_isDevMode);
        }

        EnableButtons(false);
        UpdateAllUI();
        INT_ResetValues();

        StartCoroutine(CO_DelayedEnable(isGameOver));
        GhostBlock.FilledBlocks = 0;
        a_logger.Log("Game has been reset!", a_isDevMode);
    }

    private void SpawnEnemy()
    {
        Vector3 GetRandomPositionInBox()
        {
            Vector3 size = _spawnpointBounds.size;
            Vector3 localPosition = new(Random.Range(-size.x / 2f, size.x / 2f),
                                        Random.Range(-size.y / 2f, size.y / 2f),
                                        Random.Range(-size.z / 2f, size.z / 2f));

            return _spawnpointBounds.transform.TransformPoint(_spawnpointBounds.center + localPosition);
        }
        void BindGDDEvents(Enemy e)
        {
            e.OnDeath += EVENT_CountRemainingEnemies;
            e.OnKilled += EVENT_AddToKills;
            e.OnKilled += EVENT_GainLife;

            e.Goal = a_gameMgr.Player.transform;
            _enemyList.Add(e.gameObject);

            // a_logger.Log($"Spawned new enemy at {e.transform.position}!", a_isDevMode);
        }

        int i = Random.Range(0, _waveIndex + 1);

        GameObject enemyObj = Instantiate(_enemyPrefabs[i]);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        enemyObj.transform.SetPositionAndRotation(GetRandomPositionInBox(), Quaternion.identity);
        enemy.EnemyType = (EnemyType)i;
        BindGDDEvents(enemy);
        enemyObj.SetActive(true);

        a_logger.Log($"Spawned an {(EnemyType)i} enemy!", a_isDevMode);
    }

    #region Helpers    

    private void UI_UpdateWaveIndex() => _waveCountTXT.text = $"Wave {_waveIndex + 1}";
    private void UI_UpdatePlayerLife() => _playerLivesTXT.text = $"Life: {_currHP}";
    private void UI_UpdateKillCount() => _killCountTXT.text = $"Kill Count: {_killCount}";
    private void UI_UpdateModifier() => _modifierTXT.text = $"{_currentModifier}";
    private void UpdateAllUI()
    {
        UI_UpdateKillCount();
        UI_UpdatePlayerLife();
        UI_UpdateWaveIndex();
        UI_UpdateModifier();
    }
    private void ClearAllEnemies()
    {
        for (int i = _enemyList.Count - 1; i >= 0; i--)
            Destroy(_enemyList[i]);

        _enemyList.Clear();
        a_logger.Log("Enemies have been cleared!", a_isDevMode);
    }

    private void ToggleMainHand(InputAction.CallbackContext context)
    {
        _usingLeftHand = !_usingLeftHand;
        a_logger.Log("Changed main hand!", a_isDevMode);
    }

    #endregion
    #endregion

    #region Enumerators

    private IEnumerator CO_StartEnemySpawning()
    {
        if (!a_gameMgr.Player.InsideVRSpace)
        {
            a_logger.Log("Player is outside the VR Space!", TextColor.Red, a_isDevMode);
            // a_audMgr.PlayWrong();
            yield break;
        }

        _sndEmtr.PlaySound(_startWaveSFX);
        _blockLabelsUI.SetActive(false);
        yield return _gracePeriod;

        while (_timer != 0f)
        {
            SpawnEnemy();
            _timer--;
            yield return _second;
        }

        a_logger.Log("Finished spawning!", TextColor.Yellow, a_isDevMode);
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