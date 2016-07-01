/// <summary>
/// Model of a tile
/// </summary>
public class TileModel {

    public enum TERRAIN_TYPES {EMPTY, GRASS, PLAIN, MOUNTAIN, WATER };
    TERRAIN_TYPES terrainType;
    int textureVersion;

    public TileModel(TERRAIN_TYPES terrainType, int textureVersion) {
        this.terrainType = terrainType;
        this.textureVersion = textureVersion;
    }

    /// <summary>
    /// Get the texture position of the terrain type for the provided texture version
    /// </summary>
    /// <returns>The texture position in the texture image file</returns>
    public int getTextureIndex() {
        switch(textureVersion) {
            default:
                return 0;
            case 1:
                switch (terrainType) {
                    default:
                    case TERRAIN_TYPES.EMPTY:
                        return 0;
                    case TERRAIN_TYPES.GRASS:
                        return 1;
                    case TERRAIN_TYPES.MOUNTAIN:
                        return 2;
                    case TERRAIN_TYPES.PLAIN:
                        return 3;
                    case TERRAIN_TYPES.WATER:
                        return 4;
                }
        }
    }
}
