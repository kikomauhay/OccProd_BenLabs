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

    [Header("Spawn Bounds")]
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _spawnArea; // to prevent clutters in the hierarchy
    [SerializeField] private Transform _testGoal;

    [Header("Trace Mechanic")]
    [SerializeField] private GameObject _drawingCanvas;
    [SerializeField] private WeaponSpawner _weaponSpawner;

    [Header("Wave Components")]
    [SerializeField] private uint _currentWave;
    [SerializeField] private WaveData[] _waves;

    [Space(10f), SerializeField] private GameObject _testEnemy;

    #endregion
    #region Private

    private const float GRACE_PERIOD = 2.5f;
    private GameManager _gameMgr = GameManager.Instance;

    [SerializeField] private List<GameObject> _enemyList;
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

        _drawingCanvas.SetActive(false);
    }

    #endregion
    #region Public

    public void RemoveEnemy(GameObject e) => _enemyList.Remove(e);
    public void UnbindEvents(GameObject e)
    {
        e.GetComponent<Enemy>().OnDeath -= CountRemainingEnemies;
        e.GetComponent<Enemy>().OnKilled -= IncrementKillCount;
    }
    public void BTN_PlayGame()
    {
        StartCoroutine(CO_SpawnEnemyWave());

        if (_isDevMode)
            _logger.Log("Mini-game has started!");
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

    #endregion
    #region Events

    private void CountRemainingEnemies()
    {
        _waveState = WaveState.COUNTING;

        if (_isDevMode)
            _logger.Log($"Enemies left: {_enemyList.Count}", ColorType.GREEN);

        if (_enemyList.Count == 0)
        {
            _currentWave++;

            if (_currentWave < _waves.Length)
            {
                StartCoroutine(CO_SpawnEnemyWave());

                if (_isDevMode)
                    _logger.Log($"Current Wave: {_currentWave}", ColorType.GREEN);
            }
            // else AllWavesDone();
        }
    }
    private void IncrementKillCount()
    {
        _killCount++;

        if (_isDevMode)
            _logger.Log($"Kill Count: {_killCount}", this, ColorType.YELLOW);
    }

    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        _enemyList = new List<GameObject>();
        _waveState = WaveState.WAITING;
        _killCount = 0;
    }
    protected override void Test()
    {
        if (!_isDevMode) return;

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
                _logger.Log($"{gracePeriod}s before enemy spawning!", ColorType.YELLOW);
                _logger.Log($"{_gameMgr.Player} can start drawing!", ColorType.YELLOW);
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
                _logger.Log("Finished spawning!", ColorType.YELLOW);
        }

        if (_waveState == WaveState.SPAWNING) // prevents wave spawning overlaps
        {
            if (_isDevMode)
                _logger.Log("Still spawning enemies!", this, ColorType.RED);

            yield break;
        }

        DoPrearation();
        yield return new WaitForSeconds(gracePeriod);

        if (_isDevMode)
            _logger.Log($"{_gameMgr.Player} can no longer draw!", ColorType.YELLOW);
        
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