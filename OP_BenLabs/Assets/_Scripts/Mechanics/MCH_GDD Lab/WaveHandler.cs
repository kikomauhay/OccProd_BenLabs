using System.Collections;
using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region SerializeField

    [SerializeField] private CodeBlock[] _codeBlocks;

    #endregion
    #region Private

    private GDDManager _gddMgr;

    #endregion

    #region Unity


    protected override void OnEnable()
    {
        IEnumerator CO_DelayedBinding()
        {
            yield return null;


        }
        
        StartCoroutine(CO_DelayedBinding());
    }
    protected override void OnDisable()
    {
        
    }
    
    protected override void Start()
    {
        Debug.Assert(_codeBlocks.Length != 9, "Missing elements in _codeBlocks!", gameObject);
        base.Start();
    }

    #endregion
    #region Private

    private void StartWave()
    {
        IEnumerator CO_StartWave()
        {



            yield break;
        }

        StartCoroutine(CO_StartWave());
    }


    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        _gddMgr = GDDManager.Instance;
    }
        
    #endregion
}
