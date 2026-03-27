using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SoundEmitter))]
public class NPC : Actor
{
    #region Members

    [Header("Components")]
    [SerializeField] private MeshRenderer[] _meshRends;
    [SerializeField] private Sound _voiceLine;

    [Header("Subtitle Settings")]
    [SerializeField] private string[] _dialogues;
    [SerializeField] private float[] _dialogueTimings; // this will replace _gapTimer
    [SerializeField] private Sound[] _voiceLines;
    
    [Header("Subtitle UI")]
    [SerializeField] private GameObject _subtitleBox;
    [SerializeField] private TextMeshProUGUI _txtBox;

    private SoundEmitter _sndEmtr;
    private bool _voicePlaying;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnboardPlayer();
    }
    protected override void InitComponents()
    {
        _sndEmtr = GetComponent<SoundEmitter>();
    }
    protected override void AssertReferences()
    {
        a_logger.AssertCollection(_dialogues, this);
        // a_logger.AssertCollection(_dialogueTimings, this);
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
            yield return new WaitForSeconds(_voiceLine.Clip.length);

            _voicePlaying = false;
        }

        StartCoroutine(CO_Play());
        StartCoroutine(CreateSubtitles(_dialogueTimings[0], _dialogueTimings[1]));
    }
    public void OnboardPlayer()
    {
        IEnumerator CO_PlayVoiceWithSubtitles()
        {
            if (_voicePlaying)
            {
                a_logger.Log("Voice line is currently playing!", TextColor.Red, a_isDevMode);
                yield break;
            }

            _voicePlaying = true;
            _subtitleBox.SetActive(true);

            for (int i = 0; i < _dialogues.Length; i++)
            {
                _sndEmtr.StopSound();
                _sndEmtr.PlaySound(_voiceLines[i]);
                _txtBox.text = _dialogues[i];

                yield return new WaitForSeconds(_dialogueTimings[i]);
            }

            _voicePlaying = false;
            _subtitleBox.SetActive(false);
        }

        StartCoroutine(CO_PlayVoiceWithSubtitles());
    }


    #endregion
    #region Enumerators        

    private IEnumerator CreateSubtitles(float timer1, float timer2)
    {
        _subtitleBox.SetActive(true);               //Turns on canvas

        _txtBox.text = _dialogues[0];
        yield return new WaitForSeconds(timer1);
        _txtBox.text = _dialogues[1];
        yield return new WaitForSeconds(timer2);
        _subtitleBox.SetActive(false);              //Turns off canvas
    }

    #endregion
}
