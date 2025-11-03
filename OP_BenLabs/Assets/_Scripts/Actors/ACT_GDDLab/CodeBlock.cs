using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
public class CodeBlock : Actor
{
    #region Properties

    public System.Action OnBlockFilled { get; set; }
    public bool IsEmpty => _isEmpty;
    public BlockType CurrentBlockType { get; set; }

    public GameObject EnemyPrefab { get; set; }
    public Modifier Modifier { get; set; }
    public string WeaponContent { get; set; }


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

    private GDDManager _gddMgr;

    private MeshRenderer _rend;
    private BoxCollider _boxCol;
    private Rigidbody _rb;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();
        UpdateBlockColor();
    }
    private void OnTriggerEnter(Collider other)
    {
        void PassBlockInfo(GhostBlock cb)
        {
            switch (cb.CurrentBlockType)
            {
                case BlockType.WEAPON:
                    _weaponContent = cb.WeaponContent;
                    break;

                case BlockType.MODIFIER:
                    _modifier = cb.Modifier;
                    break;

                case BlockType.ENEMY:
                    _enemyPrefab = cb.EnemyPrefab;
                    break;

                case BlockType.NOTHING: break;
                default:                break;
            }

            IsEmpty = false;
            _currBlockType = cb.CurrentBlockType;
            _gddMgr.RemoveBlock(cb);

            FilledBlocks++;
            WaveHandler.Instance.CheckRemainingBlocks();
            UpdateBlockColor();
        }

        if (other.GetComponent<GhostBlock>())
        {
            GhostBlock codeBlock = other.GetComponent<GhostBlock>();

            if (_allowedBlockType != codeBlock.CurrentBlockType)
            {
                if (_isDevMode)
                    _logger.Log("BlockType mismatch!", TextColor.RED);

                return;
            }
            if (!IsEmpty)
            {
                if (_isDevMode)
                    _logger.Log($"{this} is alrady occupied!", TextColor.RED);

                return;
            }

            if (!codeBlock.IsGhostBlock && IsEmpty)
            {
                PassBlockInfo(codeBlock);
                Destroy(codeBlock.gameObject); // add poof sfx before destorying 

                if (_isDevMode)
                {
                    _logger.Log($"{name} is now occupied with type: {_currBlockType}!", TextColor.YELLOW);
                    _logger.Log($"{name} has {FilledBlocks} filled blocks!", TextColor.GREEN);
                }           
            }
        }          
    }
    private void OnDestroy()
    {
        if (!_isGhostBlock)
            _gddMgr.RemoveBlock(this);
    }

    #endregion
    #region Public

    public void UpdateBlockColor()
    {
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
        _rend = GetComponent<MeshRenderer>();
        _boxCol = GetComponent<BoxCollider>();

        _rb = _isGhostBlock ? null : GetComponent<Rigidbody>();
    }
    protected override void InitVariables()
    {
        _gddMgr = GDDManager.Instance;
        
        _rend.enabled = true;
        _boxCol.enabled = true;
        _boxCol.isTrigger = true;
        
        FilledBlocks = 0;
        IsEmpty = true;

        if (_isGhostBlock)
        {
            _currBlockType = BlockType.NOTHING;
            return;
        }
        else
        {
            _rb.angularDrag = 0f;
            _rb.useGravity = false;
            _rb.isKinematic = true;
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
            default:                break;
        }
    }

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsEmpty = true;
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