using UnityEngine;

[RequireComponent(typeof(StampCard))]
public class ID : Equipment
{
    #region Public
        
    public void ResetID()
    {
        ResetPosition();
    }

    #endregion
}
