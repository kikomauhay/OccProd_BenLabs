using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GDDManager : Singleton<GDDManager>
{
    #region Properties

    public Transform TestGoal => _testGoal;
    public WaveState WaveState => _waveState;
    public Logger Logger => _logger;

    #endregion
    #region SerializeField

    [SerializeField] private Vector3 _testVec3;

    [Header("Spawn Bounds")]
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _spawnArea; // to prevent clutters in the hierarchy
    [SerializeField] private Transform _testGoal;

    [Header("Wave Components")]
    [SerializeField] private WaveData[] _waves;
    [SerializeField] private uint _currentWave;

    [Space(10f), SerializeField] private GameObject _testEnemy;

    #endregion
    #region Private

    private const float GRACE_PERIOD = 2.5f;

    [SerializeField] private List<GameObject> _enemyList;
    private WaveState _waveState;
    private uint _killCount;

    #endregion

    #region Unity

    private void Start()
    {
        Debug.Assert(_waves.Length != 0, "Missing _waves elements!", gameObject);

        InitComponents();
        InitVariables();
    }
    private void Update() => Test();

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
        StartCoroutine(CO_StartWave());

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
            e.GetComponent<Enemy>().OnDeath += CountRemainingEnemies;
            e.GetComponent<Enemy>().OnKilled += IncrementKillCount;

            _enemyList.Add(e.gameObject);
        }

        GameObject enemyToSpawn = _isDevMode ? _testEnemy : _enemyList[Random.Range(0, _enemyList.Count)];
        GameObject newEnemy = Instantiate(enemyToSpawn, RandomPositionInBox(), Quaternion.identity, _spawnArea);

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
            _logger.Log($"Enemies left: {_enemyList.Count}", ColorType.LIME);

        if (_enemyList.Count == 0)
        {
            _currentWave++;

            if (_currentWave < _waves.Length)
            {
                StartCoroutine(CO_StartWave());

                if (_isDevMode)
                    _logger.Log($"Current Wave: {_currentWave}", ColorType.LIME);
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

    private void InitComponents()
    {
        _collider = GetComponent<BoxCollider>();
    }
    private void InitVariables()
    {
        _enemyList = new List<GameObject>();
        _waveState = WaveState.WAITING;
        _killCount = 0;
    }
    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (GameObject e in _enemyList)
                Destroy(e);

            _enemyList.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Space)) SpawnEnemy(); // might break the continous wave spawning
        if (Input.GetKeyDown(KeyCode.Return)) StartCoroutine(CO_StartWave());
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_StartWave()
    {       
        if (_waveState == WaveState.SPAWNING) // prevents spawning overlaps
        {
            if (_isDevMode)
                _logger.Log("Still spawning enemies!", this, ColorType.RED);

            yield break;
        }

        if (_isDevMode)
            _logger.Log("Starting spawning!", ColorType.YELLOW);

        // wait time before spawning
        yield return new WaitForSeconds(GRACE_PERIOD);
        WaveData wave = _waves[_currentWave];
        _waveState = WaveState.SPAWNING;

        // enemy spawning
        for (int i = 0; i < wave.UnitCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f / wave.SpawnInterval);
        }
        _waveState = WaveState.FINISHED;

        if (_isDevMode)
            _logger.Log("Finished spawning!", ColorType.YELLOW);
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