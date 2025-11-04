using System.Collections;
using UnityEngine;

public class BarManager : Singleton<BarManager>, IGameHandler
{
    #region Properties

    public Logger Logger => _logger;

    #endregion
    #region SerializeField

    [Header("Customer Spawning")]
    [SerializeField] private Transform _customerSpawnpoint; // add a y-offset before instantiating the prefab
    [SerializeField] private GameObject _customerPrefab, _testCustomer;

    [Header("Components")]
    [SerializeField] private ColliderCheck _colliderCheck;
    [SerializeField] private SoundEmitter _soundEmitter;

    [Header("UI/UX")]
    [SerializeField] private Sound _startGameSFX;
    [SerializeField] private GameObject _startButton, _tutorialButton;

    #endregion
    #region Private

    private GameManager _gameMgr;
    private OnboardingHandler _onbHandlr;

    private const int MAX_STRIKES = 3;
    private const int MAX_CUSTOMERS_SERVED = 3;
    private const float SERVING_SCORE = 100f;
    private const float GRACE_PERIOD = 2.5f;

    private int _currStrike, _customersServed;
    private float _totalScore;

    #endregion

    #region Unity

    protected override void Start()
    {
        // Debug.Assert(_customerPrefab, "Missing _customerPrefab reference!", gameObject);
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", gameObject);
        Debug.Assert(_colliderCheck, "Missing _colliderCheck reference!", gameObject);
        Debug.Assert(_customerSpawnpoint, "Missing _customerSpawnpoint reference!", gameObject);

        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    #endregion
    #region Public

    public void INT_BTN_StartGame()
    {
        _soundEmitter.PlaySound(_startGameSFX);
        _totalScore = 0;

        SpawnCustomer();

        if (_isDevMode)
            _logger.Log("Bar mini-game has started!");
    }
    public void INT_BTN_StartTutorial()
    {
        _soundEmitter.PlaySound(_colliderCheck.UnsureSFX);

        if (_isDevMode)
            _logger.Log("No tutorial mode yet!", TextColor.RED);
    }

    public void INT_DoGameOver()
    {
        // player gets exited from the mini-game
        // play game_over.sfx
        // show highest score attained

        // StopGame();

        if (_isDevMode)
            _logger.Log("No game over logic yet!", TextColor.RED);
    }
    public void SpawnCustomer()
    {
        IEnumerator CO_SpawnCustomer()
        {
            if (_colliderCheck.HasCustomer)
            {
                if (_isDevMode)
                    _logger.Log($"{_colliderCheck} already has a customer!", TextColor.RED);

                yield break;
            }
            if (_customersServed > MAX_CUSTOMERS_SERVED)
            {
                StopGame();
                yield break;
            }

            if (_isDevMode)
                _logger.Log($"{GRACE_PERIOD}s grace period before spawning!", TextColor.YELLOW);

            yield return new WaitForSeconds(GRACE_PERIOD);

            GameObject customerToSpawn = _isDevMode ? _testCustomer : _customerPrefab;
            GameObject newCustomer = Instantiate(customerToSpawn, _customerSpawnpoint.position,
                                                _customerSpawnpoint.rotation, _customerSpawnpoint);

            _colliderCheck.CustomerOrder = newCustomer.GetComponent<Customer>();

            if (_isDevMode)
                _logger.Log("Spawned new customer!");
        }

        StartCoroutine(CO_SpawnCustomer());
    }

    public void TrickPointAllocation(string trickName)
    {
        switch (trickName)
        {
            case "Pass.xml":
                _totalScore += 5F;
                return;

            case "Toss.xml":
                _totalScore += 10F;
                return;

            case "Spin.xml":
                _totalScore += 15F;
                return;
        }

    }


    public void Correct(float drinkScore)
    {
        _totalScore += drinkScore + SERVING_SCORE;
        _customersServed++;

        SpawnCustomer();
    }
    public void Wrong()
    {
        _currStrike++;
        _customersServed++;

        if (_currStrike == MAX_STRIKES)
        {
            INT_DoGameOver();
            return;
        }

        SpawnCustomer();
    }

    #endregion
    #region Private

    private void TrickScore(float trickScore)
    {
        _totalScore += trickScore;
    }

    private void StopGame()
    {
        _startButton.SetActive(true);

        StopAllCoroutines();
        
        if (_isDevMode)
            _logger.Log("Bar mini-game has finished!");
    }
    

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _startButton.SetActive(true);
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _onbHandlr = OnboardingHandler.Instance;

        _currStrike = 0;
        _customersServed = 0;
        _totalScore = 0f;
    }

    protected override void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Tab)) INT_BTN_StartGame();
        if (Input.GetKeyDown(KeyCode.CapsLock)) StopGame();
    }

    #endregion
}