using UnityEngine;

public class Floor : MonoBehaviour 
{
    public FloorType FloorType => _floorType;
    [SerializeField] private FloorType _floorType;
}

[System.Serializable]
public enum FloorType
{
    LOBBY = 0,
    GDD = 1,
    BAR = 2
};
