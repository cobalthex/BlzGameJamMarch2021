using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    public int LoseSceneChangeDelay;

    Transform win;
    Transform lose;

    void Awake()
    {
        win = transform.Find("Win");
        lose = transform.Find("Lose");
    }

    public void OnLose()
    {
        lose.gameObject.SetActive(true);
        StartCoroutine(LoseCoroutine());
    }

    public void OnWin()
    {
        win.gameObject.SetActive(true);
    }

    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSecondsRealtime(LoseSceneChangeDelay);
        yield return Utility.LoadSceneCoroutine(SceneManager.GetActiveScene().name);
    }

    public void BeginChangeScene(string sceneName)
    {
        StartCoroutine(Utility.LoadSceneCoroutine(sceneName));
    }
}
