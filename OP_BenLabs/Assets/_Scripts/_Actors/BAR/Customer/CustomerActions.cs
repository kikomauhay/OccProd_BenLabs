using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class CustomerActions : Actor
{
    #region Members

    public bool IsMale { get; set; }

    [Header("Male Reactions")]
    [SerializeField] private Sound[] _happyMaleSFXs;
    [SerializeField] private Sound[] _angryMaleSFXs;

    [Header("Female Reactions")]
    [SerializeField] private Sound[] _happyFemaleSFXs;
    [SerializeField] private Sound[] _angryFemaleSFXs;

    private SoundEmitter _sndEmtr;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) DoCorrectReaction();
        if (Input.GetKeyDown(KeyCode.Alpha2)) DoWrongReaction();
    }
    protected override void InitComponents()
    {
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_happyMaleSFXs.Length != 0, this);
        a_logger.AssertReference(_angryMaleSFXs.Length != 0, this);
        a_logger.AssertReference(_happyFemaleSFXs.Length != 0, this);
        a_logger.AssertReference(_angryFemaleSFXs.Length != 0, this);
    }

    #endregion
    #region Unity

    protected override void Start()
    {
        base.Start();
        DoCorrectReaction();
    }
        
    #endregion
    #region Public
        
    public void DoCorrectReaction() => _sndEmtr.PlayRandomSound(IsMale ? _happyMaleSFXs : 
                                                                         _happyFemaleSFXs);
    public void DoWrongReaction() => _sndEmtr.PlayRandomSound(IsMale ? _happyMaleSFXs : 
                                                                       _happyFemaleSFXs);

    #endregion
}