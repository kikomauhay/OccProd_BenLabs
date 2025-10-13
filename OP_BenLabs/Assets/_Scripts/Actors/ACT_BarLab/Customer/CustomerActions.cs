using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomerAppearance), typeof(SoundEmitter))]
public class CustomerActions : MonoBehaviour
{
    #region SerializeField

    [Header("Sounds")]
    [SerializeField] private Sound _tipSFX;
    [SerializeField] private Sound _cheerSFX, _happySFX, _angrySFX;
        
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
    
    #endregion
    #region Enumerators

    public IEnumerator CO_Correct()
    {
        // play happy.sfx
        yield break;
    }
    public IEnumerator CO_Wrong()
    {
        // play angry.sfx
        yield break;
    }

        
    #endregion
}

