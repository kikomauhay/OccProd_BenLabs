using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(MeshRenderer), typeof(Rigidbody), typeof(SoundEmitter))]
public class Enemy : Actor
{
    #region Properties

    public System.Action OnDeath { get; set; }
    public System.Action OnKilled { get; set; }

    public Transform Goal { get; set; }
    public EnemyType EnemyType { get; set; }

    #endregion
    #region Inspector

    [Header("Materials")]
    [SerializeField] private Material[] _materials;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _enemyHPTxt;
    [SerializeField] private Slider _enemyHPslider;

    [Header("SFX")]
    [SerializeField] private Sound[] _etbSFXs;
    [SerializeField] private Sound _ltbSFX;

    #endregion
    #region Private

    private const float MINIMUM_DISTANCE = 1f;
    private const int MAX_ENEMY_TYPES = 3;
    private const int VARIANT_COUNT = 3;
    
    private GDDManager _gddMgr;
    private MeshRenderer _rend;
    private Rigidbody _rb;
    private SoundEmitter _sndEmtr;
    private Vector3 _lookAtGoal, _direction;

    private float _currHP, _maxHP, _moveSpeed, _rotSpeed;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) _sndEmtr.PlayRandomSound(_etbSFXs);
        if (Input.GetKeyDown(KeyCode.Alpha2)) _sndEmtr.PlaySound(_ltbSFX);
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_materials.Length == VARIANT_COUNT, this);

        a_logger.AssertReference(_etbSFXs.Length == MAX_ENEMY_TYPES, this);
        a_logger.AssertReference(_ltbSFX != null, this);
    }
    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        name = EnemyType.ToString().Replace("_", " ") + " Enemy";
        
        _gddMgr = GDDManager.Instance;

        _rb.mass = 10f;
        _rb.angularDrag = 0f;
        _rb.useGravity = false;
        _rb.isKinematic = false;
        
        _rend.enabled = true;
        _rend.material = new Material(_materials[Random.Range(0, _materials.Length)]);

        _maxHP = EnemyType switch
        {
            EnemyType.Crashes => 32f,
            EnemyType.Save_Error => 22f,
            EnemyType.Missing_Textures => 14f,
            _ => 0f
        };

        _currHP = _maxHP;
        _moveSpeed = Random.Range(2f, 2.5f);
        _rotSpeed = Random.Range(2f, 4f);
    }

    #endregion
    #region Unity

    protected override void Start()
    {
        base.Start();
        UI_UpdateHP();

        _sndEmtr.PlaySound(_etbSFXs[(int)EnemyType]);
    }
    protected override void OnDisable()
    {
        OnDeath?.Invoke();

        if (_currHP == 0f)
            OnKilled?.Invoke();

        _gddMgr.UnbindEvents(this);
    }
    private void LateUpdate()
    {
        _lookAtGoal = new(Goal.position.x, transform.position.y, Goal.position.z);
        transform.LookAt(_lookAtGoal);

        _direction = _lookAtGoal - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(_direction),
                                              Time.deltaTime * _rotSpeed);

        if (_direction.sqrMagnitude > MINIMUM_DISTANCE * MINIMUM_DISTANCE)
        {
            Vector3.Lerp(transform.position, Goal.position, _moveSpeed * Time.deltaTime);
            transform.Translate(0f, 0f, _moveSpeed * Time.deltaTime);
        }
        else
        {
            _gddMgr.TakeDamage();
            _gddMgr.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    #endregion
    #region Methods

    public void TakeDamage(float amt)
    {
        if (amt < 0f)
        {
            a_logger.Log("Cannot deal negative daamge!", TextColor.Red, a_isDevMode);
            a_audMgr.PlayWrong();
            return;
        }

        _currHP -= amt;
        a_logger.Log($"Enemy's HP: {_currHP}", a_isDevMode);

        UI_UpdateHP();

        if (_currHP < 1f)
        {
            _currHP = 0f;
            _gddMgr.RemoveEnemy(gameObject);
            _sndEmtr.PlaySound(_ltbSFX);

            UI_UpdateHP();
            Destroy(gameObject);
        }
    }
    private void UI_UpdateHP()
    {
        _enemyHPTxt.text = $"{_currHP}/{_maxHP}";
        _enemyHPslider.value = _currHP / _maxHP;
    }

    #endregion    
}

public enum EnemyType
{
    Missing_Textures = 0,
    Save_Error = 1,
    Crashes = 2
}