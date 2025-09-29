using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    #region Properties

    public GameObject[] Units => _units;
    public uint UnitCount => _unitCount;
    public float SpawnInterval => _spawnInterval; 

    #endregion
    #region SerializeField

    [SerializeField] private GameObject[] _units; // prefab/s to spawn
    [SerializeField] private uint _unitCount; // how many units will spawn in the wave
    [SerializeField] private float _spawnInterval; // how frequent will they spawn

    #endregion
}
