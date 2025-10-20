using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public interface IGameHandler
{
    void INT_BTN_StartGame();
    void INT_BTN_StartTutorial();

    void INT_DoGameOver();
    void INT_SpawnUnit();
}