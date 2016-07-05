using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Hold reference to all BuildingModels
/// Manage resources of all buildings
/// Create new buildings here
/// </summary>
public class BuildingLayerController : MonoBehaviour {

    public GameObject mapLayer;

    ArrayList buildingModels;
    Dictionary<int, float> productionBalance;
    Dictionary<int, float> resourceStorage;

    // Use this for initialization
    void Start () {
        buildingModels = new ArrayList();
        productionBalance = new Dictionary<int, float>();
	}

 
    // Update is called once per frame
    void Update () {
	
	}

    /// <summary>
    /// Calculate production of all buildings
    /// </summary>
    void updateResourceProduction() {
        foreach (BuildingModel building in buildingModels) {
            productionBalance.Add(0, building.GetProduction());
        }
        Debug.Log("Updated Production to: " + productionBalance[0]);
    }

    /// <summary>
    /// Create new placeholder BuildingModel for testing
    /// </summary>
    public void CreatePlaceholderModel() {
        BuildingModel newBuilding = new BuildingModel("Placeholder", "#00FF33");
        newBuilding.CbRegisterResourcesChanged(OnBuildingResourcesChanged);
        RenderBuilding(newBuilding);
    }

    /// <summary>
    /// Create new building GameObject and render it,
    /// attach BuildingModel, PlaceBuildingController and BuildingObjectController to the GameObject,
    /// pass the needed references to those components
    /// </summary>
    /// <param name="buildingModel">BuildingModel of the building</param>
    void RenderBuilding(BuildingModel buildingModel) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); // TODO: Placeholder 
        cube.SetActive(false);
        cube.transform.parent = this.transform;
        cube.name = buildingModel.GetName();
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale = new Vector3(1, 1, 1);
        cube.AddComponent<PlaceBuildingController>().SetReferences(buildingModel, mapLayer);
        cube.AddComponent<BuildingObjectController>().SetReferences(buildingModel);
        buildingModels.Add(buildingModel);
    }

    /// <summary>
    /// Callback for changed resource data of a building
    /// </summary>
    /// <param name="buildingModel">The BuildingModel which data changed</param>
    public void OnBuildingResourcesChanged(BuildingModel buildingModel) {
        updateResourceProduction();
    }
}
