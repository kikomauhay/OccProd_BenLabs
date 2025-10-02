using System.Collections;
using UnityEngine;

public class CustomerAppearance : MonoBehaviour 
{
    #region Members

    [Header("Customer Material Renderers")]
    [SerializeField] private SpriteRenderer _face;
    [SerializeField] private MeshRenderer _ears, _tail, _body;

    [Tooltip("0 = Calico, 1 = Siamese, 2 = Tabby, 3 = Torbie, 4 = Tuxedo")] 
    [SerializeField] private SkinVariant[] _skinVariants;

    [Header("Face Types")] 
    [SerializeField] private Sprite[] _reactionFaces; // 0 = neutral, 1 = happy, 2 = sus
    [SerializeField] private Sprite[] _chewingFaces;  // 0-1 = normal, 2-3 = sus
    [SerializeField] private Sprite[] _madFaces; // 0 = angry, 1 = angrier, 2 = angriest
                                                 // angry     = less than 50 pts in patience meter
                                                 // angrier   = got the wrong order 
                                                 // angriest  = customer lost all patience 
    #endregion
    
    #region Unity
    
    private void Start()
    {        
        int i = Random.Range(0, _skinVariants.Length);

        _body.material = _skinVariants[i].BodyMaterial;
        _ears.material = _skinVariants[i].EarVariants[Random.Range(0, _skinVariants[i].EarVariants.Length)];
        _tail.material = _skinVariants[i].TailVariants[Random.Range(0, _skinVariants[i].TailVariants.Length)];

        _face.sprite = _reactionFaces[0];
    }

    #endregion
    #region Customer Reactions 

    public void SetFacialEmotion(FaceVariant type)
    {
        switch (type)
        {
            case FaceVariant.NEUTRAL:
                _face.sprite = _reactionFaces[0];
                break;
            
            case FaceVariant.HAPPY:
                _face.sprite = _reactionFaces[1];
                break;

            case FaceVariant.SUS:
                _face.sprite = _reactionFaces[2];
                break;

            default: break;
        }
    }
    public void SetAngryEmotion(int type)
    {
        if (type < 0 || type > _madFaces.Length) 
        {
            Debug.LogError($"{type} was out of range!");
            return;
        }

        _face.sprite = _madFaces[type];
    }
    public IEnumerator DoChweing(float patienceRate) // I am not proud of this
    {
        yield return new WaitForSeconds(1f);

        float chewTime = 2f;

        if (patienceRate > 50) // is happy is a customer pateince meter or 50+
        {
            _face.sprite = _chewingFaces[0];    
            yield return new WaitForSeconds(chewTime);

            _face.sprite = _chewingFaces[1];
            yield return new WaitForSeconds(chewTime);

            _face.sprite = _chewingFaces[0];
            yield return new WaitForSeconds(chewTime);

            yield break;
        }

        _face.sprite = _chewingFaces[3];
        yield return new WaitForSeconds(chewTime);

        _face.sprite = _chewingFaces[4];
        yield return new WaitForSeconds(chewTime);

        _face.sprite = _chewingFaces[3];
        yield return new WaitForSeconds(chewTime);
    }

    #endregion
}

#region Structres

[System.Serializable]
public struct SkinVariant
{
    public Material BodyMaterial;
    public Material[] EarVariants, TailVariants; 
}

#endregion
#region Enumerations

    public enum FaceVariant
    {
        NEUTRAL,
        HAPPY,
        MAD,
        SUS
    }

#endregion