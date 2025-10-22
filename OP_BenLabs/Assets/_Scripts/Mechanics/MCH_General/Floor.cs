using UnityEngine;

public class Floor : MonoBehaviour 
{
    public FloorType FloorType => _floorType;
    [SerializeField] private FloorType _floorType;
}

[System.Serializable]
public enum FloorType
{
    TUTORIAL = 0,
    LOBBY = 1,
    GDD = 2,
    BAR = 3
};
