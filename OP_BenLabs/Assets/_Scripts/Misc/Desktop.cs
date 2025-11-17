using UnityEngine;
using UnityEngine.UI;

public class Desktop : MonoBehaviour
{

    #region Members

    [SerializeField] private Image _img;
    [SerializeField] private Sprite _specialWallpaper;
    [SerializeField] private Sprite[] _defaultWallpapers;

    #endregion

    private void Start()
    {
        _img.sprite = _specialWallpaper != null ? 
                      _specialWallpaper :
                      _defaultWallpapers[Random.Range(0, _defaultWallpapers.Length)]; 
    }
}
