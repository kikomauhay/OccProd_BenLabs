using System;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter), typeof(CustomerAppearance))]
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
    private CustomerAppearance _appearance;

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
        _appearance = GetComponent<CustomerAppearance>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertCollection(_happyMaleSFXs, this);
        a_logger.AssertCollection(_angryMaleSFXs, this);
        a_logger.AssertCollection(_happyFemaleSFXs, this);
        a_logger.AssertCollection(_angryFemaleSFXs, this);
    }

    #endregion
    #region Unity

    protected override void Start()
    {
        base.Start();
        DoCorrectReaction();
        _appearance.SetupCustomerBody(IsMale);
    }
        
    #endregion
    #region Public
        
    public void DoCorrectReaction() => _sndEmtr.PlayRandomSound(IsMale ? _happyMaleSFXs : 
                                                                         _happyFemaleSFXs);
    public void DoWrongReaction() => _sndEmtr.PlayRandomSound(IsMale ? _happyMaleSFXs : 
                                                                       _happyFemaleSFXs);

    #endregion
}