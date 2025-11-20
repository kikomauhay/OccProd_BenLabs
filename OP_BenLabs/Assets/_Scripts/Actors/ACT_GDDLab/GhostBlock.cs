using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer), typeof(SoundEmitter))]
public class GhostBlock : Actor, IInteractable
{
    #region Properites

    public static int FilledBlocks { get; private set; }

    public Modifier Modifier => _modifier;
    public WeaponType WeaponType => _weaponType;
    public GameObject EnemyPrefab => _enemyPrefab;

    #endregion
    #region SerializeField

    [Header("Block Stats")]
    [SerializeField] private BlockType _allowedBlockType;

    [Header("Block Content"), Tooltip("Serialized for testing")]
    [SerializeField] private Modifier _modifier;
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private GameObject _enemyPrefab;

    [Header("UI/UX")]
    [SerializeField] private Sound _snapSFX;

    #endregion
    #region Private

    private MeshRenderer _rend;
    private BoxCollider _boxCol;
    private SoundEmitter _soundEmitter;

    private bool _isEmpty;
        
    #endregion

    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        void PassBlockInfo(CodeBlock cb)
        {
            switch (cb.CurrentBlockType)
            {
                case BlockType.WEAPON:
                    _weaponType = cb.WeaponType;
                    _rend.material.color = Color.blue;

                    break;

                case BlockType.MODIFIER:
                    _modifier = cb.Modifier;
                    _rend.material.color = Color.yellow;

                    break;

                case BlockType.ENEMY:
                    _enemyPrefab = cb.EnemyPrefab;
                    _rend.material.color = Color.red;
                    break;

                case BlockType.NOTHING: break;
                default:                break;
            }

            _isEmpty = false;
            FilledBlocks++;
            
            GDDManager.Instance.RemoveBlock(cb);
            WaveHandler.Instance.CheckRemainingBlocks();
        }

        if (other.GetComponent<CodeBlock>())
        {
            CodeBlock codeBlock = other.GetComponent<CodeBlock>();

            if (_allowedBlockType != codeBlock.CurrentBlockType)
            {
                _logger.Log("BlockType mismatch!", _isDevMode);
                return;
            }
            if (!_isEmpty)
            {
                _logger.Log($"{this} is already occupied!", _isDevMode);
                return;
            }

            _soundEmitter.PlaySound(_snapSFX);
            PassBlockInfo(codeBlock);
            Destroy(codeBlock.gameObject); // add poof sfx before destorying 

            _logger.Log($"{name} is now occupied with type: {codeBlock.CurrentBlockType}!", TextColor.YELLOW, _isDevMode);
            _logger.Log($"Filled blocks: {FilledBlocks}", _isDevMode);
        }          
    }

    #endregion

    #region Public

    public void INT_Interact() => _soundEmitter.PlaySound(_snapSFX);

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _boxCol = GetComponent<BoxCollider>();
        _rend = GetComponent<MeshRenderer>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _isEmpty = true;
    }
        
    #endregion 
}
