using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderCheck : Actor
{
    #region Properties

    public Customer CustomerOrder { get; set; }
    public bool HasCustomer { get; private set; }

    #endregion
    #region SerializeField

    [Header("Components")]
    [SerializeField] private SoundEmitter _soundEmitter;

    [Header("Sounds")]
    [SerializeField] private Sound _correctSFX; 
    [SerializeField] private Sound _wrongSFX, _unsureSFX;

    #endregion
    #region Private

    private SoundManager _sndMgr;
    private BarManager _barMgr;

    private BoxCollider _collider;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", gameObject);
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        void DoGlassCollision(Glass glass)
        {
            if (!glass.HasDrink)
            {
                _logger.Log($"The glass has nothing in it!", TextColor.RED, _isDevMode);
                _sndMgr.PlaySound("SND_Unsure");
                return;
            }

            if (glass.Cocktail == CustomerOrder.WantedCocktail)
            {
                _soundEmitter.PlaySound(_correctSFX);
                _barMgr.Correct(glass.Score); 
            }
            else
            {
                _soundEmitter.PlaySound(_wrongSFX);
                _barMgr.Wrong();
            }

            Destroy(glass.gameObject); // test
            Destroy(CustomerOrder.gameObject);
        }

        if (!CustomerOrder)
        {
            _logger.Log("Missing CustomerOrder reference!", TextColor.RED, _isDevMode);
            _sndMgr.PlaySound("SND_Unsure");
            return;
        }

        if (other.gameObject.GetComponent<Glass>())
            DoGlassCollision(other.gameObject.GetComponent<Glass>());
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _collider = GetComponent<BoxCollider>();
    }
    protected override void InitVariables()
    {
        _sndMgr = SoundManager.Instance;
        _barMgr = BarManager.Instance;

        _collider.isTrigger = true;
        _collider.enabled = true;
        HasCustomer = false;
    }

    protected override void Test()
    {
        if (!_isDevMode) return;
    
        if (Input.GetKeyDown(KeyCode.C)) _soundEmitter.PlaySound(_correctSFX);
        if (Input.GetKeyDown(KeyCode.W)) _soundEmitter.PlaySound(_wrongSFX);
        if (Input.GetKeyDown(KeyCode.U)) _soundEmitter.PlaySound(_unsureSFX);
    }

    #endregion
}