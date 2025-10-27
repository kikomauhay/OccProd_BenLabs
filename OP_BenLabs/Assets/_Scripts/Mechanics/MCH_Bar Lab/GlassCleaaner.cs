using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class GlassCleaaner : MonoBehaviour
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
    
    private void Awake() => InitComponents();
    private void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW);
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

    private void InitComponents()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
    }

    #endregion
}
