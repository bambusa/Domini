using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))][RequireComponent(typeof(MeshRenderer))][RequireComponent(typeof(MeshCollider))]
public class BuildingObjectController : MonoBehaviour {

    public BuildingModel buildingModel;

	// Use this for initialization
	void Start () {
        Debug.Log("BuildingObjectController started");
        if (buildingModel != null) {
            buildingModel.SetPlacedCallback(RuildingPlacedCallback);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void RuildingPlacedCallback(BuildingModel b) {
        if (b.isPlaced())
            Debug.Log("Building got placed");
    }

    public void GetName() {
        if (buildingModel != null) {
            Debug.Log("Building name: " + buildingModel.name);
        }
        else {
            Debug.LogError("BuildingModel not provided");
        }
    }
}
