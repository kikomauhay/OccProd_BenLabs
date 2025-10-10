using System.Collections;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    #region Properties

    public float FadeDuration => _fadeDuration;

    #endregion
    #region SerializeField 

    [Header("Debugging")]
    [SerializeField] private Logger _logger;
    [SerializeField] private bool _isDevMode;

    [Header("Fade Settings")]
    [SerializeField] private bool _fadeOnStart;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Color _fadeColor;

    #endregion
    #region Private

    private Renderer _renderer;
    
    #endregion

    #region Unity

    private void Awake() => _renderer = GetComponent<Renderer>();
    private void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, ColorType.YELLOW);
        
        if (_fadeOnStart) 
            FadeIn();
    }

    #endregion
    #region Public

    public void FadeIn() => Fade(1f, 0f);
    public void FadeOut() => Fade(0f, 1f);

    #endregion
    #region Private

    private void Fade(float alphaIn, float alphaOut)
    {
        IEnumerator CO_Fade(float alphaIn, float alphaOut)
        {
            float timer = 0f;

            while (timer <= _fadeDuration)
            {
                // slowly sets the fade-in color 
                Color newColor = _fadeColor;
                newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / _fadeDuration);
                _renderer.material.SetColor("_Color", newColor);

                // changes opacity per frame
                timer += Time.deltaTime;
                yield return null;
            }

            Color col = _fadeColor;
            col.a = alphaOut;
            _renderer.material.SetColor("_Color", col); // resets the transparency back to normal
            gameObject.SetActive(false);
        }

        StartCoroutine(CO_Fade(alphaIn, alphaOut));
    }

    #endregion
}