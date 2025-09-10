using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Unity

    private void Start()
    {
        InitComponents();
        InitVariables();
    }

    #endregion
    #region Helpers

    private void InitComponents() { }
    private void InitVariables() { }
        
    #endregion

}
