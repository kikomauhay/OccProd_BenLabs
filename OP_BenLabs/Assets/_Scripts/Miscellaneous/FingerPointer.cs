using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FingerPointer : Actor
{
    #region Members

    private const float DURATION = 0.5f;

    #endregion

    #region Methods

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Point();
    }
    
    public void Point()
    {
        IEnumerator CO_Point()
        {
            WaitForSeconds _pointTimer = new(5f);

            transform.DOMoveY(transform.position.y + 5f, DURATION).
                      SetEase(Ease.OutSine).
                      SetLoops(-1, LoopType.Yoyo);
            
            yield return _pointTimer;
            
            if (a_isDevMode)
            {
                a_logger.Log("Action done", a_isDevMode);
                transform.DOKill();
            }
            else gameObject.SetActive(false);
        }

        StartCoroutine(CO_Point());
    }

    #endregion
}

public enum Direction
{
    Up, Down, 
    Front, Back, 
    Left, Right
}