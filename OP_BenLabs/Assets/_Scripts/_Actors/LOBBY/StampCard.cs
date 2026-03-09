using UnityEngine;


public class StampCard : StaticInstance<StampCard>
{
    #region Members


    [SerializeField] public SoundEmitter _sndEmtr;
    [SerializeField] private GameObject[] _checkMarks;

    [Header("SFXs")]
    [SerializeField] private Sound _stampSFX;
        
    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_sndEmtr != null, this);
        a_logger.AssertCollection(_checkMarks, this);
        a_logger.AssertReference(_stampSFX != null, this);
    }

    #endregion
    #region Unity

    protected override void Start()
    {
        base.Start();

        foreach (GameObject checkMark in _checkMarks)
            checkMark.SetActive(false);
    }
        
    #endregion
    #region Public

    public void Stamp(int i) 
    {
        if (_checkMarks[i].activeSelf) return;

        _checkMarks[i].SetActive(true);
        _sndEmtr.PlaySound(_stampSFX);
    }
        
    #endregion
}
