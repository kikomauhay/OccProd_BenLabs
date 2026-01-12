using System.Collections;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    #region Members

    public float FadeDuration => _fadeDuration;
    private Renderer _rend;

    [Header("Fade Settings")]
    [SerializeField] private bool _fadeOnStart;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Color _fadeColor;
    
    #endregion

    #region Methods

    private void Awake() => _rend = GetComponent<Renderer>();
    private void Start()
    {        
        if (_fadeOnStart) 
            FadeIn();
    }

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
                _rend.material.SetColor("_Color", newColor);

                // changes opacity per frame
                timer += Time.deltaTime;
                yield return null;
            }

            Color col = _fadeColor;
            col.a = alphaOut;
            _rend.material.SetColor("_Color", col); // resets the transparency back to normal
            // gameObject.SetActive(false);
        }

        StartCoroutine(CO_Fade(alphaIn, alphaOut));
    }

    #endregion
}