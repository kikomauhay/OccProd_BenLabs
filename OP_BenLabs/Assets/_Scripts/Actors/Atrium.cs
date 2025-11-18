using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class Atrium : Actor, IInteractable
{
    #region Members

    [Header("SFX")]
    [SerializeField] private Sound[] _meowSFXs;
    [SerializeField] private Sound _purrSFX;
    
    private SoundEmitter _soundEmitter;

    #endregion

    public void INT_Interact()
    {
        IEnumerator CO_FunnyRotation()
        {
            _soundEmitter.PlaySound(_purrSFX);
            // tween.rotation
            yield return null; // null is temporary

            // poof sfx
            GameManager.Instance.AtriumActive = false;
            Destroy(gameObject);
        }

        StartCoroutine(CO_FunnyRotation());
    }

    protected override void Start()
    {
        base.Start();
        _soundEmitter.PlaySound(_meowSFXs[Random.Range(0, _meowSFXs.Length)]);
    }
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            INT_Interact();
        }
    }

    #region Helpers
        
    protected override void AssertComponents()
    {
        Debug.Assert(_meowSFXs.Length == 2, "Missing _meowSFX elements!", this);
        Debug.Assert(_purrSFX, "Missing _purrSFX reference!", this);
    }
    protected override void InitComponents()
    {    
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        
    }

    #endregion
}
