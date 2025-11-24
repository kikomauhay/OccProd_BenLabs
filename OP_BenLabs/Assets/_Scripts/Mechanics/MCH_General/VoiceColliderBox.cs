using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VoiceColliderBox : Actor
{
    #region Members
        
    [SerializeField] private string _onbSFX;
    private BoxCollider _col;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics") ||
            other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            SoundManager.Instance.PlayOnboarding(_onbSFX);
            _col.enabled = false;
        }
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_onbSFX != string.Empty, "Missing _onbSFX reference!", this);
    }
    protected override void InitComponents()
    {
        _col = GetComponent<BoxCollider>();
    }
    protected override void InitVariables()
    {
        _col.enabled = true;
        _col.isTrigger = true;
    }
}
