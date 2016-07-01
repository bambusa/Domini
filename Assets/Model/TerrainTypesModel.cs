using System.Collections.Generic;

/// <summary>
/// Building a Dictionary of terrain types and referencing color hex codes
/// </summary>
public class TerrainTypesModel {

    Dictionary<string, TileModel.TERRAIN_TYPES> terrainTypeMap;

	public TerrainTypesModel() {
        CreateTerrainTypeDictionary();
    }

    void CreateTerrainTypeDictionary() {
        terrainTypeMap = new Dictionary<string, TileModel.TERRAIN_TYPES>();
        terrainTypeMap.Add("00FF00", TileModel.TERRAIN_TYPES.GRASS);                // Green
        terrainTypeMap.Add("FFFF00", TileModel.TERRAIN_TYPES.PLAIN);                // Yellow
        terrainTypeMap.Add("683C11", TileModel.TERRAIN_TYPES.MOUNTAIN);             // Brown
        terrainTypeMap.Add("0000FF", TileModel.TERRAIN_TYPES.WATER);                // Blue
    }

    /// <summary>
    /// Get the TileModel.TERRAIN_TYPES of the color hex code
    /// </summary>
    /// <param name="hex">The color hex code of the tile</param>
    /// <returns>The TileModel.TERRAIN_TYPES out of the dictionary</returns>
    public TileModel.TERRAIN_TYPES getTerrainType(string hex) {
        TileModel.TERRAIN_TYPES value = TileModel.TERRAIN_TYPES.EMPTY; // Default value if key doesn't exist
        terrainTypeMap.TryGetValue(hex, out value);
        return value;
    }
}