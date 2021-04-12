using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneName;

    //private void Start()
    //{
    //    if (string.IsNullOrEmpty(sceneName))
    //    {
    //        sceneName = SceneManager.GetActiveScene().name;
    //        Debug.Log($"Setting {name}'s scene change to {sceneName}");
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            Debug.Log("Changing scene to " + sceneName);
            StartCoroutine(loadScene());
        }
    }

    IEnumerator loadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}