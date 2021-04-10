using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Scene scene;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            SceneManager.LoadSceneAsync(scene.name);
        }
    }
}