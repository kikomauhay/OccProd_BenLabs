using UnityEngine;

public class TEMP_Teleporter : MonoBehaviour
{
    private Floor _floor;

    private void Start()
    {
        _floor = GetComponent<Floor>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != GameManager.Instance.Player) return;

        // tutorial -> lobby -> GDD -> BAR -> back to tutorial area 
        switch (_floor.FloorType)
        {
            case FloorType.TUTORIAL:
                TeleportPlayer(FloorHandler.Instance.Floors[1]);
                break;

            case FloorType.LOBBY:
                TeleportPlayer(FloorHandler.Instance.Floors[2]);
                break;

            case FloorType.GDD:
                TeleportPlayer(FloorHandler.Instance.Floors[3]);
                break;

            case FloorType.BAR:
                TeleportPlayer(FloorHandler.Instance.Floors[0]);
                break;

            default: break;
        }    
    }

    private void TeleportPlayer(Floor floor)
    {
        GameManager.Instance.Player.transform.position = floor.transform.position;
    }
}

