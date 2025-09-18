using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Properies

    public System.Action OnBarGameStart { get; set; }
    public float BarHighScore
    {
        get => _barHighScore;
        set
        {
            if (value < 0f) return;
            if (value < _barHighScore) return;

            _barHighScore = value;
        }
    }
        
    #endregion
    #region Private

    private float _barHighScore;
        
    #endregion

    #region Unity

    private void Start()
    {
        InitComponents();
        InitVariables();
    }
    private void Update()
    {
        Test();
    }

    #endregion
    #region Helpers

    private void InitComponents() { }
    private void InitVariables() 
    {
        _barHighScore = 0f;
    }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Space)) OnBarGameStart?.Invoke();        
    }

    #endregion

}
