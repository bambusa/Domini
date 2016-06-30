using System.Collections.Generic;

public class TerrainTypesModel {

    Dictionary<string, TileModel.TERRAIN_TYPES> terrainTypeMap;

	public TerrainTypesModel() {
        CreateTerrainTypeMap();
    }

    void CreateTerrainTypeMap() {
        terrainTypeMap = new Dictionary<string, TileModel.TERRAIN_TYPES>();
        terrainTypeMap.Add("00FF00", TileModel.TERRAIN_TYPES.GRASS);                // Green
        terrainTypeMap.Add("FFFF00", TileModel.TERRAIN_TYPES.PLAIN);                // Yellow
        terrainTypeMap.Add("683C11", TileModel.TERRAIN_TYPES.MOUNTAIN);             // Brown
        terrainTypeMap.Add("0000FF", TileModel.TERRAIN_TYPES.WATER);                // Blue
    }

    public TileModel.TERRAIN_TYPES getTerrainType(string hex) {
        TileModel.TERRAIN_TYPES value = TileModel.TERRAIN_TYPES.EMPTY; // Default value if key doesn't exist
        terrainTypeMap.TryGetValue(hex, out value);
        return value;
    }
}
