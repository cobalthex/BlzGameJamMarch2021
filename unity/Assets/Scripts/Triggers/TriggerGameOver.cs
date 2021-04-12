using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameOverType
{
    Lose,
    Win,
}

public class TriggerGameOver : MonoBehaviour
{
    public GameOverType Type = GameOverType.Lose;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.GetComponent<PlayerController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;

        GameObject uiObject = GameObject.FindGameObjectWithTag("UI");
        GameOverUIController uiController = uiObject.GetComponent<GameOverUIController>();
        uiController.BroadcastMessage(Type == GameOverType.Win ? "OnWin" : "OnLose");
    }
}
