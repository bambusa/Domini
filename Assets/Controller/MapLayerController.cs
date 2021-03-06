﻿using UnityEngine;/// <summary>
/// Generating the Map Model and render it
/// </summary>[RequireComponent(typeof(MeshFilter))][RequireComponent(typeof(MeshRenderer))][RequireComponent(typeof(MeshCollider))]public class MapLayerController : MonoBehaviour {    public Texture2D mapFile;    public Texture2D terrainTexture;    public int textureResolution;    public int textureVersion;    MapModel mapModel;
    int sizeX;    int sizeZ;

    void Start() {        PrepareMapData();        BuildMesh();    }    /// <summary>
    /// Generate all vertices, triangles, normals and uvs for the map model
    /// </summary>    public void BuildMesh() {
        // Generate mesh data
        int verticesXNumber = sizeX + 1;        int verticesZNumber = sizeZ + 1;        int verticeNumber = verticesXNumber * verticesZNumber;        int triangleNumber = sizeX * sizeZ * 2;        int normalNumber = verticeNumber;        int uvNumber = verticeNumber;        Vector3[] vertices = new Vector3[verticeNumber]; // Basic vertices        int[] triangles = new int[triangleNumber * 3]; // Triangles consisting each of 3 vertices        Vector3[] normals = new Vector3[normalNumber]; // Lighting direction of vertex        Vector2[] uv = new Vector2[uvNumber]; // Texture position of vertex

        int x, z;        for (z = 0; z < verticesZNumber; z++) {            for (x = 0; x < verticesXNumber; x++) {                vertices[z * verticesXNumber + x] = new Vector3(x, 0, z);                normals[z * verticesXNumber + x] = Vector3.up;
                uv[z * verticesXNumber + x] = new Vector2((float)x / sizeX, (float)z / sizeZ);
                //Debug.Log("Created vertex " + (z * verticesXNumber + x) + ": " + (x * sizeTile) + ", 0, " + (z * sizeTile));
            }        }        for (z = 0; z < sizeZ; z++) {            for (x = 0; x < sizeX; x++) {
                int squareIndex = z * sizeX + x;
                int triangleIndex = squareIndex * 6;
                int triangleOffset = (z + 1) * verticesXNumber + x;
                //Debug.Log("squareIndex: " + squareIndex + ", triangleIndex: " + triangleIndex +  ", triangleOffset: " + triangleOffset);

                triangles[triangleIndex + 0] = triangleOffset;
                triangles[triangleIndex + 1] = triangleOffset - verticesXNumber + 1;
                triangles[triangleIndex + 2] = triangleOffset - verticesXNumber;
                //Debug.Log("Created triangle " + triangleIndex + ": " + triangles[triangleIndex + 0] + ", " + triangles[triangleIndex + 1] + ", " + triangles[triangleIndex + 2]);
                triangles[triangleIndex + 3] = triangleOffset;
                triangles[triangleIndex + 4] = triangleOffset + 1;
                triangles[triangleIndex + 5] = triangleOffset - verticesXNumber + 1;
                //Debug.Log("Created triangle " + (triangleIndex + 1) + ": " + triangles[triangleIndex + 3] + ", " + triangles[triangleIndex + 4] + ", " + triangles[triangleIndex + 5]);
            }
        }



        // Create mesh with data
        Mesh mesh = new Mesh();        mesh.vertices = vertices;        mesh.triangles = triangles;        mesh.normals = normals;        mesh.uv = uv;

        // Assign mesh to game
        MeshFilter meshFilter = GetComponent<MeshFilter>();        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();        MeshCollider meshCollider = GetComponent<MeshCollider>();        meshFilter.mesh = mesh;        meshCollider.sharedMesh = mesh;        BuildTextures();    }    /// <summary>
    /// Create texture for the map material on top of the mesh
    /// </summary>    void BuildTextures() {
        int textureWidth = sizeX * textureResolution;
        int textureHeight = sizeZ * textureResolution;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        Color[][] textures = PrepareTerrainTextureTiles();

        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                int textureIndex = mapModel.getTile(x, z).getTextureIndex();
                //Debug.Log("textureIndex " + textureIndex + " for tile " + x + "/" + z);
                texture.SetPixels(x * textureResolution, z * textureResolution, textureResolution, textureResolution, textures[textureIndex]);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }    Color[][] PrepareTerrainTextureTiles() {
        int tilesPerRow = terrainTexture.width / textureResolution;
        int rowNumber = terrainTexture.height / textureResolution;
        Color[][] tiles = new Color[tilesPerRow * rowNumber][];

        for (int y = 0; y < rowNumber; y++) {
            for (int x = 0; x < tilesPerRow; x++) {
                tiles[y * tilesPerRow + x] = terrainTexture.GetPixels(x * textureResolution, y * textureResolution, textureResolution, textureResolution);
            }
        }
        //Debug.Log("Prepared " + tiles.Length + " textures (" + tilesPerRow + " * " + rowNumber + ")");
        return tiles;
    }    /// <summary>
    /// Create MapModel from map file
    /// Each pixel is a terrain type, represented by color
    /// </summary>    void PrepareMapData() {
        MapModel map = null;

        if (mapFile != null) {
            //Debug.Log("Found map file");
            int mapWidth = mapFile.width;
            int mapHeight = mapFile.height;
            Color[] pixels = mapFile.GetPixels(0, 0, mapWidth, mapHeight);
            string[][] hexCodes = new string[mapWidth][];
            for (int x = 0; x < mapWidth; x++) {
                hexCodes[x] = new string[mapHeight];
                for (int z = 0; z < mapHeight; z++) {
                    Color pixel = pixels[z * mapWidth + x];
                    hexCodes[x][z] = ((int)(pixel.r * 255)).ToString("X2") + ((int)(pixel.g * 255)).ToString("X2") + ((int)(pixel.b * 255)).ToString("X2");
                    //Debug.Log("Found color #" + hexCodes[x][z] + " for pixel " + (z * mapWidth + x) + " at coordinates " + x + "/" + z);
                }
            }
            map = new MapModel(mapWidth, mapHeight, hexCodes, textureVersion);
        }
        else {
            Debug.Log("No map file provided, generating random map");
            if (sizeX != 0 && sizeZ != 0) {
                map = new MapModel(sizeX, sizeZ, textureVersion);
            }
            else {
                map = new MapModel();
            }
        }

        sizeX = map.getMapWidth();
        sizeZ = map.getMapHeight();
        mapModel = map;
    }}