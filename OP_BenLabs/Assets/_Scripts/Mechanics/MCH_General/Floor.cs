using UnityEngine;

public class Floor : MonoBehaviour 
{
    public FloorType FloorType => _floorType;
    [SerializeField] private FloorType _floorType;
}

[System.Serializable]
public enum FloorType
{
    Lobby = 0,
    GDD = 1,
    Cafeteria = 2,
    Bar = 3,
    Inaccesible = 4
};
