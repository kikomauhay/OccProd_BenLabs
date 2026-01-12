using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region Members

    public GhostBlock[] GhostBlocks => _ghostBlocks;

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
            GDDManager.Instance.EnableButtons(true);

        a_logger.Log("Enabled confirm buttons!", a_isDevMode);
    }

    #endregion
}