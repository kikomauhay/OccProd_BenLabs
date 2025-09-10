using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class SceneHandler : Singleton<SceneHandler>
{
#region Members

#region Readers

    public bool CanPause { get; private set; }
    
#endregion
#endregion

#region Unity

    protected override void OnApplicationQuit() => base.OnApplicationQuit();
    protected override void Awake() => base.Awake();
    private void Start()
    {
        CanPause = true;

        // loads the next scene after persistent
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, 
                                    LoadSceneMode.Additive);
    }

#endregion

}
