using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Track))] // Replace "Track" with the class name of your track objects

public class TrackEditor : Editor
{
    private void OnSceneGUI()
    {
        Track track = (Track)target;
        Transform trackTransform = track.transform;

        // Define grid size (1 unit)
        float gridSize = 1f;

        // Get the current position
        Vector3 currentPosition = trackTransform.position;

        // Snap to grid
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(currentPosition.x / gridSize) * gridSize,
            Mathf.Round(currentPosition.y / gridSize) * gridSize,
            Mathf.Round(currentPosition.z / gridSize) * gridSize
        );

        // Apply the snapped position (but only if the position is different)
        if (currentPosition != snappedPosition)
        {
            Undo.RecordObject(trackTransform, "Move Track to Grid");
            trackTransform.position = snappedPosition;
        }

        // Optional: Display the snapped position in the scene view for visualization
        Handles.color = Color.green;
        Handles.DrawWireCube(snappedPosition, Vector3.one * 0.5f);

        // Refresh the scene view to reflect the changes
        SceneView.RepaintAll();
    }
}
