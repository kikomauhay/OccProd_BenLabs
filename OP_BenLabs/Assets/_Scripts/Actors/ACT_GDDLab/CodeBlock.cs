using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(tyepof(RigidBody))]
public class CodeBlock : Actor
{
    #region Properties

    public bool IsEmpty => _isEmpty;
    public BlockType BlockType => _blockType;

    #endregion
    #region SerializeField

    [SerializeField] private BlockType _blockType;
    [SerializeField] private BlockType _allowedBlockType;

    [SerializeField] private bool _isGhostBlock;

    #endregion
    #region Private

    private RigidBody _rb;

    private bool _isEmpty;

    #endregion

    #region Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CodeBlock>() && _isEmpty)
        {
            if (!_isGhostBlock) return;

            CodeBlock cb = other.GetComponent<CodeBlock>();

            if (cb.IsEmpty)
            {
                if (_isDevMode)
                    _logger.Log($"{cb.name} is empty!");

                return;
            }
            if (cb.BlockType != _allowedBlockType)
            {
                if (_isDevMode)
                    _logger.Log($"{cb.name} isn't the same type!");

                return;
            }

            _blockType = cb.BlockType;
            _isEmpty = false;
        }        
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _rb = GetComponent<RigidBody>();
    }
    protected override void InitVariables()
    {
        name = $"{this}";

        _rb.useGravity = false;
        _rb.enabled = !_isGhostBlock;

        _blockType = BlockType.EMPTY;
        _isEmpty = false;
    }

    private void SetColor()
    {
        switch (_blockType)
        {
            case BlockType.EMPTY:
                break;
            case BlockType.WEAPON:
                break;
            case BlockType.ENEMY:
                break;
            case BlockType.MODIFIER:
                break;
            default:
                break;
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