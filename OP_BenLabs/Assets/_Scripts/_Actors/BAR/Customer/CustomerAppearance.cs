using Unity.Burst.Intrinsics;
using UnityEngine;

[RequireComponent(typeof(CustomerActions))]
public class CustomerAppearance : Actor
{
    #region Members

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer _face;
    [SerializeField] private MeshRenderer[] _customerRenderer;

    [Header("Assets"), Tooltip("0 = Neutral, 1 = Happy, 2 = Angry")]
    [SerializeField] private GameObject[] _customerFaces;
    [SerializeField] private Material[] _customerMaterials;

    #endregion
    
    #region Actor

    protected override void Test()
    {   
        
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_face != null, this);
        a_logger.AssertCollection(_customerRenderer, this);
        a_logger.AssertCollection(_customerFaces, this);

        a_logger.AssertCollection(_customerMaterials, this);
    }
    #endregion
    #region Unity

    protected override void Start()
    {
        base.Start();
        SetEmotion(Emotion.Happy);
    }

    #endregion
    #region Public 

    public void SetupCustomerBody(bool isMale)
    {
        int i = isMale ? _customerMaterials.Length : Random.Range(0, _customerMaterials.Length - 1);

        foreach (Renderer rend in _customerRenderer) 
            rend.material = new(_customerMaterials[i]);
    }
    public void SetEmotion(Emotion type)
    {
        for (int i = 0; i < _customerFaces.Length; i++)
            _customerFaces[i].SetActive((int)type == i);
    }

    #endregion
}

public enum Emotion
{
    Neutral = 0,
    Happy = 1,
    Mad = 2
}