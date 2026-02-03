using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Space))
            SetupCustomerBody();

        if (Input.GetKeyDown(KeyCode.Backspace))
            SetEmotion((Emotion)Random.Range(0, 3));
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_face, this);
        a_logger.AssertReference(_customerRenderer.Length == 4, this);
        a_logger.AssertReference(_customerFaces.Length == 3, this);        

        a_logger.AssertReference(_customerMaterials.Length == 3, this);
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

    public void SetupCustomerBody()
    {
        int i = Random.Range(0, _customerMaterials.Length);

        foreach (MeshRenderer rend in _customerRenderer) 
            rend.material = new Material(_customerMaterials[i]);
    }
    public void SetEmotion(Emotion type)
    {
        for (int i = 0; i < _customerFaces.Length; i++)
            _customerFaces[i].SetActive((int)type == i);
    }

    #endregion
}

#region Enumerations

public enum Emotion
{
    Neutral = 0,
    Happy = 1,
    Mad = 2
}

#endregion