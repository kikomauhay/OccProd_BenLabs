using System.Collections;
using TMPro;
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
    [SerializeField] private GameObject _shakerCap;
    [SerializeField] private Transform _capSpawnPoint;

    [Header("UI/UX")]
    [SerializeField] private Sound _startGameSFX;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject[] _onboardingBoxes;
    [SerializeField] private TextMeshProUGUI _scoreText;

    #endregion
    #region Private

    private GameManager _gameMgr;
    private OnboardingHandler _onbHandlr;
    private SoundManager _sndMgr;

    private const int MAX_STRIKES = 3;
    private const int MAX_CUSTOMERS_SERVED = 3;
    private const float SERVING_SCORE = 100f;
    private const float GRACE_PERIOD = 2.5f;

    private int _currStrike, _customersServed;
    private float _totalScore;

    #endregion

    #region Public

    public void INT_BTN_StartGame()
    {
        EnableOnboardingPanels(false);      
        
        _startButton.SetActive(false);
        _soundEmitter.PlaySound(_startGameSFX);
        _totalScore = 0;

        UpdateScore();
        SpawnCustomer();
        _logger.Log("Bar mini-game has started!", _isDevMode);
    }
    public void INT_DoGameOver()
    {
        // player gets exited from the mini-game
        // play game_over.sfx
        // show highest score attained

        StopGame();
        _logger.Log("No game over logic yet!", TextColor.RED, _isDevMode);
    }

    public void SpawnCustomer()
    {
        IEnumerator CO_SpawnCustomer()
        {
            if (_colliderCheck.HasCustomer)
            {
                _logger.Log($"{_colliderCheck} already has a customer!", TextColor.RED, _isDevMode);
                yield break;
            }
            if (_customersServed > MAX_CUSTOMERS_SERVED)
            {
                StopGame();
                yield break;
            }
            
            _logger.Log($"{GRACE_PERIOD}s grace period before spawning!", TextColor.YELLOW, _isDevMode);
            yield return new WaitForSeconds(GRACE_PERIOD);

            GameObject customerToSpawn = _isDevMode ? _testCustomer : _customerPrefab;
            GameObject newCustomer = Instantiate(customerToSpawn, 
                                                _customerSpawnpoint.position,
                                                _customerSpawnpoint.rotation, 
                                                _customerSpawnpoint);

            _colliderCheck.CustomerOrder = newCustomer.GetComponent<Customer>();
            _logger.Log("Spawned new customer!", _isDevMode);
        }

        StartCoroutine(CO_SpawnCustomer());
    }

    public void AddTrickScore(string trickName)
    {
        switch (trickName)
        {
            case "Pass.xml":
                _totalScore += 5f;
                break;

            case "Toss.xml":
                _totalScore += 10f;
                break;

            case "Spin.xml":
                _totalScore += 15f;
                break;
            
            default: break;
        }

        UpdateScore();
    }
    public void Correct()
    {
        _totalScore += SERVING_SCORE;
        _customersServed++;
        UpdateScore();

        _sndMgr.PlaySound("SND_Correct");
        _logger.Log($"Total score: {_totalScore}", _isDevMode);

        SpawnCustomer();
    }
    public void Wrong()
    {
        _currStrike++;
        _customersServed++;

        _sndMgr.PlaySound("SND_Wrong");

        if (_currStrike == MAX_STRIKES)
        {
            INT_DoGameOver();
            return;
        }

        SpawnCustomer();
    }

    public void SpawnCap()
    {
        Instantiate(_shakerCap, _capSpawnPoint);
        _logger.Log($"Cap Respawned at {_capSpawnPoint}", _isDevMode);
    }

    #endregion
    #region Private

    private void StopGame()
    {
        StopAllCoroutines();

        EnableOnboardingPanels(true);
        _startButton.SetActive(true);
        
        _logger.Log("Bar mini-game has finished!", _isDevMode);
    }
    private void EnableOnboardingPanels(bool isActive)
    {
        foreach (GameObject panels in _onboardingBoxes)
            panels.SetActive(isActive);  
    }


    #endregion
    #region Helpers

    protected override void AssertComponents()
    {
        // Debug.Assert(_customerPrefab, "Missing _customerPrefab reference!", gameObject);
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", gameObject);
        Debug.Assert(_colliderCheck, "Missing _colliderCheck reference!", gameObject);
        Debug.Assert(_customerSpawnpoint, "Missing _customerSpawnpoint reference!", gameObject);
    }
    protected override void InitComponents()
    {
        _startButton.SetActive(true);
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _onbHandlr = OnboardingHandler.Instance;
        _sndMgr = SoundManager.Instance;

        _currStrike = 0;
        _customersServed = 0;
        _totalScore = 0f;
    }

    protected override void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Tab)) INT_BTN_StartGame();
        if (Input.GetKeyDown(KeyCode.CapsLock)) StopGame();
        if (Input.GetKeyDown(KeyCode.Space)) SpawnCustomer();
    }

    private void UpdateScore() => _scoreText.text = "Total Score: " + _totalScore.ToString();

    #endregion
}