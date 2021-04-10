using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Scene scene;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            StartCoroutine(loadScene());
        }
    }

    IEnumerator loadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.name);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}