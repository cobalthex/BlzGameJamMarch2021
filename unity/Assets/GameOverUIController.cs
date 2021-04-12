using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    public int SceneChangeDelay;

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
        StartCoroutine(SceneManager.GetActiveScene().name);
    }

    public void OnWin()
    {
        win.gameObject.SetActive(true);
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSecondsRealtime(SceneChangeDelay);
        yield return LoadSceneCoroutine(sceneName);
    }

    public void BeginChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
}
