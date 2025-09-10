using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager> 
{

#region Unity

    protected override void OnApplicationQuit() => base.OnApplicationQuit();
    protected override void Awake() => base.Awake();
    
#endregion

}
