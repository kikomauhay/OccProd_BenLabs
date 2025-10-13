using UnityEngine;

[RequireComponent(typeof(SoundEmitter), typeof(Rigidbody))]
public class Enemy : Actor
{
    #region Properties

    public System.Action OnDeath { get; set; }
    public System.Action OnKilled { get; set; }

    #endregion
    #region SerializeField

    [Header("Enemy Stats")]
    [SerializeField] private float _maxHP;
    [SerializeField] private float _minDistance, _moveSpeed, _rotSpeed;

    #endregion
    #region Private

    private Rigidbody _rb;
    private SoundEmitter _soundEmitter;
    private Transform _goal, _testGoal;

    private float _currHP;

    #endregion

    #region Unity

    private void LateUpdate()
    {
    Vector3 lookAtGoal = new Vector3(_goal.position.x,
                                         transform.position.y,
                                         _goal.position.z);
        transform.LookAt(lookAtGoal);

        // smooth rotation
        Vector3 direction = lookAtGoal - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(direction),
                                              Time.deltaTime * _rotSpeed);

        // enemy travels to the goal (ignores Y-axis) 
        if (Vector3.Distance(lookAtGoal, transform.position) > _minDistance)
        {
            Vector3.Lerp(transform.position, _goal.position, _moveSpeed * Time.deltaTime);
            transform.Translate(0f, 0f, _moveSpeed * Time.deltaTime);
        }
        else 
        {
            GDDManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject); // test
        }
    }
    private void OnDestroy()
    {
        OnDeath?.Invoke();

        if (_currHP <= 0f)
            OnKilled?.Invoke();

        if (_isDevMode)
            _logger.Log($"{name} is destoryed!", TextColor.YELLOW);

        GDDManager.Instance.UnbindEvents(this);
    }

    #endregion
    #region Public

    public void SetGoal(Transform t) => _goal = t;
        
    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _logger = GDDManager.Instance.Logger;
        _rb = GetComponent<Rigidbody>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        name = "Enemy";
        _rb.mass = 10f;
        _rb.angularDrag = 0f;
        _rb.useGravity = true;
        _currHP = _maxHP;
    }

    protected override void Test()
    {
        if (!_isDevMode) return;
    }

    #endregion
}
