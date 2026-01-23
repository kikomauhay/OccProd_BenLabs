using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SoundEmitter), typeof(MeshRenderer))]
public class Equipment : Actor, IInteractable
{
    #region SerializeField

    [Header("SFX")]
    [SerializeField] protected Sound e_pickUpSFX;
    [SerializeField] protected Sound e_landOnFloorSFX;

    #endregion
    #region Protected

    protected MeshRenderer e_rend;
    protected Rigidbody e_rb;
    protected SoundEmitter e_sndEmitter;

    protected Vector3 e_startPosition;
    protected Quaternion e_startRotation;

    #endregion
    
    #region Actor

    protected override void AssertReferences()
    {
        // a_logger.AssertReference(e_pickUpSFX, this);
        // a_logger.AssertReference(e_landOnFloorSFX, this);
    }
    protected override void InitComponents()
    {
        e_rend = GetComponent<MeshRenderer>();
        e_rb = GetComponent<Rigidbody>();
        e_sndEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        e_rend.enabled = true;
        
        e_rb.angularDrag = 0f;
        e_rb.useGravity = true;

        e_startPosition = transform.position;
        e_startRotation = transform.rotation;
    }

    #endregion
    #region Public

    public virtual void INT_Interact()
    {
        if (e_pickUpSFX != null)
            e_sndEmitter.PlaySound(e_pickUpSFX);
    }
        
    #endregion
    #region Protected

    protected void ResetPosition() 
    {
        transform.SetPositionAndRotation(e_startPosition, e_startRotation);
        e_sndEmitter.PlaySound(e_landOnFloorSFX);

        a_logger.Log($"{name}'s position has been reset!", TextColor.Yellow, a_isDevMode);
    }

    #endregion
}
