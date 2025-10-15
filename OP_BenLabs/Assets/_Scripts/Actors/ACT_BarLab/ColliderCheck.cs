using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(BoxCollider), typeof(SoundEmitter))]
public class ColliderCheck : Actor
{
    #region Properties

    public Customer CustomerOrder { get; set; }
    public bool HasCustomer { get; private set; }

    #endregion
    #region SerializeField

    [Header("Sounds")]
    [SerializeField] private Sound _correctSFX;
    [SerializeField] private Sound _wrongSFX, _unsureSFX;
        
    #endregion
    #region Private

    private BarManager _barMgr;
    private BoxCollider _collider;
    private SoundEmitter _soundEmitter;

    #endregion

    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        void DoGlassCollision(Glass glass)
        {
            if (!glass.HasDrink)
            {
                if (_isDevMode)
                    _logger.Log($"The glass has nothing in it!", TextColor.RED);

                _soundEmitter.PlaySound(_unsureSFX);
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
            if (_isDevMode)
                _logger.Log("Missing CustomerOrder reference!", TextColor.RED);

            _soundEmitter.PlaySound(_unsureSFX);
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
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
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