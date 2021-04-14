using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameOverType
{
    Lose,
    Win,
}

public static class Utility
{
    public static Vector3 VectorMultiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static bool InCamerasFrustum(Camera camera, Renderer renderer)
    {
        // calculate once per frame?
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }
    public static T GetChildComponentByName<T>(this Component parent, string name, bool recursive = true) where T : Component
    {
        foreach (T component in parent.GetComponentsInChildren<T>(true))
        {
            if (component.gameObject.name == name)
                return component;
        }
        return null;
    }
    public static IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
            yield return null;
    }

    public static void ActivateGameOver(GameOverType type)
    {
        var player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Picker>().enabled = false;
        Cursor.lockState = CursorLockMode.None;

        GameObject uiObject = GameObject.FindGameObjectWithTag("UI");
        GameOverUIController uiController = uiObject.GetComponent<GameOverUIController>();
        uiController.BroadcastMessage(type == GameOverType.Win ? "OnWin" : "OnLose");
    }
}
