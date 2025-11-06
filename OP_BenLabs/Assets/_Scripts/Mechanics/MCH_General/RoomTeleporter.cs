using Unity.XR.CoreUtils;
using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    private GameManager _gameMgr;

    #endregion

    #region Methods

    private void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);
        _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW, _isDevMode);

        _gameMgr = GameManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<XROrigin>())
        {
            StartCoroutine(_gameMgr.CO_Enter(FloorType.LOBBY));
            _logger.Log("Teleported player to the Lobby!", _isDevMode);
        }
        else _logger.Log("Player Not Detected!", _isDevMode);
    }

    #endregion
}
