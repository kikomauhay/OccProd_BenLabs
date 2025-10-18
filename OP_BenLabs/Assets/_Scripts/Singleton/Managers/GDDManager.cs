using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDDManager : Singleton<GDDManager>
{
    #region Properties

    public Transform TestGoal => _testGoal;
    public WaveState WaveState => _waveState;
    public Logger Logger => _logger;

    #endregion
    #region SerializeField

    [Header("Player Waypoints")]
    [SerializeField] private Transform _waypointGame;
    [SerializeField] private Transform _waypointR803;

    [Header("Spawn Bounds")]
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _spawnArea; // must be Vec3.zero so the children have normal scale
    [SerializeField] private Transform _testGoal;

    [Header("Trace Mechanic")]
    [SerializeField] private GameObject _drawingCanvas;
    [SerializeField] private WeaponSpawner _weaponSpawner;

    [Header("Wave Components")]
    [SerializeField] private uint _currentWave;
    [SerializeField] private WaveData[] _waves;
    [SerializeField] private List<GameObject> _enemyList; // seen in inspector for debugging
    
    [Space(10f), SerializeField] private GameObject _testEnemy;

    #endregion
    #region Private

    private GameManager _gameMgr;
    private OnboardingHandler _onbHandlr;

    private const float GRACE_PERIOD = 2.5f;
    
    private WaveState _waveState;
    private uint _killCount;

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

    public void RemoveEnemy(GameObject e) => _enemyList.Remove(e);
    public void UnbindEvents(Enemy e)
    {
        e.OnDeath -= CountRemainingEnemies;
        e.OnKilled -= IncrementKillCount;
    }
    public void BTN_PlayGame()
    {
        _gameMgr.TeleportPlayer(_waypointGame.position, true);
        StartCoroutine(CO_SpawnEnemyWave());

        if (_isDevMode)
            _logger.Log("GDD mini-game has started!");
    }
    public void BTN_PlayTutorial()
    {
        // TP player to the GDD area
        // _soundEmitter.PlaySound(_colliderCheck.WrongSFX);

        if (_isDevMode)
            _logger.Log("No tutorial mode yet!", TextColor.RED);
    }

    #endregion
    #region Private 
    
    private void SpawnEnemy()
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
            e.OnDeath += CountRemainingEnemies;
            e.OnKilled += IncrementKillCount;
            e.SetGoal(_testGoal);
            
            _enemyList.Add(e.gameObject);
        }

        GameObject enemyToSpawn = _isDevMode ? _testEnemy :
                                  _enemyList[Random.Range(0, _enemyList.Count)];

        GameObject newEnemy = Instantiate(enemyToSpawn, RandomPositionInBox(),
                                          Quaternion.identity, _spawnArea);

        SetUpEnemy(newEnemy.GetComponent<Enemy>());

        if (_isDevMode)
            _logger.Log("Spawned new enemy!");
    }
    private void StopGame() // only be called once player gets 0 HP
    {
        _gameMgr.TeleportPlayer(_waypointGame.position, false);
    }

    #endregion
    #region Events

    private void CountRemainingEnemies()
    {
        _waveState = WaveState.COUNTING;

        if (_isDevMode)
            _logger.Log($"Enemies left: {_enemyList.Count}", TextColor.GREEN);

        if (_enemyList.Count == 0)
        {
            _currentWave++;

            if (_currentWave < _waves.Length)
            {
                StartCoroutine(CO_SpawnEnemyWave());

                if (_isDevMode)
                    _logger.Log($"Current Wave: {_currentWave}", TextColor.GREEN);
            }
            // else AllWavesDone();
        }
    }
    private void IncrementKillCount()
    {
        _killCount++;

        if (_isDevMode)
            _logger.Log($"Kill Count: {_killCount}", this, TextColor.YELLOW);
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _drawingCanvas.SetActive(false);
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _onbHandlr = OnboardingHandler.Instance;

        _enemyList = new List<GameObject>();
        _waveState = WaveState.WAITING;
        _killCount = 0;
    }
    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (GameObject e in _enemyList)
                Destroy(e);

            _enemyList.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Space)) SpawnEnemy(); // might break the continous wave spawning
        if (Input.GetKeyDown(KeyCode.Return)) StartCoroutine(CO_SpawnEnemyWave());
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_SpawnEnemyWave()
    {
        WaveData wave = _waves[_currentWave];
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
                SpawnEnemy();
                yield return new WaitForSeconds(1f / wave.SpawnInterval);
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
    SPAWNING,
    WAITING,
    COUNTING,
    FINISHED
};