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
 
    private const int MAX_STRIKES = 3;
    private const int MAX_CUSTOMERS_SERVED = 3;
    private const float SERVING_SCORE = 100f;
    private const float GRACE_PERIOD = 2.5f;

    private int _currStrike, _customersServed;
    private float _totalScore;
    [SerializeField] private int _capSpawned;
    [SerializeField] private bool _minigamePlaying;

    #endregion

    #region Actor
    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) INT_BTN_StartGame();
        if (Input.GetKeyDown(KeyCode.Space)) SpawnCustomer();
    }
    protected override void InitComponents()
    {
        _startButton.SetActive(true);
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_soundEmitter, this);
        a_logger.AssertReference(_colliderCheck, this);
        a_logger.AssertReference(_customerSpawnpoint, this);
    }
    protected override void InitVariables()
    {
        _currStrike = 0;
        _customersServed = 0;
        _totalScore = 0f;

        _capSpawned = 1;
        _minigamePlaying = false;
    }

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

        if (a_audMgr.OnboardingPlaying)
            a_audMgr.StopOnboarding();

        _startButton.SetActive(false);
        a_audMgr.PlayMusic("SND_BAR_BGM_01");
        
        _totalScore = 0;
        _minigamePlaying = true;

        UI_UpdateScore();
        SpawnCustomer();

        a_logger.Log("Bar mini-game has started!", a_isDevMode);
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
        a_audMgr.StopMusic();
        a_logger.Log("Bar mini-game has finished!", a_isDevMode);
        StampCard.Instance.Stamp(6);
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
                a_logger.Log($"{_colliderCheck} already has a customer!", TextColor.Red, a_isDevMode);
                yield break;
            }
            if (_customersServed == MAX_CUSTOMERS_SERVED)
            {
                a_logger.Log("Already at the max amount of customers!", TextColor.Red, a_isDevMode);
                yield break;
            }

            _soundEmitter.PlaySound(_startGameSFX);
            a_logger.Log($"{GRACE_PERIOD}s grace period before spawning!", TextColor.Yellow, a_isDevMode);
            yield return new WaitForSeconds(GRACE_PERIOD);

            GameObject newCustomer = Instantiate(_customerPrefab, 
                                                 _customerSpawnpoint.position,
                                                 _customerSpawnpoint.rotation);

            _colliderCheck.CustomerOrder = newCustomer.GetComponent<Customer>();
            a_logger.Log("Spawned new customer!", a_isDevMode);
        }

        OnCustomerSpawn?.Invoke();
        StartCoroutine(CO_SpawnCustomer());
    }

    public void AddTrickScore(string trickName)
    {
        a_logger.Log("Calculating Score", a_isDevMode);
        
        switch (trickName)
        {
            case "Pass":
                a_logger.Log("Pass",a_isDevMode);
                _totalScore += 5f;
                break;

            case "Toss":
                a_logger.Log("Toss", a_isDevMode);
                _totalScore += 10f;
                break;

            case "Spin":
                a_logger.Log("Spin", a_isDevMode);
                _totalScore += 15f;
                break;
            
            default: break;
        }

        a_logger.Log("AddingScore", a_isDevMode);
        
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
        a_logger.Log($"Total score: {_totalScore}", a_isDevMode);

        if (_customersServed == MAX_CUSTOMERS_SERVED)
        {
            INT_DoGameOver();
            return;
        }

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

        if (_currStrike == MAX_STRIKES || _customersServed == MAX_CUSTOMERS_SERVED)
        {
            INT_DoGameOver();
            return;
        }

        ChangeMusic();
        StartCoroutine(CO_SpawnNewCustomer());
    }

    public void DecreaseScore()
    {
        _totalScore -= 20f;

        if (_totalScore < 0f)
            _totalScore = 0f;

        UI_UpdateScore();
    }
    public void SpawnCap()
    {
        if (_capSpawned > 0)
        {
            a_audMgr.PlaySound("SND_Unsure");
            a_logger.Log("Cap is already spawned!", a_isDevMode);
            return;
        }

        Instantiate(_shakerCap, _capSpawnPoint);
        _capSpawned++;
        a_logger.Log($"Cap Respawned at {_capSpawnPoint}", a_isDevMode);
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
        a_audMgr.StopMusic();
        a_audMgr.PlayMusic($"SND_BAR_BGM_0{_customersServed + 1}");
    }

    #endregion    
}