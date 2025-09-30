using UnityEngine;

public class Customer : Actor
{
    #region SerializeField

    [Header("Customer UI")]
    [SerializeField] private GameObject[] _drinksUI;
    [SerializeField] private Material[] _faceMaterials;
        
    #endregion
    #region Private

    private Renderer _rend;
        
    #endregion

    #region Unity
        
    protected override void Start()
    {
        base.Start(); // already contains both Init methods


    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
    }
    protected override void InitVariables()
    {
        
    }

    #endregion

}
