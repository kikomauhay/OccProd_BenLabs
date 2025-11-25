using System.Collections;
using TMPro;
using UnityEngine;

public class BarManager : Singleton<BarManager>, IGameHandler
{
    #region Properties

    public System.Action OnCustomerSpawn { get; set; }
    public bool MinigamePlaying => _minigamePlaying;

    #endregion
    #region SerializeField

    [Header("Customer Spawning")]
    [SerializeField] private Transform _customerSpawnpoint; // add a y-offset before instantiating the prefab
    [SerializeField] private GameObject _customerPrefab;

    [Header("Components")]
    [SerializeField] private ColliderCheck _colliderCheck;
    [SerializeField] private SoundEmitter _soundEmitter;
    [SerializeField] private GameObject _shakerCap;
    [SerializeField] private Transform _capSpawnPoint;
    [SerializeField] private XRTrickRecognizer _trickRecognizer;

    [Header("UI/UX")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject[] _onboardingBoxes;
    [SerializeField] private Sound _startGameSFX, _correctSFX, _wrongSFX;

    #endregion
    #region Private

    private SoundManager _sndMgr;

    private const int MAX_STRIKES = 3;
    private const int MAX_CUSTOMERS_SERVED = 3;
    private const float SERVING_SCORE = 100f;
    private const float GRACE_PERIOD = 2.5f;

    private int _currStrike, _customersServed;
    private float _totalScore;
    [SerializeField] private int _capSpawned;
    [SerializeField] private bool _minigamePlaying;

    #endregion

    #region Unity

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

    #endregion
    #region Public

    public void INT_BTN_StartGame()
    {
        EnableOnboardingPanels(false);

        if (_sndMgr.OnboardingPlaying)
            _sndMgr.StopOnboarding();

        _startButton.SetActive(false);
        _sndMgr.PlayMusic("SND_BAR_BGM_01");
        
        _totalScore = 0;
        _minigamePlaying = true;

        UI_UpdateScore();
        SpawnCustomer();

        _logger.Log("Bar mini-game has started!", _isDevMode);
    }
    public void INT_DoGameOver()
    {        
        // show highest score attained
        
        StopAllCoroutines();
        EnableOnboardingPanels(true);
        INT_ResetValues();

        OnCustomerSpawn?.Invoke(); // gets called to reset the bottles one last time

        _minigamePlaying = false;
        _startButton.SetActive(true);
        _sndMgr.StopMusic();
        _logger.Log("Bar mini-game has finished!", _isDevMode);
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
                _logger.Log("Already at the max amount of customers!", TextColor.RED, _isDevMode);
                yield break;
            }

            _soundEmitter.PlaySound(_startGameSFX);
            _logger.Log($"{GRACE_PERIOD}s grace period before spawning!", TextColor.YELLOW, _isDevMode);
            yield return new WaitForSeconds(GRACE_PERIOD);

            GameObject newCustomer = Instantiate(_customerPrefab, 
                                                 _customerSpawnpoint.position,
                                                 _customerSpawnpoint.rotation);

            _colliderCheck.CustomerOrder = newCustomer.GetComponent<Customer>();
            _logger.Log("Spawned new customer!", _isDevMode);
        }

        OnCustomerSpawn?.Invoke();
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

        _soundEmitter.PlaySound(_correctSFX);
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
        _soundEmitter.PlaySound(_wrongSFX);

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
        if (_capSpawned > 0)
        {
            _sndMgr.PlaySound("SND_Unsure");
            _logger.Log("Cap is already spawned!", _isDevMode);
            return;
        }

        Instantiate(_shakerCap, _capSpawnPoint);
        _capSpawned++;
        _logger.Log($"Cap Respawned at {_capSpawnPoint}", _isDevMode);
    }
    public void CapDespawned() => _capSpawned--;

    #endregion
    #region Private

    private void UI_UpdateScore() => _scoreText.text = $"Total Score: {_totalScore}";

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
    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) INT_BTN_StartGame();
        if (Input.GetKeyDown(KeyCode.Space)) SpawnCustomer();
    }

    protected override void AssertComponents()
    {
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
        _sndMgr = SoundManager.Instance;

        _currStrike = 0;
        _customersServed = 0;
        _totalScore = 0f;

        _capSpawned = 1;
        _minigamePlaying = false;
    }

    #endregion
}