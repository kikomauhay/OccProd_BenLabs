using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(SoundEmitter))]
public class VoiceColliderBox : Actor
{
    #region Members
        
    [SerializeField] private Sound _onbSFX;
    
    private SoundEmitter _soundEmitter;
    private BoxCollider _col;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics") ||
            other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            _soundEmitter.PlaySound(_onbSFX);
            _col.enabled = false;
            GameManager.Instance.DisableLogo();
        }
    }

    protected override void AssertReferences()
    {
        Debug.Assert(_onbSFX, "Missing _onbSFX reference!", this);
    }
    protected override void InitComponents()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
        _col = GetComponent<BoxCollider>();
    }
    protected override void InitVariables()
    {
        _col.enabled = true;
        _col.isTrigger = true;
    }
}
