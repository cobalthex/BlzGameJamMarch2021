using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    public string SceneName;

    //private void Start()
    //{
    //    if (string.IsNullOrEmpty(SceneName))
    //    {
    //        SceneName = SceneManager.GetActiveScene().name;
    //        Debug.Log($"Setting {name}'s scene change to {SceneName}");
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
            StartCoroutine(Utility.LoadSceneCoroutine(SceneName));

        enabled = false;
    }
}
