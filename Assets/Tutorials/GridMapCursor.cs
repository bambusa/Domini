using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GridMapGenerator))]
public class GridMapCursor : MonoBehaviour {

    GridMapGenerator gridMap;

    void Start() {
        gridMap = GetComponent<GridMapGenerator>();
    }

	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Input.GetMouseButtonDown(0) && GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity)) {
            int tileX = Mathf.FloorToInt(hitInfo.point.x / gridMap.sizeTile);
            int tileZ = Mathf.FloorToInt(hitInfo.point.z / gridMap.sizeTile);
            Debug.Log("Tile: " + tileX + "/" + tileZ);
        }
    }
}
