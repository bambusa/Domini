using UnityEngine;using System.Collections;[RequireComponent(typeof(MeshFilter))][RequireComponent(typeof(MeshRenderer))][RequireComponent(typeof(MeshCollider))]public class GridMapGenerator : MonoBehaviour {    public int sizeX = 20;    public int sizeZ = 20;    public float sizeTile = 1.0f;    public Texture2D terrainTexture;    public int textureResolution;    // Use this for initialization    void Start() {        BuildMesh();    }    public void BuildMesh() {
        // Generate mesh data
        int verticesXNumber = sizeX + 1;        int verticesZNumber = sizeZ + 1;        int verticeNumber = verticesXNumber * verticesZNumber;        int triangleNumber = sizeX * sizeZ * 2;        int normalNumber = verticeNumber;        int uvNumber = verticeNumber;        Vector3[] vertices = new Vector3[verticeNumber];        int[] triangles = new int[triangleNumber * 3];        Vector3[] normals = new Vector3[normalNumber];        Vector2[] uv = new Vector2[uvNumber];

        int x, z;        for (z = 0; z < verticesZNumber; z++) {            for (x = 0; x < verticesXNumber; x++) {                vertices[z * verticesXNumber + x] = new Vector3(x * sizeTile, 0, z * sizeTile);                normals[z * verticesXNumber + x] = Vector3.up;
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
        Mesh mesh = new Mesh();        mesh.vertices = vertices;        mesh.triangles = triangles;        mesh.normals = normals;        mesh.uv = uv;        // Assign mesh to game        MeshFilter meshFilter = GetComponent<MeshFilter>();        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();        MeshCollider meshCollider = GetComponent<MeshCollider>();        meshFilter.mesh = mesh;        meshCollider.sharedMesh = mesh;        BuildTextures();    }    void BuildTextures() {
        int textureWidth = sizeX * textureResolution;
        int textureHeight = sizeZ * textureResolution;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        Color[][] tiles = PrepareTerrainTextureTiles();

        for (int y = 0; y < sizeZ; y++) {
            for (int x = 0; x < sizeX; x++) {
                int randomTile = Random.Range(0, 4);
                texture.SetPixels(x * textureResolution, y * textureResolution, textureResolution, textureResolution, tiles[randomTile]);
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
        return tiles;
    }}