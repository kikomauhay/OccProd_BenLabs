using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GDDManager : Singleton<GDDManager>
{
    #region Properties

    public WaveState WaveState => _waveState;

    #endregion
    #region SerializeField

    [Header("Spawn Bounds")]
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _spawnArea; // to prevent clutters in the hierarchy

    [Header("Wave Components")]
    [SerializeField] private WaveData[] _waves;
    [SerializeField] private uint _currentWave;

    [Space(10f), SerializeField] private GameObject _testEnemy;

    #endregion
    #region Private

    private List<GameObject> _enemies;
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
    private void OnDrawGizmos()
    {
        if (!_isDevMode) return;

        Gizmos.color = Color.white;
        Gizmos.DrawCube(_collider.center, _collider.size);
    }

    #endregion
    #region Public

    public void RemoveEnemy(GameObject e)
    {
        e.GetComponent<Enemy>().OnDeath -= CountRemainingEnemies;
        _enemies.Remove(e);
    }
    public void BTN_PlayGame()
    {
        StartCoroutine(CO_StartWave());

        if (_isDevMode)
            _logger.Log("Mini-game has started!");
    }

    #endregion
    #region Private

    private void CountRemainingEnemies()
    {
        _waveState = WaveState.COUNTING;
        _killCount++;

        if (_enemies.Count == 0)
        {
            // finish current wave

            if (_currentWave < 3)
                _currentWave++;
        }
    }
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

        GameObject enemyToSpawn = _isDevMode ? _testEnemy : _enemies[Random.Range(0, _enemies.Count)];
        GameObject newEnemy = Instantiate(enemyToSpawn,
                                          RandomPositionInBox(),
                                          Quaternion.identity,
                                          _spawnArea);

        _enemies.Add(newEnemy);
        newEnemy.GetComponent<Enemy>().OnDeath += CountRemainingEnemies;

        if (_isDevMode)
            _logger.Log("Spawned new enemy!");
    }
        
    #endregion
    #region Helpers

    private void InitComponents()
    {
        _collider = GetComponent<BoxCollider>();
    }
    private void InitVariables()
    {
        _enemies = new List<GameObject>();
        _waveState = WaveState.WAITING;
        _killCount = 0;
    }
    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (GameObject e in _enemies)
                Destroy(e);            

            _enemies.Clear();
        }    
        if (Input.GetKeyDown(KeyCode.Space)) SpawnEnemy();
    } 

    #endregion
    #region Enumerators

    private IEnumerator CO_StartWave()
    {
        WaveData wave = _waves[_currentWave];

        _waveState = WaveState.SPAWNING;

        for (int i = 0; i < wave.UnitCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f / wave.SpawnInterval);
        }
        _waveState = WaveState.WAITING;
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