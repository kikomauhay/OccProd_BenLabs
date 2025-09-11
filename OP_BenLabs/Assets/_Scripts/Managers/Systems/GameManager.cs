using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Properies

    public System.Action OnBarGameStart { get; set; }
        
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
    private void InitVariables() { }

    private void Test()
    {
        if (!_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnBarGameStart?.Invoke();
        }
    }

    #endregion

}
