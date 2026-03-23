using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class NPC : Actor
{
    #region Members
        
    public static System.Action<bool> OnVoicePlayed { get; set; } // not yet used

    [SerializeField] private MeshRenderer[] _meshRends;
    [SerializeField] private Sound _voiceLine;

    [Header("Subtitles Section")]
    [SerializeField] private string[] _dialogues;
    [SerializeField] private GameObject _subtitleBox;
    [SerializeField] private TextMeshProUGUI _txtBox;
    [SerializeField] private float _gapTimer = 0.2f;

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
        a_logger.AssertCollection(_meshRends, this);
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
        StartCoroutine(CreateSubtitles());
    }

    private IEnumerator CreateSubtitles()
    {
        _subtitleBox.SetActive(true);               //Turns on canvas

        _txtBox.text = _dialogues[0];
        yield return new WaitForSeconds(11f);
        _txtBox.text = _dialogues[1];
        yield return new WaitForSeconds(9f);
        _subtitleBox.SetActive(false);              //Turns off canvas
    }

    #endregion
}
