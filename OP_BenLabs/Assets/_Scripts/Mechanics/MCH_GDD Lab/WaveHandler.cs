using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region SerializeField

    [SerializeField] private CodeBlock[] _codeBlocks;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_codeBlocks.Length != 9, "Missing elements in _codeBlocks!", gameObject);
        base.Start();
    }
        
    #endregion
}
