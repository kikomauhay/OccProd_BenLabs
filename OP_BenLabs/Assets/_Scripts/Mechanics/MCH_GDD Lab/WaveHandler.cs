using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region Propeties

    public System.Action OnBlocksFilled { get; set; }
    public ReadOnlyArray<CodeBlock> GhostBlocks => _ghostBlocks;

    #endregion
    #region SerializeField

    [SerializeField] private CodeBlock[] _ghostBlocks;

    #endregion
    #region Private

    private GDDManager _gddMgr;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_ghostBlocks.Length != 9, "Missing elements in _ghostBlocks!", gameObject);
        base.Start();
    }

    #endregion
    #region Public

    public bool AllBlocksOccupied()
    {
        foreach (CodeBlock gb in _ghostBlocks)
        {
            if (gb.IsEmpty) break;

            return true;
        }
        
        return false;
    }

    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        _gddMgr = GDDManager.Instance;
    }
        
    #endregion
}
