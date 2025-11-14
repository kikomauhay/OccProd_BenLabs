using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region Properties

    public System.Action OnAllBlocksFilled { get; set; }
    public GhostBlock[] GhostBlocks => _ghostBlocks;

    #endregion
    #region SerializeField

    [SerializeField] private GhostBlock[] _ghostBlocks;

    #endregion

    #region Methods

    protected override void Start()
    {
        Debug.Assert(_ghostBlocks.Length == 9, "Missing elements in _ghostBlocks!", this);    
        base.Start();
    }

    public void CheckRemainingBlocks()
    {
        if (GhostBlock.FilledBlocks == 9)
            OnAllBlocksFilled?.Invoke();
    }

    #endregion
}
