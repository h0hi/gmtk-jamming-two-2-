using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class EncounterAsset : ScriptableObject
{
    public TileRow[] tilemap;

    public int GetHeight() {
        if (tilemap != null && tilemap.Length > 0 && tilemap[0] != null) return tilemap[0].tiles.Length;
        return 0;
    }
    public int GetLength() {
        if (tilemap != null) return tilemap.Length;
        return 0;
    }

    public void SetSize(int l, int h) {
        tilemap = new TileRow[l];
        for (int i = 0; i < l; i++) {
            var tilerow = new TileRow();
            tilerow.SetSize(h);
            tilemap[i] = tilerow;
        }
    }

    public int GetTile(int x, int y) {
        return tilemap[x].tiles[y];
    }
    public void SetTile(int x, int y, int value) {
        tilemap[x].tiles[y] = value;
    }

    [System.Serializable]
    public class TileRow {
        public int[] tiles;

        public void SetSize(int l) {
            tiles = new int[l];
        }
    }
}
