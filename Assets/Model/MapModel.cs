using System;
using UnityEngine;

/// <summary>
/// Model of the map
/// </summary>
public class MapModel {

    int width;
    int height;
    int textureVersion;
    TileModel[][] mapTiles;

    /// <summary>
    /// Default constructor - creating a random generated map with size 20 * 20
    /// </summary>
    public MapModel() {
        width = 10;
        height = 10;
        textureVersion = 1;
        GenerateRandomMapData();
    }

    /// <summary>
    /// Random constructor - creating a random generated map with provided size
    /// </summary>
    /// <param name="width">Width of the map as tile numbers</param>
    /// <param name="height">Height of the map as tile numbers</param>
    public MapModel(int width, int height, int textureVersion) {
        this.width = width;
        this.height = height;
        this.textureVersion = textureVersion;
        GenerateRandomMapData();
    }

    /// <summary>
    /// Map constructor - creating a map from provided map data
    /// </summary>
    /// <param name="hexCodes">Hex color codes, referencing terrain types</param>
    public MapModel(int width, int height, string[][] hexCodes, int textureVersion) {
        this.width = width;
        this.height = height;
        this.textureVersion = textureVersion;
        GenerateMapFromHexColors(hexCodes);
    }

    /// <summary>
    /// Generate a map from the provided map data
    /// Reading the Hex Color codes and creating defined terrain types for those
    /// </summary>
    /// <param name="hexCodes">Array if colors, representing position and terrain type in the map</param>
    void GenerateMapFromHexColors(string[][] hexCodes) {
        TerrainTypesModel terrainTypesModel = new TerrainTypesModel();
        mapTiles = new TileModel[width][];
        for (int x = 0; x < width; x++) {
            mapTiles[x] = new TileModel[height];
            for (int z = 0; z < height; z++) {
                Debug.Log("Found hex code #" + hexCodes[x][z]);
                mapTiles[x][z] = new TileModel(terrainTypesModel.getTerrainType(hexCodes[x][z]), textureVersion);
            }
        }
        
    }

    /// <summary>
    /// Generate a map with defined size an random terrain types
    /// </summary>
    void GenerateRandomMapData() {
        mapTiles = new TileModel[width][];
        for (int x = 0; x < width; x++) {
            mapTiles[x] = new TileModel[height];
            for (int z = 0; z < height; z++) {
                Array values = Enum.GetValues(typeof(TileModel.TERRAIN_TYPES));
                System.Random random = new System.Random();
                TileModel.TERRAIN_TYPES tT = (TileModel.TERRAIN_TYPES)values.GetValue(random.Next(values.Length));
                TileModel tile = new TileModel(tT, textureVersion);
                mapTiles[x][z] = tile;
            }
        }
    }

    /// <summary>
    /// Get the TileModel at the provided location
    /// </summary>
    /// <param name="x">X coordinate of the tile</param>
    /// <param name="z">Z coordinate of the tile</param>
    /// <returns></returns>
    public TileModel getTile(int x, int z) {
        if (x < 0 || x >= width || z < 0 || z >= height) {
            return null;
        }

        return mapTiles[x][z];
    }

    public int getMapWidth() {
        return width;
    }

    public int getMapHeight() {
        return height;
    }
}
