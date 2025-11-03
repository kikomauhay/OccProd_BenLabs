using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(MeshRenderer))]
public class CodeBlock : Actor
{
    #region Properties

    public bool IsEmpty => _isEmpty;
    public BlockType BlockType => _currBlockType;

    public GameObject EnemyPrefab => _enemyPrefab;
    public Modifier Modifier => _modifier;
    public string WeaponContent => _weaponContent;

    public static int FilledBlocks { get; private set; }

    #endregion
    #region SerializeField

    [Header("Block Stats")]
    [SerializeField] private bool _isGhostBlock;
    [SerializeField] private BlockType _currBlockType, _allowedBlockType;

    [Header("Block Content")]
    [SerializeField] private Modifier _modifier;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private string _weaponContent;
    #endregion
    #region Private

    private System.Action _onBlockFilled; 
    private GDDManager _gddMgr;

    private MeshRenderer _rend;
    private BoxCollider _boxCol;
    private Rigidbody _rb;

    private bool _isEmpty;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        if (_isGhostBlock)
            _onBlockFilled += WaveHandler.Instance.EVENT_CheckRemainingBlocks;
    }
    protected override void OnDisable()
    {
        if (_isGhostBlock)
            _onBlockFilled -= WaveHandler.Instance.EVENT_CheckRemainingBlocks;
    }
    protected override void Start()
    {
        base.Start();
        UpdateBlockColor();
    }
    private void OnTriggerEnter(Collider other)
    {
        // normal block -> ghost block
        if (!other.GetComponent<CodeBlock>()) return;

        if (!_isGhostBlock)
        {
            if (_isDevMode)
                _logger.Log($"{this} isn't a ghost block!", gameObject, TextColor.RED);

            return;
        }
        if (_isEmpty)
        {
            if (_isDevMode)
                _logger.Log($"{this} is alraedy occupied!", gameObject, TextColor.RED);

            return;
        }

        CodeBlock cb = other.GetComponent<CodeBlock>();

        if (cb.IsEmpty)
        {
            if (_isDevMode)
                _logger.Log($"{cb.name} is empty!", TextColor.RED);

            return;
        }
        if (cb.BlockType != _allowedBlockType)
        {
            if (_isDevMode)
                _logger.Log($"{cb.name} isn't the same type!", TextColor.RED);

            return;
        }

        _currBlockType = cb.BlockType;
        _isEmpty = false;
        _gddMgr.RemoveBlock(cb);
        FilledBlocks++;

        // add poof sfx
        UpdateBlockColor();
        Destroy(cb.gameObject);

        if (_isDevMode)
        {
            _logger.Log($"{this} is now occupied with type: {_currBlockType}!", TextColor.YELLOW);
            _logger.Log($"{this} has {FilledBlocks} filled blocks!", TextColor.GREEN);
        }
    }
    private void OnDestroy()
    {
        if (!_isGhostBlock)
            _gddMgr.RemoveBlock(this);
    }

    #endregion
    #region Private

    private void UpdateBlockColor()
    {
        switch (_currBlockType)
        {
            case BlockType.NOTHING:
                _rend.material.color = Color.gray;
                break;

            case BlockType.WEAPON:
                _rend.material.color = Color.blue;
                break;

            case BlockType.ENEMY:
                _rend.material.color = Color.red;
                break;

            case BlockType.MODIFIER:
                _rend.material.color = Color.yellow;
                break;

            default: break;
        }
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _boxCol = GetComponent<BoxCollider>();
        _rb = GetComponent<Rigidbody>();
    }
    protected override void InitVariables()
    {
        _gddMgr = GDDManager.Instance;
        FilledBlocks = 0;

        _rend.enabled = true;
        
        _boxCol.enabled = true;
        _boxCol.isTrigger = true;

        _rb.angularDrag = 0f;
        _rb.useGravity = false;
        _rb.isKinematic = true;

        _isEmpty = false;

        if (_isGhostBlock)
        {
            _currBlockType = BlockType.NOTHING;
            return;
        }

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
            default: break;
        }
    }

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isEmpty = true;
            _currBlockType = BlockType.NOTHING;
            UpdateBlockColor();
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