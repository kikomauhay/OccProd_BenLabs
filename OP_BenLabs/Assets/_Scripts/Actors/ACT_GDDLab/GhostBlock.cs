using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
public class GhostBlock : Actor
{
    #region Properites

    public static int FilledBlocks { get; private set; }

    public GameObject EnemyPrefab { get; set; }
    public Modifier Modifier { get; set; }
    public string WeaponContent { get; set; }


    #endregion
    #region Members

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
        
    #endregion 
}
