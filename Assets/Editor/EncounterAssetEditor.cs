using UnityEngine;
using UnityEditor;

public class EncounterAssetEditor : EditorWindow
{
    private EncounterAsset editingAsset;
    private int length;
    private int height;

    private static readonly Color[] colormap = {
        new Color(1, 1, 1), // empty
        new Color(0, 0, 0), // indestructible block
        new Color(0.1f, 0.1f, 0), // destructible block,
        new Color(0.1f, 0.3f, 0.9f), // player spawn
        new Color(0.9f, 0.2f, 0f), // generic enemy
        new Color(0.9f, 0.1f, 0.4f) // turret enemy
    };

    [MenuItem("Window/EncounterAssetEditor")]
    public static void ShowWindow() {
        GetWindow<EncounterAssetEditor>();
    }

    private void OnGUI() {

        var guiEvent = Event.current;

        GUILayout.Label("Encounter Asset Editor", EditorStyles.boldLabel);
        editingAsset = (EncounterAsset)EditorGUILayout.ObjectField("Asset to edit: ", editingAsset, typeof(EncounterAsset), false);

        if (editingAsset) {
            length = EditorGUILayout.IntField("Tilemap width: ", editingAsset.GetLength());
            height = EditorGUILayout.IntField("Tilemap height: ", editingAsset.GetHeight());

            if (length != editingAsset.GetLength() || height != editingAsset.GetHeight()) {
                Undo.RecordObject(editingAsset, "Resize encounter tilemap");
                editingAsset.SetSize(length, height);
            }

            var texRect = new Rect(100, 100, 200, 200);

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
                var uvMousePos = (guiEvent.mousePosition - texRect.min) / texRect.size;
                
                var outOfBounds = uvMousePos.x < 0 || uvMousePos.x > 1 || uvMousePos.y < 0 || uvMousePos.y > 1;
                if (!outOfBounds) {
                    var tile = new Vector2Int(Mathf.FloorToInt(uvMousePos.x * length), Mathf.FloorToInt(uvMousePos.y * height));
                    tile.y = height - 1 - tile.y;

                    var value = (editingAsset.GetTile(tile.x, tile.y) + 1) % 6;
                    editingAsset.SetTile(tile.x, tile.y, value);
                    EditorUtility.SetDirty(editingAsset);
                    AssetDatabase.SaveAssetIfDirty(editingAsset);
                    AssetDatabase.Refresh();
                }

                Repaint();
            }

            if (length * height != 0) {
                var tex = BakeEncounterToTexture(editingAsset);
                EditorGUI.DrawPreviewTexture(texRect, tex);
            }
        }
    }

    private static Texture2D BakeEncounterToTexture(EncounterAsset asset) {
        var l = asset.GetLength();
        var h = asset.GetHeight(); 
        var tex = new Texture2D (
            l,
            h
        );

        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < l; x++) {
                tex.SetPixel(x, y, colormap[asset.GetTile(x, y)]);
            }
        }

        tex.Apply();
        return tex;
    }
}
