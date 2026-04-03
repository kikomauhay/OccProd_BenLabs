using UnityEngine;
using UnityEngine.UI;

public class Desktop : MonoBehaviour
{
    #region Members

    [SerializeField] private Transform _monitor;

    [Header("Desktop Wallpaper")]
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _wallpaper;

    #endregion

    private void Start()
    {
        _img.sprite = _wallpaper; 
        // _monitor.localRotation = Quaternion.Euler(0f, Random.Range(-10f, 10f), 0f);
    }
}
