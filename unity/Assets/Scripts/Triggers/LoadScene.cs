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

    private void Start()
    {
        // required for enabling/disabling in inspector
    }

    void OnTriggerStay(Collider other)
    {
        if (!enabled)
            return;

        if (other.tag.Contains("Player"))
            StartCoroutine(Utility.LoadSceneCoroutine(sceneName));

        enabled = false;
    }
}