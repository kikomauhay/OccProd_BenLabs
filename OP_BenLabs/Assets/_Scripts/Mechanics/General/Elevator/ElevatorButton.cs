using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider), typeof(Button))]
public class ElevatorButton : Actor
{
    #region Members

    public static Action OnButtonPressed { get; set; }
    
    [Header("Tweening")]
    [SerializeField] private Vector3 _endpos;

    private const float CYCLE_LENGTH = 0.2f;    
    private Button _button;
    private BoxCollider _col;
    private Vector3 _startPos;

    #endregion
    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushButton();
            a_logger.Log("Button pushed", a_isDevMode);
        }
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_endpos != Vector3.zero, this);
        a_logger.AssertReference(CYCLE_LENGTH != 0f, this);
    }
    protected override void InitComponents()
    {
        _button = GetComponent<Button>();
        _col = GetComponent<BoxCollider>();
    }
    protected override void InitVariables()
    {
        _button.enabled = true;
        _col.enabled = true;
        _col.isTrigger = false;
        
        _startPos = transform.localPosition;
    }

    #endregion
    #region Unity

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<VR_Hands>() != null)
        {
            _button.onClick.Invoke();
            OnButtonPressed?.Invoke();
            
            PushButton();
        }
        else a_logger.Log("NO INTERACTION", TextColor.Red, a_isDevMode);
    }
    */

    #endregion

    public void PushButton()
    {
        IEnumerator CO_Push()
        {
            transform.DOLocalMove(_endpos, CYCLE_LENGTH);
            // _buttonTransform.DOLocalMove(_endpos, CYCLE_LENGTH);

            yield return new WaitForSeconds(0.2f);
         
            transform.DOLocalMove(_startPos, CYCLE_LENGTH);
            // _buttonTransform.DOLocalMove(_startPos, CYCLE_LENGTH);
        }

        StartCoroutine(CO_Push());
    }
}
