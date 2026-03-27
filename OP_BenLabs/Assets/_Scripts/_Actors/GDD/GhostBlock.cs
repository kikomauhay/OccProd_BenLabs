using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer), typeof(SoundEmitter))]
public class GhostBlock : Actor, IInteractable
{
    #region Properites

    public static int FilledBlocks { get; set; }

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
                case BlockType.Weapon:
                    _weaponType = cb.WeaponType;
                    _rend.material.color = Color.blue;
                    break;

                case BlockType.Modifier:
                    _modifier = cb.Modifier;
                    _rend.material.color = Color.yellow;

                    break;

                case BlockType.Enemy:
                    _enemyPrefab = cb.EnemyPrefab;
                    _rend.material.color = Color.red;
                    break;

                case BlockType.Nothing: break;
                default:                break;
            }

            _isEmpty = false;
            FilledBlocks++;
            
            WaveHandler.Instance.CheckRemainingBlocks();
        }

        if (other.GetComponent<CodeBlock>())
        {
            CodeBlock codeBlock = other.GetComponent<CodeBlock>();

            if (_allowedBlockType != codeBlock.CurrentBlockType)
            {
                a_logger.Log("BlockType mismatch!", a_isDevMode);
                return;
            }
            if (!_isEmpty)
            {
                a_logger.Log($"{this} is already occupied!", a_isDevMode);
                return;
            }

            _soundEmitter.PlaySound(_snapSFX);
            PassBlockInfo(codeBlock);

            codeBlock.transform.position = new Vector3(0f, 100f, 0f);
            codeBlock.gameObject.SetActive(false);
            // add poof sfx before destorying 

            a_logger.Log($"{name} is now occupied with type: {codeBlock.CurrentBlockType}!", TextColor.Yellow, a_isDevMode);
            a_logger.Log($"Filled blocks: {FilledBlocks}", a_isDevMode);
        }
    }

    #endregion

    #region Public

    public void INT_Interact() => _soundEmitter.PlaySound(_snapSFX);
    public void ResetBlock()
    {
        _rend.material.color = Color.gray;
        _isEmpty = true;

        _weaponType = WeaponType.Default;
        _modifier = Modifier.Default;
        _enemyPrefab = null;

        a_logger.Log($"{name} has been reset!", a_isDevMode);
    }
    public void ShowInformation()
    {
        switch (_allowedBlockType)
        {
            case BlockType.Weapon: 
                a_logger.Log($"{name} contains", a_isDevMode);
                break;

            case BlockType.Enemy: 
                a_logger.Log($"{name} contains", a_isDevMode);
                break;

            case BlockType.Modifier:
                a_logger.Log($"{name} contains", a_isDevMode);
                break;

            case BlockType.Nothing: break;
            default:                break;
        }
    }


    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ShowInformation();
    }

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _isEmpty = true;
    }
        
    #endregion 
}
