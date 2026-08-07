using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(BVHExporter))]
public class BVHExporterEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BVHExporter bvhExporter = (BVHExporter)target;

        if (GUILayout.Button("Detect bones")) {
            bvhExporter.getBones();
            Debug.Log("Bone detection done.");
        }

        if (GUILayout.Button("Remove empty entries from bone list")) {
            bvhExporter.cleanupBones();
            Debug.Log("Cleaned up bones.");
        }

        if (GUILayout.Button("Clear recorded motion data")) {
            bvhExporter.clearCapture();
            Debug.Log("Cleared motion data.");
        }

        if (GUILayout.Button("Save motion to BVH file")) {
            try {
                bvhExporter.saveBVH();
            } catch (Exception ex) {
                Debug.LogError("An error has occurred while saving the BVH file: " + ex);
            }
        }
    }
}
