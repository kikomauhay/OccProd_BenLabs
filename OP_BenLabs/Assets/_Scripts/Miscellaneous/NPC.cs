using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class NPC : Actor
{
    #region Members
        
    [SerializeField] private MeshRenderer[] _meshRends;
    [SerializeField] private Sound _voiceLine;

    private SoundEmitter _sndEmtr;
    
    #endregion

    #region Actor
    
    protected override void InitComponents()
    {
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_meshRends.Length == 4, this);
        a_logger.AssertReference(_voiceLine, this);
    }
        
    #endregion
    #region Private

    public void TriggerVoiceLine()
    {
        _sndEmtr.PlaySound(_voiceLine);
        Debug.Log($"Voice Line: {_voiceLine.name}");
    }
        
    #endregion
}
