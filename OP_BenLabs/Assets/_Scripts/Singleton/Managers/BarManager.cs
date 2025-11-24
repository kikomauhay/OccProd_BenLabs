using System.Collections;
using TMPro;
using UnityEngine;

public class BarManager : Singleton<BarManager>, IGameHandler
{
    #region SerializeField

    [Header("Customer Spawning")]
    [SerializeField] private Transform _customerSpawnpoint; // add a y-offset before instantiating the prefab
    [SerializeField] private GameObject _customerPrefab, _testCustomer;

    [Header("Components")]
    [SerializeField] private ColliderCheck _colliderCheck;
    [SerializeField] private SoundEmitter _soundEmitter;
    [SerializeField] private GameObject _shakerCap;
    [SerializeField] private Transform _capSpawnPoint;
    [SerializeField] private XRTrickRecognizer _trickRecognizer;

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
    private bool _capSpawned;

    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();
        _trickRecognizer.OnRecognized.AddListener(AddTrickScore);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        _trickRecognizer.OnRecognized.RemoveListener(AddTrickScore);
    }

    #region Public

    public void INT_BTN_StartGame()
    {
        EnableOnboardingPanels(false);

        if (_sndMgr.OnboardingPlaying)
            _sndMgr.StopOnboarding();

        _startButton.SetActive(false);
        _soundEmitter.PlaySound(_startGameSFX);
        _sndMgr.PlayMusic("SND_BAR_BGM_01");
        _totalScore = 0;

        UI_UpdateScore();
        SpawnCustomer();

        _logger.Log("Bar mini-game has started!", _isDevMode);
    }
    public void INT_DoGameOver()
    {
        // player gets exited from the mini-game
        // play game_over.sfx
        // show highest score attained

        StopGame();
        INT_ResetValues();

        _sndMgr.StopMusic();
        _logger.Log("No game over logic yet!", TextColor.RED, _isDevMode);
    }
    public void INT_ResetValues()
    {
        _currStrike = 0;
        _totalScore = 0;
        _customersServed = 0;
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
            if (_customersServed == MAX_CUSTOMERS_SERVED)
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
        _logger.Log("Calculating Score", _isDevMode);
        
        switch (trickName)
        {
            case "Pass":
                _logger.Log("Pass",_isDevMode);
                _totalScore += 5f;
                break;

            case "Toss":
                _logger.Log("Toss", _isDevMode);
                _totalScore += 10f;
                break;

            case "Spin":
                _logger.Log("Spin", _isDevMode);
                _totalScore += 15f;
                break;
            
            default: break;
        }

        _logger.Log("AddingScore", _isDevMode);
        
        UI_UpdateScore();
    }
    public void Correct()
    {
        IEnumerator CO_SpawnNewCustomer()
        {
            yield return new WaitForSeconds(GRACE_PERIOD);
            SpawnCustomer();
        }

        _totalScore += SERVING_SCORE;
        _customersServed++;
                    
        UI_UpdateScore();

        _sndMgr.PlaySound("SND_Correct");
        _logger.Log($"Total score: {_totalScore}", _isDevMode);

        ChangeMusic();
        StartCoroutine(CO_SpawnNewCustomer());
    }
    public void Wrong()
    {
        IEnumerator CO_SpawnNewCustomer()
        {
            yield return new WaitForSeconds(GRACE_PERIOD);
            SpawnCustomer();
        }
        _currStrike++;
        _customersServed++;

        _sndMgr.PlaySound("SND_Wrong");

        if (_currStrike == MAX_STRIKES)
        {
            INT_DoGameOver();
            return;
        }

        ChangeMusic();
        StartCoroutine(CO_SpawnNewCustomer());
    }

    public void SpawnCap()
    {
        if (_capSpawned)
        {
            _sndMgr.PlaySound("SND_Unsure");
            _logger.Log("Cap is already spawned!", _isDevMode);
            return;
        }

        Instantiate(_shakerCap, _capSpawnPoint);
        _capSpawned = true;
        _logger.Log($"Cap Respawned at {_capSpawnPoint}", _isDevMode);
    }
    public void CapDesapwned() => _capSpawned = false;

    #endregion
    #region Private

    private void UI_UpdateScore() => _scoreText.text = $"Total Score: {_totalScore}";

    private void StopGame()
    {
        StopAllCoroutines();
        EnableOnboardingPanels(true);

        _startButton.SetActive(true);
        _sndMgr.StopMusic();
        _logger.Log("Bar mini-game has finished!", _isDevMode);
    }
    private void EnableOnboardingPanels(bool isActive)
    {
        foreach (GameObject panels in _onboardingBoxes)
            panels.SetActive(isActive);  
    }
    private void ChangeMusic()
    {
        _sndMgr.StopMusic();
        _sndMgr.PlayMusic($"SND_BAR_BGM_0{_customersServed + 1}");

        //switch (_customersServed)
        //{
        //    case 1:
        //        _sndMgr.PlayMusic("SND_BAR_BGM_02");
        //        break;
        //    case 2:
        //        _sndMgr.PlayMusic("SND_BAR_BGM_02");
        //        break;

        //    default: break;
        //}

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
        _capSpawned = true;
    }

    protected override void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Tab)) INT_BTN_StartGame();
        if (Input.GetKeyDown(KeyCode.CapsLock)) StopGame();
        if (Input.GetKeyDown(KeyCode.Space)) SpawnCustomer();
    }


    #endregion
}