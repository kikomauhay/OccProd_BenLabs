using UnityEngine;


public class StampCard : Actor
{
    #region Members

    public SoundEmitter SoundEmitter { get; set; }

    [SerializeField] private GameObject[] _checkMarks;

    [Header("SFXs")]
    [SerializeField] private Sound _stampSFX; 
        
    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_checkMarks.Length == 9, gameObject);
        a_logger.AssertReference(_stampSFX, gameObject);
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
        if (!_checkMarks[i].activeSelf)
        {
            _checkMarks[i].SetActive(true);
            SoundEmitter.PlaySound(_stampSFX);
        }
    }
        
    #endregion
}
