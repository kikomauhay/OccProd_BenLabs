using UnityEngine;

public class WaveHandler : StaticInstance<WaveHandler>
{
    #region Members

    public GhostBlock[] GhostBlocks => _ghostBlocks;

    [SerializeField] private GhostBlock[] _ghostBlocks;

    private GDDManager _gddMgr;

    #endregion

    #region Methods

    protected override void Start()
    {
        _gddMgr = GDDManager.Instance;

        Debug.Assert(_ghostBlocks.Length == 9, "Missing elements in _ghostBlocks!", this);
        base.Start();
    }

    public void CheckRemainingBlocks()
    {
        if (GhostBlock.FilledBlocks == 9)
            _gddMgr.EnableButtons(true);
    }

    #endregion
}
