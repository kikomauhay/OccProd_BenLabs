using UnityEngine;
using UnityEngine.UI;

public class Desktop : MonoBehaviour
{

    #region Members

    [SerializeField] private Transform _monitor;

    [Header("Desktop Wallpaper")]
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _specialWallpaper;
    [SerializeField] private Sprite[] _defaultWallpapers;

    #endregion

    private void Start()
    {
        _img.sprite = _specialWallpaper != null ? 
                      _specialWallpaper :
                      _defaultWallpapers[Random.Range(0, _defaultWallpapers.Length)]; 

        _monitor.localRotation = Quaternion.Euler(0f, Random.Range(-10f, 10f), 0f);
    }
}
