using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Actor
{
    #region Members

    public int PoolCount => _pool.Count;

    [SerializeField] private GameObject[] _prefabs;
    private Queue<GameObject> _pool;

    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        a_logger.AssertCollection(_prefabs, this);
    }
    protected override void InitVariables()
    {
        _pool = new();
    }

    #endregion
    #region Public

    public GameObject Get(int i = 0)
    {
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);

            return obj;
        }
        return Instantiate(_prefabs[i]);
    }
    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
        
    #endregion
}
