using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for building placement
/// </summary>
public class PlaceBuildingController : MonoBehaviour {

    private BuildingModel buildingModel;
    private Collider mapCollider;
    private bool gameObjectActive;

    // Use this for initialization
    void Start () {
        Debug.Log("Start PlaceBuildingController");
    }

    /// <summary>
    /// Update is called once per frame
    /// Lets the building position follow the mouse position
    /// </summary>
    void Update () {
        if (mapCollider != null) {
            if (Input.GetMouseButtonDown(0)) { // Place the building if GameObject is active = positioned and left mouse button is clicked
                Debug.Log("Place Building here");
                buildingModel.NotifyPlaced();
                Destroy(this);
            }
            else { // Update the building position to the mouse position on the map
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (mapCollider.Raycast(ray, out hit, 100.0F)) {
                    int x = (int)Mathf.Floor(hit.point.x);
                    int z = (int)Mathf.Floor(hit.point.z);
                    //Debug.Log("Move to " + x + "/" + z);
                    buildingModel.SetPosition(x, z);
                }
            }
        }
    }

    public void SetReferences(BuildingModel buildingModel, GameObject mapLayer) {
        this.buildingModel = buildingModel;
        mapCollider = mapLayer.GetComponent<Collider>();
    }
}
