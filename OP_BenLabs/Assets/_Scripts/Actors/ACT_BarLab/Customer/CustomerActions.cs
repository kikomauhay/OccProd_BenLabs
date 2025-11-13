using UnityEngine;

[RequireComponent(typeof(CustomerAppearance), typeof(SoundEmitter))]
public class CustomerActions : MonoBehaviour
{
    #region Members

    public bool IsMale { get; set; }

    [SerializeField] private Sound _moneySFX;

    [Header("Male Reactions")]
    [SerializeField] private Sound[] _happyMaleSFXs;
    [SerializeField] private Sound[] _angryMaleSFXs;

    [Header("Female Reactions")]
    [SerializeField] private Sound[] _happyFemaleSFXs;
    [SerializeField] private Sound[] _angryFemaleSFXs;

    private SoundEmitter _soundEmitter;

    #endregion
    #region Methods

    private void Awake()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    private void Start()
    {
        Debug.Assert(_happyMaleSFXs.Length != 0, "Missing elements in _happyMaleSFXs!", gameObject);
        Debug.Assert(_angryMaleSFXs.Length != 0, "Missing elements in _angryMaleSFXs!", gameObject);
        Debug.Assert(_happyFemaleSFXs.Length != 0, "Missing elements in _happyFemaleSFXs!", gameObject);
        Debug.Assert(_angryFemaleSFXs.Length != 0, "Missing elements in _angryFemaleSFXs!", gameObject);
        Debug.Assert(_moneySFX, "Missing _moneySFX reference!", gameObject);

        CorrectReaction();
    }

    public void CorrectReaction() // can also act as the customer's ETB sound
    {
        _soundEmitter.PlaySound(IsMale ?
                                _happyMaleSFXs[Random.Range(0, _happyMaleSFXs.Length)] :
                                _happyFemaleSFXs[Random.Range(0, _happyFemaleSFXs.Length)]);
    }
    public void WrongReaction()
    {
        _soundEmitter.PlaySound(IsMale ?
                                _angryMaleSFXs[Random.Range(0, _angryMaleSFXs.Length)] :
                                _angryFemaleSFXs[Random.Range(0, _angryFemaleSFXs.Length)]);
    }
    public void TipBartender() => _soundEmitter.PlaySound(_moneySFX);

    #endregion
}