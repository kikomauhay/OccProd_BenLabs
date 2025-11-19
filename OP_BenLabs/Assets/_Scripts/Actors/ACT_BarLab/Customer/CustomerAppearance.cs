using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomerAppearance : Actor
{
    #region Members

    [Header("Customer Material Renderers")]
    [SerializeField] private SpriteRenderer _face;
    [SerializeField] private MeshRenderer[] _customerRenderer;

    [Header("Customer Asserts"), Tooltip("0 = Neutral, 1 = Happy, 2 = Angry")]
    [SerializeField] private GameObject _customerFace;
    [SerializeField] private Sprite[] _faceSprites;
    [SerializeField] private Material[] _customerMaterials;

    private SpriteRenderer _spriteRend;

    #endregion

    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space)) InitVariables();
    }

    protected override void InitComponents()
    {
        _spriteRend = GetComponent<SpriteRenderer>();
    }
    protected override void AssertComponents()
    {
        Debug.Assert(_customerRenderer.Length == 4, "Missing _customerRenderer elements!");
        Debug.Assert(_faceSprites.Length == 3, "Missing _faceSprites elements!");
        Debug.Assert(_customerMaterials.Length == 3, "Missing _customerMaterials elements!");
    }
    protected override void InitVariables()
    {
        int i = Random.Range(0, _customerMaterials.Length);

        foreach (MeshRenderer rend in _customerRenderer)
            rend.material = new Material(_customerMaterials[i]);

        _spriteRend.sprite = _faceSprites[0];
    }

    #endregion
    #region Customer Reactions 

    public void SetFacialEmotion(FaceVariant type)
    {
        //switch (type)
        //{
        //    case FaceVariant.NEUTRAL:
        //        _face.sprite = _reactionFaces[0];
        //        break;

        //    case FaceVariant.HAPPY:
        //        _face.sprite = _reactionFaces[1];
        //        break;

        //    case FaceVariant.SUS:
        //        _face.sprite = _reactionFaces[2];
        //        break;

        //    default: break;
        //}
    }
    public void SetAngryEmotion(int type)
    {
        //if (type < 0 || type > _madFaces.Length) 
        //{
        //    Debug.LogError($"{type} was out of range!");
        //    return;
        //}

        //_face.sprite = _madFaces[type];
    }
    public IEnumerator DoChweing(float patienceRate) // I am not proud of this
    {
        yield return new WaitForSeconds(1f);

        //float chewTime = 2f;

        //if (patienceRate > 50) // is happy is a customer pateince meter or 50+
        //{
        //    _face.sprite = _chewingFaces[0];    
        //    yield return new WaitForSeconds(chewTime);

        //    _face.sprite = _chewingFaces[1];
        //    yield return new WaitForSeconds(chewTime);

        //    _face.sprite = _chewingFaces[0];
        //    yield return new WaitForSeconds(chewTime);

        //    yield break;
        //}

        //_face.sprite = _chewingFaces[3];
        //yield return new WaitForSeconds(chewTime);

        //_face.sprite = _chewingFaces[4];
        //yield return new WaitForSeconds(chewTime);

        //_face.sprite = _chewingFaces[3];
        //yield return new WaitForSeconds(chewTime);
    }

    #endregion
}

#region Enumerations

public enum FaceVariant
{
    NEUTRAL = 0,
    HAPPY = 1,
    MAD = 2,
    SUS = 3
}

#endregion