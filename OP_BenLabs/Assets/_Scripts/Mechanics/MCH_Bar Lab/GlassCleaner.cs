using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class GlassCleaner : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    [Header("Sounds")]
    [SerializeField] private Sound _waterSplashSFX;
    private SoundEmitter _soundEmitter;

    #endregion

    #region Methods

    private void Awake() => _soundEmitter = GetComponent<SoundEmitter>();
    private void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);
        _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW, _isDevMode);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Glass>())
        {
            other.GetComponent<Glass>().Washed();
            _soundEmitter.PlaySound(_waterSplashSFX);
        }

        if (other.GetComponent<Shaker>())
        {
            other.GetComponent<Shaker>().Washed();
            _soundEmitter.PlaySound(_waterSplashSFX);
        }
    }

    #endregion
}
