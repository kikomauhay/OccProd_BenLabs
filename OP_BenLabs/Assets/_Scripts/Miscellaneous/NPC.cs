using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class NPC : Actor
{
    #region Members
        
    public static System.Action<bool> OnVoicePlayed { get; set; } // not yet used

    [SerializeField] private MeshRenderer[] _meshRends;
    [SerializeField] private Sound _voiceLine;

    private SoundEmitter _sndEmtr;
    private bool _voicePlaying;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            TriggerVoiceLine();
    }
    protected override void InitComponents()
    {
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_meshRends.Length == 4, this);
        a_logger.AssertReference(_voiceLine != null, this);
    }
    protected override void InitVariables()
    {
        _voicePlaying = false;
    }
        
    #endregion
    #region Private

    public void TriggerVoiceLine()
    {
        if (_voicePlaying)
        {
            a_logger.Log("Voice line is currently playing!", TextColor.Red, a_isDevMode);
            return;
        }

        IEnumerator CO_Play()
        {
            _sndEmtr.PlaySound(_voiceLine);
            _voicePlaying = true;
            OnVoicePlayed?.Invoke(_voicePlaying);
            yield return new WaitForSeconds(_voiceLine.Clip.length);

            _voicePlaying = false;
            OnVoicePlayed?.Invoke(_voicePlaying);
        }
        
        StartCoroutine(CO_Play());
    }
        
    #endregion
}
