using UnityEngine;

[RequireComponent(typeof(StampCard))]
public class ID : Equipment
{
    private StampCard _stampCard;

    #region Actor

    protected override void InitComponents()
    {
        base.InitComponents();

        _stampCard = GetComponent<StampCard>();
        _stampCard.SoundEmitter = e_sndEmitter;
    }

    #endregion
    #region Unity

    protected override void OnEnable()
    {
        // a_gameMgr
    }
    protected override void OnDisable()
    {
        
    }
        
    #endregion
    #region Public
        
    public void ResetID()
    {
        ResetPosition();
    }

    #endregion
}
