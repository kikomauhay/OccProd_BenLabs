using System.Collections;
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
    [SerializeField] private float _totalScore;

    #endregion
    #region Private

    private GameManager _gameMgr = GameManager.Instance;
    private const int MAX_STRIKES = 3;
    private const float SERVING_SCORE = 100f;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        _gameMgr.OnBarGameStart += BTN_PlayGame;
    }
    protected override void OnDisable()
    {
        _gameMgr.OnBarGameStart -= BTN_PlayGame;
    }
    private void Start()
    {
        InitComponents();
        InitVariables();
    }

    #endregion

    #region Game Loop

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
        }
    }
    private void DoGameOver()
    {
        // player gets exited from the mini-game
        // play game_over.sfx
    }

    #endregion
    #region Helpers

    private void InitComponents()
    {
        // Debug.Assert(_customerPrefab, "Missing _customerPrefab reference!", gameObject);
        // Debug.Assert(_customerSpawnpoint, "Missing _customerSpawnpoint reference!", gameObject);
    }
    private void InitVariables()
    {
        _currStrike = 0;
        _totalScore = 0f;
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_SpawnCustomer()
    {
        yield break;
    }
        
    #endregion
}
