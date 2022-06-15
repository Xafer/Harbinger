using UnityEngine;
using UnityEditor;

public class SnapperTool
{
    // Start is called before the first frame update
    [MenuItem("Tool/Snap Selection")]
    public static void SnapSelection()
    {
        GameObject[] selection = Selection.gameObjects;

        if (selection.Length > 1 &&
            selection[0].GetComponent<SnapperGizmo>() != null &&
            selection[1].GetComponent<SnapperGizmo>() != null)
        {
            //SnapTogether(selection[0].transform, selection[1].transform);
        }
        else
            Debug.LogWarning("Could not snap!");
    }
}
