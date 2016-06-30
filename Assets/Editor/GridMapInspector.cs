using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GridMapGenerator))]
public class GridMapInspector : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if(GUILayout.Button("Regenerate")) {
            GridMapGenerator gridMap = (GridMapGenerator)target;
            gridMap.BuildMesh();
        }
    }
}
