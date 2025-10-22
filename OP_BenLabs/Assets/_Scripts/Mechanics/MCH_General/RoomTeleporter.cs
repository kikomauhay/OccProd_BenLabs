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

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW);

        _gameMgr = GameManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<XROrigin>())
        {
            StartCoroutine(_gameMgr.CO_Enter(FloorType.LOBBY));

            if (_isDevMode)
                _logger.Log("Teleported player to the Lobby!");
        }
        else
        {
            if (_isDevMode)
                _logger.Log("Player Not Detected!");
        }
    }

    #endregion
}
