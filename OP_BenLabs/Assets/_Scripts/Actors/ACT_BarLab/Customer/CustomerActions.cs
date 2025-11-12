using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomerAppearance), typeof(SoundEmitter))]
public class CustomerActions : MonoBehaviour
{
    #region Properties

    public bool IsMale { get; set; }

    #endregion
    #region SerializeField

    [Header("Male Reactions")]
    [SerializeField] private Sound[] _happMaleSFXs;
    [SerializeField] private Sound[] _angryMaleSFXs;
    
    [Header("Female Reactions")]
    [SerializeField] private Sound[] _happyFemaleSFXs;
    [SerializeField] private Sound[] _angryFemaleSFXs;

    #endregion
    #region Private 

    private CustomerAppearance _appearance;
    private SoundEmitter _soundEmitter;
        
    #endregion
    
    #region Unity

    private void Awake() 
    {
        _appearance = GetComponent<CustomerAppearance>();
        _soundEmitter = GetComponent<SoundEmitter>();    
    } 
        
    #endregion
    #region Public
        
    public void Cheer()
    {
        // play cheer.sfx
    }
    public void TipBartender()
    {
        // play money.sfx
    }

    public void Correct()
    {
        _soundEmitter.PlaySound(IsMale ? 
                                _happMaleSFXs[Random.Range(0, _happMaleSFXs.Length)] :
                                _happyFemaleSFXs[Random.Range(0, _happyFemaleSFXs.Length)]);
    }
    public void Wrong()
    {
        _soundEmitter.PlaySound(IsMale ?
                               _angryMaleSFXs[Random.Range(0, _angryMaleSFXs.Length)] :
                               _angryFemaleSFXs[Random.Range(0, _angryFemaleSFXs.Length)]);
    }

    #endregion
    #region Enumerators

    public IEnumerator CO_Correct()
    {
        yield break;
    }
    public IEnumerator CO_Wrong()
    {
        // play angry.sfx
        yield break;
    }

        
    #endregion
}

