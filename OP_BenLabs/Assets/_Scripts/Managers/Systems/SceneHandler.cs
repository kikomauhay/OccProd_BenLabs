using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler>
{
    #region Properties

    public bool CanPause { get; private set; }

    #endregion
    #region SerializeField

    [Space(10f), SerializeField] private string _startingSceneToLoad;

    #endregion

    #region Unity

    private void Start()
    {
        InitVariables();

        SceneManager.LoadSceneAsync(_startingSceneToLoad, LoadSceneMode.Additive);
    }

    #endregion
    #region Helpers

    private void InitVariables()
    {
        CanPause = true;
    }

    #endregion
    #region Enumerators

    /*
    public IEnumerator LoadScene(string sceneName)
    {
        _fadeScreen.gameObject.SetActive(true);
        IsFading = true;
        _fadeScreen.FadeOut();
        yield return new WaitForSeconds(_fadeScreen.FadeDuration);
        IsFading = false;

        if (sceneName == "MainGameScene")
        {
            SceneManager.UnloadSceneAsync("TrainingScene");
            SceneManager.LoadSceneAsync("MainGameScene", LoadSceneMode.Additive);
        }
        else if (sceneName == "TrainingScene")
        {
            SceneManager.UnloadSceneAsync("MainGameScene");
            SceneManager.LoadSceneAsync("TrainingScene", LoadSceneMode.Additive);
        }
        else
        {
            SoundManager.Instance.PlaySound("wrong");
            Debug.LogError("Wrong scene named!");
        }

        IsFading = true;
        _fadeScreen.FadeIn();
        yield return new WaitForSeconds(_fadeScreen.FadeDuration);
        IsFading = false;
    }
    */
        
    #endregion

}
