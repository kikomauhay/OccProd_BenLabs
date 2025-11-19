using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomerAppearance : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] private bool _isDevMode;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer _face;
    [SerializeField] private MeshRenderer[] _customerRenderer;

    [Header("Assets"), Tooltip("0 = Neutral, 1 = Happy, 2 = Angry")]
    [SerializeField] private GameObject _customerFace;
    [SerializeField] private Sprite[] _faceSprites;
    [SerializeField] private Material[] _maleCustomerMaterials;
    [SerializeField] private Material _femaleCustomerMaterial;

    #endregion

    #region Unity

    private void Start()
    {
        Debug.Assert(_face, "Missing _face reference!");
        Debug.Assert(_customerRenderer.Length == 4, "Missing _customerRenderer elements!");
        Debug.Assert(_customerFace, "Missing _customerFace reference!");        

        Debug.Assert(_faceSprites.Length == 3, "Missing _faceSprites elements!");
        Debug.Assert(_maleCustomerMaterials.Length == 2, "Missing _maleCustomerMaterials elements!");
        Debug.Assert(_femaleCustomerMaterial, "Missing _femaleCustomerMaterial reference!");
    }
    private void Update()
    {
        if (_isDevMode) return;

        if (Input.GetKeyDown(KeyCode.Space))
            SetupCustomerBody(Random.value > 0.5f);

        if (Input.GetKeyDown(KeyCode.Backspace))
            SetEmotion((Emotion)Random.Range(0, 3));
    }

    #endregion
    #region Public 

    public void SetupCustomerBody(bool isMale)
    {
        int i = Random.Range(0, _maleCustomerMaterials.Length);

        foreach (MeshRenderer rend in _customerRenderer)
        {
            rend.material = new Material(isMale ? 
                                        _maleCustomerMaterials[i] :  
                                        _femaleCustomerMaterial);
        }
    }
    public void SetEmotion(Emotion type) => _face.sprite = _faceSprites[(int)type];

    #endregion
}

#region Enumerations

public enum Emotion
{
    NEUTRAL = 0,
    HAPPY = 1,
    MAD = 2
}

#endregion