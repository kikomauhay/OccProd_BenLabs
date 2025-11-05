using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorButtonInteraction : Actor
{
    #region Members

    [SerializeField] private Button _button;

    [Header("Tweening")]
    [SerializeField] private Vector3 _endpos;

    private const float CYCLE_LENGTH = 0.2f;
    private Vector3 _startPos;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_button, "Missing _button reference!", gameObject);
        Debug.Assert(_endpos != Vector3.zero, "Missing _endpos reference!", gameObject);
        Debug.Assert(CYCLE_LENGTH !=  0f, "Missing _cycleLength reference!", gameObject);

        base.Start();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<VR_Hands>() != null)
        {
            _button.onClick.Invoke();
            StartCoroutine(DisableButton());
        }
        else if (_isDevMode)
            _logger.Log("NO INTERACTION", TextColor.RED);
    }

    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        _startPos = transform.localPosition;
    }
    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushButton();
        }
    }

    private void PushButton()
    {
        IEnumerator CO_PushLogic()
        {
            transform.DOLocalMove(_endpos, CYCLE_LENGTH);
            yield return new WaitForSeconds(0.2f);
            transform.DOLocalMove(_startPos, CYCLE_LENGTH);
        }

        StartCoroutine(CO_PushLogic());
    }

    #endregion

    #region Enumerators

    private IEnumerator DisableButton()
    {
        _button.interactable = false;
        yield return new WaitForSeconds(3f);
        _button.interactable = true;
    }
    
    #endregion
}
