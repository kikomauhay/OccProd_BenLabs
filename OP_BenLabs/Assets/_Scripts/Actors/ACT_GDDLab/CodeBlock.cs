using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer), typeof(Rigidbody))]
public class CodeBlock : Actor
{
    #region Properties

    public BlockType CurrentBlockType => _currBlockType;
    public Modifier Modifier => _modifier;
    public GameObject EnemyPrefab => _enemyPrefab;
    public string WeaponContent => _weaponContent;

    #endregion
    #region SerializeField

    [Header("Block Stats")]
    [SerializeField] private BlockType _currBlockType;

    [Header("Block Content")]
    [SerializeField] private Modifier _modifier;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private string _weaponContent;

    #endregion
    #region Private

    private MeshRenderer _rend;
    private BoxCollider _boxCol;
    private Rigidbody _rb;

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
    #region Helpers

    protected override void InitComponents()
    {
        _boxCol = GetComponent<BoxCollider>();
        _rend = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
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
                _weaponContent = string.Empty;
                _modifier = Modifier.DEFAULT;
                break;

            case BlockType.MODIFIER:
                _enemyPrefab = null;
                _weaponContent = string.Empty;
                break;

            case BlockType.NOTHING: break;
            default:                break;
        }
    }

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
                default:                break;
            }

            _logger.Log($"{this} has been reset!", TextColor.YELLOW);
        }
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