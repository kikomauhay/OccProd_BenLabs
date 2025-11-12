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
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private float _minDistance; // testing

    [Header("UI/UX")]
    [SerializeField] private Sound[] _etbSFXs;
    [SerializeField] private Sound _ltbSFX;

    #endregion
    #region Private

    private GameManager _gameMgr;
    private GDDManager _gddMgr;

    private Rigidbody _rb;
    private SoundEmitter _soundEmitter;
    private Transform _goal, _testGoal;

    private float _maxHP, _currHP, _moveSpeed, _rotSpeed;

    #endregion

    #region Unity

    private void LateUpdate() => TravelToPlayer();
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Weapon>())
        {
            Weapon w = other.GetComponent<Weapon>();

            TakeDamage(w.Damage + w.DamageModifier);
            _logger.Log($"{this} took damage!", _isDevMode);
            
            return;
        }

        if (other.gameObject == _gameMgr.Player)
        {
            _gddMgr.TakeDamage();
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        OnDeath?.Invoke();

        if (_currHP == 0f)
            OnKilled?.Invoke();

        _gddMgr.UnbindEvents(this);
        _soundEmitter.PlaySound(_ltbSFX);
        _logger.Log($"{name} is destoryed!", TextColor.YELLOW, _isDevMode);
    }

    #endregion
    #region Public

    public void SetGoal(Transform t) => _goal = t;
        
    #endregion
    #region Private
    
    private void TakeDamage(float amt)
    {
        if (amt < 0f)
        {
            _logger.Log("Cannot deal negative daamge!", TextColor.RED, _isDevMode);
            return;
        }

        _currHP -= amt;
        _logger.Log($"Enemy's HP: {_currHP}", _isDevMode);

        if (_currHP < 1f)
        {
            _currHP = 0f;
            Destroy(gameObject);
        }
    }

    #endregion
    #region Helpers

    protected override void AssertComponents()
    {
        Debug.Assert(_etbSFXs.Length != 3, "Missing elements in _etbSFXs!", gameObject);
        Debug.Assert(_ltbSFX, "Missing _ltbSFX reference!", gameObject);
    }
    protected override void InitComponents()
    {
        _logger = GDDManager.Instance.Logger;
        _rb = GetComponent<Rigidbody>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        name = "Enemy";

        _gameMgr = GameManager.Instance;
        _gddMgr = GDDManager.Instance;

        _rb.mass = 10f;
        _rb.angularDrag = 0f;
        _rb.useGravity = true;

        switch (_enemyType)
        {
            case EnemyType.CRASHES:
                _maxHP = 10f;
                break;

            case EnemyType.SAVE_ERROR:
                _maxHP = 8f;
                break;

            case EnemyType.MISSING_TEXTURE:
                _maxHP = 2f;
                break;

            default: break;
        }

        _currHP = _maxHP;
        _moveSpeed = Random.Range(2f, 4f);
        _rotSpeed = Random.Range(2f, 4f);

        _soundEmitter.PlaySound(_etbSFXs[(int)_enemyType]);
    }

    private void TravelToPlayer()
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

    #endregion
}

public enum EnemyType
{
    CRASHES = 0,
    SAVE_ERROR = 1,
    MISSING_TEXTURE = 2
}