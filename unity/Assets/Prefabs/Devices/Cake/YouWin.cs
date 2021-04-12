using UnityEngine;

public class YouWin : MonoBehaviour
{
    public void OnSwitchStateChanged()
    {
        Utility.ActivateGameOver(GameOverType.Win);
    }
}
