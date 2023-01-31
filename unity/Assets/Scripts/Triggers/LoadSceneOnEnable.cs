using UnityEngine;

public class LoadSceneOnEnable : MonoBehaviour
{
    public string SceneName;

    private void OnEnable()
    {
        StartCoroutine(Utility.LoadSceneCoroutine(SceneName));
    }
}