using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VoiceColliderBox : Actor
{
    #region Members
        
    [SerializeField] private string _onbSFX;

    private SoundManager _sndMgr;
    private BoxCollider _col;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics") ||
            other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            if (_sndMgr.OnboardingPlaying)
                _sndMgr.StopOnboarding();

            _sndMgr.PlayOnboarding(_onbSFX);
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
        _sndMgr = SoundManager.Instance;

        _col.enabled = true;
        _col.isTrigger = true;
    }
}
