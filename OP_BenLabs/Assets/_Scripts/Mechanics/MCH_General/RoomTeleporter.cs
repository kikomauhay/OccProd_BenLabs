using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    [Space(10f), SerializeField] private Transform _lobbyWaypoint;
    private GameManager _gameMgr;

    #endregion

    #region Methods

    private void Start()
    {
        Debug.Assert(_lobbyWaypoint, "Missing _lobbyWaypoint reference!", gameObject);
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW);

        _gameMgr = GameManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _gameMgr.Player)
        {
            _gameMgr.TeleportPlayer(_lobbyWaypoint.position, false);

            if (_isDevMode)
                _logger.Log("Teleported player to the Lobby!");
        }
    }

    #endregion
}
