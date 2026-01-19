using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private Transform _goal;

    private float _currHP, _maxHP, _moveSpeed, _rotSpeed;

    private const float MINIMUM_DISTANCE = 1f;

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

        Vector3 direction = lookAtGoal - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(direction),
                                              Time.deltaTime * _rotSpeed);

        if (Vector3.Distance(lookAtGoal, transform.position) > MINIMUM_DISTANCE)
        {
            Vector3.Lerp(transform.position, _goal.position, _moveSpeed * Time.deltaTime);
            transform.Translate(0f, 0f, _moveSpeed * Time.deltaTime);
        }
        else // enemy got too close and hit the player
        {
            _gddMgr.TakeDamage();
            _gddMgr.RemoveEnemy(gameObject);

            Destroy(gameObject);
        }
    }
    protected override void OnDisable()
    {
        OnDeath?.Invoke();

        if (_currHP == 0f)
            OnKilled?.Invoke();
        

        _gddMgr.UnbindEvents(this);
        a_logger.Log($"{name} is destoryed!", TextColor.Yellow, a_isDevMode);
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
    public void TakeDamage(float amt)
    {
        if (amt < 0f)
        {
            a_logger.Log("Cannot deal negative daamge!", TextColor.Red, a_isDevMode);
            return;
        }

        _currHP -= amt;
        a_logger.Log($"Enemy's HP: {_currHP}", a_isDevMode);

        UI_UpdateHP();

        if (_currHP < 1f)
        {
            _currHP = 0f;
            _gddMgr.RemoveEnemy(gameObject);
            _soundEmitter.PlaySound(_ltbSFX);

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
        _moveSpeed = Random.Range(2f, 2.5f);
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