using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorButton : Actor
{
    #region Members

    public static Action OnButtonPressed { get; set; }

    [SerializeField] private Button _button;

    [Header("Tweening")]
    [SerializeField] private Vector3 _endpos;

    private const float CYCLE_LENGTH = 0.2f;
    private Vector3 _startPos;

    #endregion

    #region Unity

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<VR_Hands>() != null)
        {
            _button.onClick.Invoke();
            OnButtonPressed?.Invoke();
            
            PushButton();
        }
        else _logger.Log("NO INTERACTION", TextColor.RED, _isDevMode);
    }

    #endregion
    #region Helpers
    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushButton();
        }
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_button, "Missing _button reference!", gameObject);
        Debug.Assert(_endpos != Vector3.zero, "Missing _endpos reference!", gameObject);
        Debug.Assert(CYCLE_LENGTH != 0f, "Missing _cycleLength reference!", gameObject);
    }
    protected override void InitVariables()
    {
        _startPos = transform.localPosition;
    }

    private void PushButton()
    {
        IEnumerator CO_Push()
        {
            transform.DOLocalMove(_endpos, CYCLE_LENGTH);
            yield return new WaitForSeconds(0.2f);
            transform.DOLocalMove(_startPos, CYCLE_LENGTH);
        }

        StartCoroutine(CO_Push());
    }

    #endregion
}
