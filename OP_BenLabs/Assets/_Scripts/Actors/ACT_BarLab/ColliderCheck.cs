using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(SoundEmitter))]
public class ColliderCheck : Actor
{
    #region Properties

    public Customer CustomerOrder { get; set; }
    public bool HasCustomer { get; private set; }

    #endregion
    #region SerializeField

    [SerializeField] private Sound _correctSFX, _wrongSFX;
        
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

                // play wrong.sfx
                return;
            }

            if (glass.Cocktail == CustomerOrder.WantedCocktail)
            {
                // _soundEmitter.PlaySound(_correctSFX);
                _barMgr.Correct(glass.Score); 
            }
            else
            {
                // _soundEmitter.PlaySound(_wrongSFX);s
                _barMgr.Wrong();
            }

            Destroy(glass.gameObject); // test
            Destroy(CustomerOrder.gameObject);
        }

        if (!CustomerOrder)
        {
            if (_isDevMode)
                _logger.Log("Missing CustomerOrder reference!", TextColor.RED);

            // play wrong.sfx
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
    
    }

    #endregion
}