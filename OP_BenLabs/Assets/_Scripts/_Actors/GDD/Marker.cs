using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Marker : Actor
{
    public void ReleaseMarker()
    {
        // add a checker if there's a drawing in the white board

        StampCard.Instance.Stamp(1);
    }
}
