using UnityEngine;

public class BarManager : Singleton<BarManager>
{
    #region SerializeField

    [Header("Customer Spawning")]
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private Transform _customerSpawnpoint;

    [Header("Scoring System")]
    [SerializeField] private Collider _colliders;
    [SerializeField] private int _currStrike;
        
    #endregion
    #region Private

    private GameManager _gameMgr = GameManager.Instance;
    private const int MAX_STRIKES = 3;
    
    #endregion
    
    #region Unity

    protected override void OnEnable()
    {
        _gameMgr.OnBarGameStart += StartGame;
    }
    protected override void OnDisable()
    {
        _gameMgr.OnBarGameStart -= StartGame;
    }
    private void Start()
    {
        InitComponents();
        InitVariables();
    }

    #endregion

    #region Game Loop

    private void StartGame()
    {
        if (_isDevMode)
            _logger?.Log("Started the game!");

        // spawns the first customer
    }
    public void Wrong()
    {
        _currStrike++;

        if (_currStrike == MAX_STRIKES)
        {
            DoGameOver();

            if (_isDevMode)
                _logger?.Log("Game has ended!", gameObject, ColorType.YELLOW);
        }
    }
    private void DoGameOver()
    {
        
    }

    #endregion
    #region Helpers

    private void InitComponents()
    {
        
    }
    private void InitVariables()
    {
        _currStrike = 0;
    }
    
    #endregion
}
