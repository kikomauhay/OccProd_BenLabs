using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(SoundEmitter))]
public class Enemy : Actor
{
    #region Properties

    public System.Action OnDeath { get; set; }

    #endregion
    #region SerializeField

    [Header("Enemy Stats")]
    [SerializeField] private float _currHP;
    [SerializeField] private float _moveSpeed;

    #endregion
    #region Private

    private NavMeshAgent _agent;
    private SoundEmitter _soundEmitter;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start(); // already contains both init methods


    }
    private void OnDestroy()
    {
        GDDManager.Instance.RemoveEnemy(gameObject);
        OnDeath?.Invoke();
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _agent = GetComponent<NavMeshAgent>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {

    }

    #endregion
}
