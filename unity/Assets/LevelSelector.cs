using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    bool guiVisible;

    (string name, string path)[] availableLevels;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            guiVisible ^= true;
    }

    private void OnGUI()
    {
        if (!guiVisible)
            return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (availableLevels == null)
        {
            availableLevels = EditorBuildSettings.scenes
             .Where(scene => scene.enabled)
             .Select(scene => (System.IO.Path.GetFileNameWithoutExtension(scene.path), scene.path))
             .ToArray();
        }

        const int w = 400;
        const int h = 600;
        using var levelSelector = new GUILayout.AreaScope(new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h));
        GUILayout.Label("Select a level");

        using (var levels = new GUILayout.VerticalScope())
        {
            foreach (var level in availableLevels)
            {
                if (GUILayout.Button(level.name))
                {
                    guiVisible = false;
                    SceneManager.LoadScene(level.path, LoadSceneMode.Single);
                    break;
                }
            }
        }
    }
#endif
}
