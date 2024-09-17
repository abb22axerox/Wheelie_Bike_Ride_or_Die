using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SceneViewReset
{
    // Define the home position and rotation
    private static readonly Vector3 HomePosition = new Vector3(0, 5, -10);
    private static readonly Quaternion HomeRotation = Quaternion.Euler(30, 0, 0);

    static SceneViewReset()
    {
        // Hook up a custom menu item
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        // Add a button to reset the Scene view
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 150, 50));
        if (GUILayout.Button("Reset Home View"))
        {
            ResetSceneView(sceneView);
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void ResetSceneView(SceneView sceneView)
    {
        // Set the Scene view camera to the home position and rotation
        sceneView.camera.transform.position = HomePosition;
        sceneView.camera.transform.rotation = HomeRotation;
        sceneView.Repaint();
    }
}
