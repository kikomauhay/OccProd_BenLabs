using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderCheck : Actor
{
    #region Properties

    public Customer CustomerOrder { get; set; }
    public bool HasCustomer { get; private set; }

    #endregion
    #region SerializeField

    [Header("SFX")]
    [SerializeField] private SoundEmitter _soundEmitter;

    #endregion
    #region Private

    private AudioManager _sndMgr;
    private BarManager _barMgr;

    private BoxCollider _collider;

    #endregion

    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        if (!CustomerOrder)
        {
            a_logger.Log("Missing CustomerOrder reference!", TextColor.Red, a_isDevMode);
            _sndMgr.PlaySound("SND_Unsure");
            return;
        }

        if (other.gameObject.GetComponent<Glass>())
        {
            Glass glass = other.gameObject.GetComponent<Glass>();
            CustomerActions actions = CustomerOrder.GetComponent<CustomerActions>();
            CustomerAppearance appearance = CustomerOrder.GetComponent<CustomerAppearance>();

            if (!glass.HasDrink)
            {
                a_logger.Log("The glass has nothing in it!", TextColor.Red, a_isDevMode);
                _sndMgr.PlaySound("SND_Unsure");
                return;
            }

            if (glass.Cocktail == CustomerOrder.WantedCocktail)
            {
                actions.CorrectReaction();
                appearance.SetEmotion(Emotion.Happy);

                _barMgr.Correct();

            }
            else
            {
                actions.WrongReaction();
                appearance.SetEmotion(Emotion.Mad);

                _barMgr.Wrong();
            }

            Destroy(other.gameObject);
            Destroy(CustomerOrder.gameObject);

            CustomerOrder = null;
        }
    }

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.C)) _sndMgr.PlaySound("SND_Correct");
        if (Input.GetKeyDown(KeyCode.W)) _sndMgr.PlaySound("SND_Wrong");
        if (Input.GetKeyDown(KeyCode.U)) _sndMgr.PlaySound("SND_Unsure");
    }

    protected override void InitComponents()
    {
        _collider = GetComponent<BoxCollider>();
    }
    protected override void AssertReferences()
    {
        Debug.Assert(_soundEmitter, "Missing _soundEmitter reference!", this);
    }
    protected override void InitVariables()
    {
        _sndMgr = AudioManager.Instance;
        _barMgr = BarManager.Instance;

        _collider.isTrigger = true;
        _collider.enabled = true;
        HasCustomer = false;
    }

    #endregion
}