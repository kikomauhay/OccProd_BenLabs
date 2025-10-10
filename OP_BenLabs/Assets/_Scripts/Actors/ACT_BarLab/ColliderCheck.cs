using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(SoundEmitter))]
public class ColliderCheck : Actor
{
    #region Properties

    public Customer CustomerOrder { get; set; }

    #endregion
    #region Private

    private Collider _collider;
    private SoundEmitter _soundEmitter;

    #endregion

    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        if (!CustomerOrder)
        {
            if (_isDevMode)
                _logger.Log("Missing CustomerOrder reference!", ColorType.RED);

            // SoundManager.Instance.PlaySound("wrong");
            return;
        }
        /*
        if (other.gameObject.GetComponent<Ingredient>() != null)
        {
            DoIngredientCollision(other.gameObject.GetComponent<Ingredient>());
            return;
        }

        NEW_Plate plate = other.gameObject.GetComponent<NEW_Plate>();
        NEW_Dish dish = other.gameObject.GetComponent<NEW_Dish>();

        // makes sure that you have both a PLATE & DISH script
        if (dish != null && plate != null)
        {
            if (!dish.HasFood) return;

            DoDishCollision(dish, plate);
            // Debug.LogWarning($"Collided with {other.gameObject.name}");
            // Debug.LogWarning("Finished dish collision!");

            if (GameManager.Instance.CurrentShift == GameShift.Training)
            {
                plate.Served();
                dish.DisableDish();
            }
        }
        */
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
    }

    protected override void Test()
    {
        if (!_isDevMode) return;
    
    }

    #endregion
}