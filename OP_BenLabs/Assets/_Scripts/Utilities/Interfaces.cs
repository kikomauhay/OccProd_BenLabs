public interface IGameHandler
{
    void INT_BTN_StartGame();
    void INT_BTN_StartTutorial();

    void INT_DoGameOver();
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
