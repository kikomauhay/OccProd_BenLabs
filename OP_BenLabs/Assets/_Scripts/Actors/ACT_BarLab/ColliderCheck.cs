using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(SoundEmitter))]
public class ColliderCheck : Actor
{
    #region Properties

    public Customer CustomerOrder { get; set; }
    public bool HasCustomer { get; private set; }

    #endregion
    #region Private

    private BarManager _barMgr = BarManager.Instance;
    private Collider _collider;
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
                    _logger.Log($"The glass has nothing in it!", ColorType.RED);

                // play wrong.sfx
                return;
            }
            
            if (glass.Cocktail != CustomerOrder.WantedCocktail)
            {
                _barMgr.Wrong();
                return;
            }
            _barMgr.Correct(glass.Score);
        } 

        if (!CustomerOrder)
        {
            if (_isDevMode)
                _logger.Log("Missing CustomerOrder reference!", ColorType.RED);

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