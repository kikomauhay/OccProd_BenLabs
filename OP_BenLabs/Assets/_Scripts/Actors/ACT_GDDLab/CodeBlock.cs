using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(MeshRenderer))]
public class CodeBlock : Actor
{
    #region Properties

    public bool IsEmpty => _isEmpty;
    public BlockType BlockType => _currBlockType;

    #endregion
    #region SerializeField

    [Header("Block Stats")]
    [SerializeField] private bool _isGhostBlock;
    [SerializeField] private BlockType _currBlockType, _allowedBlockType;

    [Header("Block Content")]
    [SerializeField] private Modifier _modifier;
    [SerializeField] private GameObject _enemyPrefab, _weaponPrefab;

    #endregion
    #region Private

    private MeshRenderer _rend;
    private BoxCollider _boxCol;
    private Rigidbody _rb;

    private bool _isEmpty;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();
        UpdateBlockColor();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CodeBlock>()) // normal block -> ghost block
        {
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

            // add poof sfx
            UpdateBlockColor();
            Destroy(cb.gameObject);

            if (_isDevMode)
                _logger.Log($"{this} is now occupied with type: {_currBlockType}!", TextColor.YELLOW);
        }
    }

    #endregion
    #region Private

    private void UpdateBlockColor()
    {
        switch (_currBlockType)
        {
            case BlockType.EMPTY:
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
    #region Public

    public void DoAction()
    {
        if (!_isGhostBlock) return;

        switch (_currBlockType)
        {
            case BlockType.EMPTY: break;

            case BlockType.WEAPON: 
                break;

            case BlockType.ENEMY: 
                break;
            
            case BlockType.MODIFIER: 
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
        _rend.enabled = true;
        
        _boxCol.enabled = true;
        _boxCol.isTrigger = true;

        _rb.angularDrag = 0f;
        _rb.useGravity = false;
        _rb.isKinematic = true;

        _isEmpty = false;

        if (_allowedBlockType != BlockType.MODIFIER) 
            _modifier = Modifier.DEFAULT;
    }

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isEmpty = true;
            _currBlockType = BlockType.EMPTY;
            UpdateBlockColor();
        }
    }

    #endregion
}

public enum BlockType
{
    EMPTY = 0,
    WEAPON = 1,
    ENEMY = 2,
    MODIFIER = 3
}

public enum Modifier
{
    DEFAULT,
    DAMAGE,
    SPEED,
    HEALTH  
}