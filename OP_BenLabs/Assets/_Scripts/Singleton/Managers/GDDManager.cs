using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SoundEmitter))]
public class GDDManager : Singleton<GDDManager>, IGameHandler
{
    #region Properties

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
    [SerializeField] private List<CodeBlock> _availableBlocksList;

    [Header("Trace Mechanic")]
    [SerializeField] private GameObject _drawingCanvas;
    [SerializeField] private GameObject _swordImage, _hammerImage;
    [SerializeField, Space(10f)] private WeaponSpawner _weaponSpawner;
    [SerializeField] private string _preferredWeapon;

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
    }
    protected override void OnDisable()
    {
        _waveHandler.OnAllBlocksFilled -= EVENT_StartWave;
    }
    protected override void Start()
    {
        Debug.Assert(_availableBlocksList.Count == 11, "Missing elements in _codeBlockList!", gameObject);
        Debug.Assert(_drawingCanvas, "Missing _drawingCanvas reference!", gameObject);
        Debug.Assert(_weaponSpawner, "Missing _weaponSpawner reference!", gameObject);

        base.Start();
    }

    #endregion
    #region Public

    public string GetPreferredWeapon()
    {
        return _preferredWeapon;
    }

    public void INT_BTN_StartGame()
    {
        StartCoroutine(_gameMgr.CO_Enter(FloorType.GDD));

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

            e.SetGoal(_testGoal);

            _enemyList.Add(e.gameObject);
        }

        // GameObject enemyToSpawn = _isDevMode ? _testEnemy : _enemyList[Random.Range(0, _enemyList.Count)];
        GameObject enemy = _ghostBlockGridList[_waveIndex][(int)BlockType.ENEMY].EnemyPrefab;
        GameObject newEnemy = Instantiate(enemy, RandomPositionInBox(), Quaternion.identity, _spawnArea);

        // SetUpEnemy(newEnemy.GetComponent<Enemy>());
        SetUpEnemy(enemy.GetComponent<Enemy>());

        if (_isDevMode)
            _logger.Log("Spawned new enemy!");
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

            if (_waveIndex < 2)
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
    }

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

    #endregion
    #region Enumerators    

    private IEnumerator CO_SpawnEnemyWave()
    {        
        Modifier modifier = Modifier.DEFAULT;

        void PrepareWave() // prep time for the player to "draw" a weapon
        {
            // play StartTimer.sfx

            _waveHandler.gameObject.SetActive(false);
            _drawingCanvas.SetActive(true);

            // variable assignment
            _preferredWeapon = _ghostBlockGridList[_waveIndex][(int)BlockType.WEAPON].WeaponContent;
            modifier =         _ghostBlockGridList[_waveIndex][(int)BlockType.MODIFIER].Modifier;

            if      (_preferredWeapon == "Sword")  _swordImage.SetActive(true);
            else if (_preferredWeapon == "Hammer") _hammerImage.SetActive(true);
      
            switch (modifier)
            {
                case Modifier.HEALTH: 
                    if (_isDevMode) 
                        _logger.Log("Increased HP!", TextColor.GREEN);
                    
                    break;                
                
                case Modifier.DAMAGE:
                    if (_isDevMode) 
                        _logger.Log("Increased damage!", TextColor.GREEN);
                    
                    break;                
                
                case Modifier.REDUCED_ENEMIES: 
                    if (_isDevMode) 
                        _logger.Log("reduced enemies!", TextColor.GREEN);
                    
                    break;                
                
                case Modifier.DEFAULT: break;                
                default:               break;
            }

            if (_isDevMode)
            {
                _logger.Log($"{GRACE_PERIOD}s before enemy spawning!", TextColor.YELLOW);
                _logger.Log($"{_gameMgr.Player} can start drawing!", TextColor.YELLOW);
            }
        }
        IEnumerator CO_DoEnemySpawning()
        {
            // play StopTimer.sfx

            _waveState = WaveState.SPAWNING;
            _drawingCanvas.SetActive(false);

            for (int i = 0; i < _enemiesToSpawn[_waveIndex]; i++)
            {
                SpawnEnemy();
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