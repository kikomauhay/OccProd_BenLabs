using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(SoundEmitter), typeof(Rigidbody))]
public class CodeBlock : Actor, IInteractable
{
    #region Properties

    public BlockType CurrentBlockType => _currBlockType;
    public Modifier Modifier => _modifier;
    public WeaponType WeaponType => _weaponType;
    public GameObject EnemyPrefab => _enemyPrefab;

    #endregion
    #region SerializeField

    [Header("Block Stats")]
    [SerializeField] private BlockType _currBlockType;

    [Header("Block Content")]
    [SerializeField] private Modifier _modifier;
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private GameObject _enemyPrefab;

    [Header("UI/UX")]
    [SerializeField] private Sound _grabSFX;

    #endregion
    #region Private

    private MeshRenderer _rend;
    private BoxCollider _boxCol;
    private Rigidbody _rb;
    private SoundEmitter _soundEmitter;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    
    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();
        
        switch (_currBlockType)
        {
            case BlockType.WEAPON:
                _rend.material.color = Color.blue;
                break;

            case BlockType.ENEMY:
                _rend.material.color = Color.red;
                break;

            case BlockType.MODIFIER:
                _rend.material.color = Color.yellow;
                break;

            case BlockType.NOTHING: break;
            default:                break;
        }
    }

    #endregion
    #region Public

    public void INT_Interact() => _soundEmitter.PlaySound(_grabSFX);
    public void ResetPosition()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _currBlockType = BlockType.NOTHING;

            switch (_currBlockType)
            {
                case BlockType.WEAPON:
                    _rend.material.color = Color.blue;
                    break;

                case BlockType.ENEMY:
                    _rend.material.color = Color.red;
                    break;

                case BlockType.MODIFIER:
                    _rend.material.color = Color.yellow;
                    break;

                case BlockType.NOTHING: break;
                default: break;
            }

            a_logger.Log($"{this} has been reset!", TextColor.Yellow, a_isDevMode);
        }
    }

    protected override void InitComponents()
    {
        _boxCol = GetComponent<BoxCollider>();
        _rend = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _soundEmitter = GetComponent<SoundEmitter>();

        base.InitComponents();
    }
    protected override void InitVariables()
    {        
        _rend.enabled = true;
        _boxCol.enabled = true;
        _boxCol.isTrigger = true;

        _rb.angularDrag = 0f;
        _rb.useGravity = false;
        _rb.isKinematic = true;

        switch (_currBlockType) // prevents overlaps of diffent block types
        {
            case BlockType.WEAPON:
                _enemyPrefab = null;
                _modifier = Modifier.DEFAULT;
                break;

            case BlockType.ENEMY:
                _weaponType = WeaponType.Default;
                _modifier = Modifier.DEFAULT;
                break;

            case BlockType.MODIFIER:
                _enemyPrefab = null;
                _weaponType = WeaponType.Default;
                break;

            case BlockType.NOTHING: break;
            default:                break;
        }

        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }    

    #endregion
}

public enum BlockType
{
    NOTHING = -1,
    WEAPON = 0,
    MODIFIER = 1,
    ENEMY = 2
}

public enum Modifier
{
    DEFAULT = 0,
    HEALTH = 1,
    DAMAGE = 2,
    REDUCED_ENEMIES = 3
}