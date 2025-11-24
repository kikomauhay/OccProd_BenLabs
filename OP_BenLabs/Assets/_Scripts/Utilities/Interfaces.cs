public interface IGameHandler
{
    void INT_BTN_StartGame();
    void INT_DoGameOver();
    void INT_ResetValues();
}

public interface IPourable
{
    void INT_CheckPourAngle();
    void INT_Pour();
}

public interface IInteractable
{
    void INT_Interact();
}
