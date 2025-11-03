using System.Collections;
using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region Members

    public System.Action OnBlocksFilled { get; set; }
    public CodeBlock[] GhostBlocks => _ghostBlocks;

    [SerializeField] private CodeBlock[] _ghostBlocks;

    #endregion

    #region Methods

    protected override void Start()
    {
        Debug.Assert(_ghostBlocks.Length == 9, "Missing elements in _ghostBlocks!", gameObject);
        base.Start();
    }

    public void EVENT_CheckRemainingBlocks()
    {
        if (CodeBlocks.FilledBlocks == 9)
            OnBlocksFilled?.Invoke();
    }

    #endregion
}
