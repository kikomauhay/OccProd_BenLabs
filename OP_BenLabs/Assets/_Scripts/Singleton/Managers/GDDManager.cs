using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SoundEmitter))]
public class GDDManager : Singleton<GDDManager>, IGameHandler
{
    #region Properties

    public Logger Logger => _logger;

    #endregion
    #region SerializeField

    [Header("Spawning & Waves")]
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _spawnArea; // must be Vec3.zero so the child GOs will have normal scale
    [SerializeField] private Transform _testGoal; // will remove one the GDD game is more structured
    [SerializeField] private WaveData[] _waves;

    [Header("Trace Mechanic")]
    [SerializeField] private GameObject _drawingCanvas;
    [SerializeField] private WeaponSpawner _weaponSpawner;

    [Space(10f), SerializeField] private GameObject _testEnemy;

    [Header("UI/UX")]
    [SerializeField] private TextMeshProUGUI _waveCountTXT;
    [SerializeField] private TextMeshProUGUI _playerLivesTXT;
    [SerializeField] private Sound _startGameSFX, _gameOverSFX;

    #endregion
    #region Private

    private GameManager _gameMgr;
    private OnboardingHandler _onbHandlr;
    private SoundEmitter _soundEmitter;

    private const float GRACE_PERIOD = 2.5f;
    private const float SPAWN_INTERVAL = 0.5f;

    private List<GameObject> _enemyList;
    private WaveState _waveState;
    private uint _killCount, _waveIndex;
    private float _currHP;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_waves.Length != 0, "Missing _waves elements!", gameObject);
        Debug.Assert(_drawingCanvas, "Missing _drawingCanvas reference!", gameObject);
        Debug.Assert(_weaponSpawner, "Missing _weaponSpawner reference!", gameObject);

        base.Start();
    }

    #endregion
    #region Public

    public void INT_BTN_StartGame()
    {
        StartCoroutine(_gameMgr.CO_Enter(FloorType.GDD));
        StartCoroutine(CO_SpawnEnemyWave());

        if (_isDevMode)
            _logger.Log("Game start!", TextColor.YELLOW);
    }
    public void INT_BTN_StartTutorial()
    {
        // TP player to the GDD area
        // _soundEmitter.PlaySound(_colliderCheck.WrongSFX);

        if (_isDevMode)
            _logger.Log("No tutorial mode yet!", TextColor.RED);
    }

    public void INT_DoGameOver() // only be called once player gets 0 HP
    {
        StartCoroutine(_gameMgr.CO_Exit(FloorType.GDD));

        if (_isDevMode)
            _logger.Log("Game over!", TextColor.YELLOW);
    }
    public void INT_SpawnUnit()
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

            e.SetGoal(_testGoal);

            _enemyList.Add(e.gameObject);
        }

        GameObject enemyToSpawn = _isDevMode ? _testEnemy : _enemyList[Random.Range(0, _enemyList.Count)];
        GameObject newEnemy = Instantiate(enemyToSpawn, RandomPositionInBox(), Quaternion.identity, _spawnArea);

        SetUpEnemy(newEnemy.GetComponent<Enemy>());

        if (_isDevMode)
            _logger.Log("Spawned new enemy!");
    }

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

        if (_isDevMode)
            _logger.Log($"Enemies left: {_enemyList.Count}", TextColor.GREEN);

        if (_enemyList.Count == 0)
        {
            _waveIndex++;
            UI_UpdateWaveIndex();

            if (_waveIndex < _waves.Length)
            {
                StartCoroutine(CO_SpawnEnemyWave());

                if (_isDevMode)
                    _logger.Log($"Current Wave: {_waveIndex}", TextColor.GREEN);
            }
        }
    }
    private void EVENT_IncrementKillCount()
    {
        _killCount++;

        if (_isDevMode)
            _logger.Log($"Kill Count: {_killCount}", this, TextColor.YELLOW);
    }
    private void EVENT_GainLife()
    {
        if (Random.value < 0.1f)
        {
            _currHP++;
            UI_UpdatePlayerLife();
        }
    }

    private void UI_UpdateWaveIndex()
    {
        IEnumerator CO_ClearWaveTxt()
        {
            _waveCountTXT.gameObject.SetActive(true);
            _waveCountTXT.text = $"Wave {_waveIndex + 1}";

            yield return new WaitForSeconds(15f);
            _waveCountTXT.gameObject.SetActive(false);
        }

        StartCoroutine(CO_ClearWaveTxt());
    }
    private void UI_UpdatePlayerLife() => _waveCountTXT.text = $"Life: {_currHP}";

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _drawingCanvas.SetActive(false);
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
        _currHP = 0f;
    }

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (GameObject e in _enemyList)
                Destroy(e);

            _enemyList.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Space)) INT_SpawnUnit(); // might break the continous wave spawning
        if (Input.GetKeyDown(KeyCode.Return)) StartCoroutine(CO_SpawnEnemyWave());
    }

    #endregion
    #region Enumerators    

    private IEnumerator CO_SpawnEnemyWave()
    {
        WaveData wave = _waves[_waveIndex];
        float gracePeriod = _isDevMode ? GRACE_PERIOD : wave.GracePeriod;

        void DoPrearation() // prep time for the player to "draw" a weapon
        {
            _drawingCanvas.SetActive(true);

            if (_isDevMode)
            {
                _logger.Log($"{gracePeriod}s before enemy spawning!", TextColor.YELLOW);
                _logger.Log($"{_gameMgr.Player} can start drawing!", TextColor.YELLOW);
            }
        }
        IEnumerator CO_DoEnemySpawning() // spawning starts and drawing stops
        {
            _waveState = WaveState.SPAWNING;
            _drawingCanvas.SetActive(false);

            for (int i = 0; i < wave.UnitCount; i++)
            {
                INT_SpawnUnit();
                yield return new WaitForSeconds(1f / SPAWN_INTERVAL);
            }
            _waveState = WaveState.FINISHED;

            if (_isDevMode)
                _logger.Log("Finished spawning!", TextColor.YELLOW);
        }

        if (_waveState == WaveState.SPAWNING) // prevents wave spawning overlaps
        {
            if (_isDevMode)
                _logger.Log("Still spawning enemies!", this, TextColor.RED);

            yield break;
        }

        DoPrearation();
        yield return new WaitForSeconds(gracePeriod);

        if (_isDevMode)
            _logger.Log($"{_gameMgr.Player} can no longer draw!", TextColor.YELLOW);

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