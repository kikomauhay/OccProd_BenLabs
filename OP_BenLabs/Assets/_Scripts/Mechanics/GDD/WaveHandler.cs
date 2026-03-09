using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region Members

    public GhostBlock[] GhostBlocks => _ghostBlocks;
    [SerializeField] private GhostBlock[] _ghostBlocks;

    private const int GHOST_BLOCK_COUNT = 6;

    #endregion
    #region Methods

    protected override void AssertReferences()
    {
        a_logger.AssertCollection(_ghostBlocks, this);
    }
   
    public void CheckRemainingBlocks()
    {
        if (GhostBlock.FilledBlocks == GHOST_BLOCK_COUNT)
            GDDManager.Instance.EnableButtons(true);

        a_logger.Log("Enabled confirm buttons!", a_isDevMode);
    }

    #endregion
}