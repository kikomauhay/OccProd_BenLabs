using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
public class GhostBlock : Actor
{
    #region Properites

    public static int FilledBlocks { get; private set; }

    public Modifier Modifier => _modifier;
    public GameObject EnemyPrefab => _enemyPrefab;
    public string WeaponContent => _weaponContent;

    #endregion
    #region SerializeField

    [Header("Block Stats")]
    [SerializeField] private BlockType _allowedBlockType;

    [Header("Block Content"), Tooltip("Serialized for testing")]
    [SerializeField] private Modifier _modifier;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private string _weaponContent;

    #endregion
    #region Private

    private MeshRenderer _rend;
    private BoxCollider _boxCol;

    private bool _isEmpty;
        
    #endregion

    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        void LogError(string message)
        {
            if (_isDevMode)
                _logger.Log(message, TextColor.RED);
        }
        void PassBlockInfo(CodeBlock cb)
        {
            switch (cb.CurrentBlockType)
            {
                case BlockType.WEAPON:
                    _weaponContent = cb.WeaponContent;
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
                LogError("BlockType mismatch!");
                return;
            }
            if (!_isEmpty)
            {
                LogError($"{this} is already occupied!");
                return;
            }

            PassBlockInfo(codeBlock);
            Destroy(codeBlock.gameObject); // add poof sfx before destorying 

            if (_isDevMode)
            {
                _logger.Log($"{name} is now occupied with type: {codeBlock.CurrentBlockType}!", TextColor.YELLOW);
                _logger.Log($"Filled blocks: {FilledBlocks}");
            }
        }          
    }

    #endregion
    #region Helpers
    
    protected override void InitComponents()
    {
        _boxCol = GetComponent<BoxCollider>();
        _rend = GetComponent<MeshRenderer>();
    }
    protected override void InitVariables()
    {
        _isEmpty = true;
    }
        
    #endregion 
}
