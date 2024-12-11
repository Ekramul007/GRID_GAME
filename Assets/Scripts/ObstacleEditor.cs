using UnityEditor;
using UnityEngine;

public class ObstacleEditor : EditorWindow
{
    private ObstacleData obstacleData;

    [MenuItem("Tools/Obstacle Editor")]
    public static void ShowWindow()
    {
        GetWindow<ObstacleEditor>("Obstacle Editor");
    }

    private void OnGUI()
    {
        if (obstacleData == null)
        {
            GUILayout.Label("Select Obstacle Data:");
            obstacleData = (ObstacleData)EditorGUILayout.ObjectField(obstacleData, typeof(ObstacleData), false);
            return;
        }

        GUILayout.Label("Obstacle Grid (10x10):", EditorStyles.boldLabel);

        for (int i = 0; i < 10; i++) // Rows
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 10; j++) // Columns
            {
                int index = i * 10 + j;
                obstacleData.blockedTiles[index] = GUILayout.Toggle(obstacleData.blockedTiles[index], "");
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(obstacleData);
            AssetDatabase.SaveAssets();
        }
    }
}
