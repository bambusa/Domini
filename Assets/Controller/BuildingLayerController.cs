using UnityEngine;
using System.Collections;

public class BuildingLayerController : MonoBehaviour {

    public GameObject mapLayer;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreatePlaceholderModel() {
        RenderBuilding (new BuildingModel("Placeholder", "#00FF33"));
    }

    void RenderBuilding(BuildingModel buildingModel) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = this.transform;
        cube.name = buildingModel.name;
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.position = Input.mousePosition;
        cube.transform.localScale = new Vector3(1, 1, 1);
        PlaceBuildingController p = cube.AddComponent<PlaceBuildingController>();
        p.mapLayer = mapLayer;
        p.buildingModel = buildingModel;
        BuildingObjectController b = cube.AddComponent<BuildingObjectController>();
        b.buildingModel = buildingModel;
        b.GetName();
    }
}
