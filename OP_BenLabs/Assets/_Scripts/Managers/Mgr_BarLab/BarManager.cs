using System.Collections;
using UnityEngine;

public class BarManager : Singleton<BarManager>
{
    #region Properties

    public Logger Logger => _logger;

    #endregion
    #region SerializeField

    [Header("Customer Spawning")]
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private Transform _customerSpawnpoint;

    [Header("Scoring System")]
    [SerializeField] private ColliderCheck _colliderCheck;
    [SerializeField] private int _currStrike;
    [SerializeField] private float _totalScore;

    [Space(10f), SerializeField] private GameObject _testCustomer;

    #endregion
    #region Private

    private GameManager _gameMgr = GameManager.Instance;
    private const int MAX_STRIKES = 3;
    private const float SERVING_SCORE = 100f;
    private const float GRACE_PERIOD = 2.5f;

    #endregion

    #region Unity

    protected override void Start()
    {
        // Debug.Assert(_customerPrefab, "Missing _customerPrefab reference!", gameObject);
        Debug.Assert(_customerSpawnpoint, "Missing _customerSpawnpoint reference!", gameObject);
        Debug.Assert(_colliderCheck, "Missing _colliderCheck reference!", gameObject);

        base.Start();
    }

    #endregion
    #region Public

    private void BTN_PlayGame()
    {
        if (_isDevMode)
            _logger.Log("Mini-game has started!");

        // spawns the first customer
    }
    public void Wrong()
    {
        _currStrike++;
        // play wrong.sfx

        if (_currStrike == MAX_STRIKES)
        {
            DoGameOver();

            if (_isDevMode)
                _logger.Log("Mini-game has ended!", gameObject, ColorType.YELLOW);

            return;
        }

        StartCoroutine(CO_SpawnCustomer());
    }
    public void Correct(float drinkScore)
    {
        _totalScore += (drinkScore + SERVING_SCORE);
        StartCoroutine(CO_SpawnCustomer());
    }
    private void DoGameOver()
    {
        // player gets exited from the mini-game
        // play game_over.sfx
    }

    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        _currStrike = 0;
        _totalScore = 0f;
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_SpawnCustomer()
    {
        _colliderCheck.CustomerOrder = null;
        
        if (_colliderCheck.HasCustomer) 
        {
            if (_isDevMode)
                _logger.Log($"{_colliderCheck} already has a customer!", ColorType.RED);

            yield break;
        }

        yield return new WaitForSeconds(GRACE_PERIOD);

        GameObject customerToSpawn = _isDevMode ? _testCustomer : _customerPrefab;
        GameObject newCustomer = Instantiate(customerToSpawn, _customerSpawnpoint.position, 
                                            _customerSpawnpoint.rotation, _customerSpawnpoint);

        _colliderCheck.CustomerOrder = newCustomer.GetComponent<Customer>();

        if (_isDevMode)
            _logger.Log("Spawned new customer!");
    }
        
    #endregion
}
