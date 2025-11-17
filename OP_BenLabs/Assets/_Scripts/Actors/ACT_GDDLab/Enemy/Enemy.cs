using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundEmitter), typeof(Rigidbody))]
public class Enemy : Actor
{
    #region Properties

    public System.Action<Enemy> OnDeath { get; set; }
    public System.Action OnKilled { get; set; }

    #endregion
    #region SerializeField

    [Header("Enemy Stats")]
    [SerializeField] private EnemyType _enemyType;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _enemyHPTxt;
    [SerializeField] private Slider _enemyHPslider;

    [Header("SFX")]
    [SerializeField] private Sound[] _etbSFXs;
    [SerializeField] private Sound _ltbSFX;

    #endregion
    #region Private

    private GDDManager _gddMgr;

    private Rigidbody _rb;
    private SoundEmitter _soundEmitter;
    private Transform _goal, _testGoal;

    private float _currHP, _maxHP, _moveSpeed, _rotSpeed;

    private const float MINIMUM_DISTANCE = 0.2f;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();
        // start sine Y axis for floating movement 
        UI_UpdateHP();

        _soundEmitter.PlaySound(_etbSFXs[(int)_enemyType]);
    }
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
        if (Vector3.Distance(lookAtGoal, transform.position) > MINIMUM_DISTANCE)
        {
            Vector3.Lerp(transform.position, _goal.position, _moveSpeed * Time.deltaTime);
            transform.Translate(0f, 0f, _moveSpeed * Time.deltaTime);
        }
        else
        {
            _gddMgr.TakeDamage();
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Weapon>())
        {
            Weapon w = other.GetComponent<Weapon>();

            TakeDamage(w.Damage + w.DamageModifier);
            _logger.Log($"{this} took damage!", _isDevMode);
        }
    }
    private void OnDestroy()
    {
        OnDeath?.Invoke(this);

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
    
    private void UI_UpdateHP()
    {
        _enemyHPTxt.text = $"{_currHP}/{_maxHP}";
        _enemyHPslider.value = _currHP / _maxHP;
    }
    private void TakeDamage(float amt)
    {
        if (amt < 0f)
        {
            _logger.Log("Cannot deal negative daamge!", TextColor.RED, _isDevMode);
            return;
        }

        _currHP -= amt;
        _logger.Log($"Enemy's HP: {_currHP}", _isDevMode);
        UI_UpdateHP();

        if (_currHP < 1f)
        {
            _currHP = 0f;
            UI_UpdateHP();
            Destroy(gameObject);
        }
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) _soundEmitter.PlaySound(_etbSFXs[Random.Range(0, _etbSFXs.Length)]);
        if (Input.GetKeyDown(KeyCode.Alpha2)) _soundEmitter.PlaySound(_ltbSFX); 
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_etbSFXs.Length == 3, "Missing elements in _etbSFXs!", this);
        Debug.Assert(_ltbSFX, "Missing _ltbSFX reference!", this);
    }
    protected override void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _gddMgr = GDDManager.Instance;

        _rb.mass = 10f;
        _rb.angularDrag = 0f;
        _rb.useGravity = false;
        _rb.isKinematic = true;

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

        name = $"{_enemyType}";

        _currHP = _maxHP;
        _moveSpeed = Random.Range(2f, 4f);
        _rotSpeed = Random.Range(2f, 4f);   
    }

    #endregion
}

public enum EnemyType
{
    CRASHES = 0,
    SAVE_ERROR = 1,
    MISSING_TEXTURE = 2
}