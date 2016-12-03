using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Create buildings here, render and set callbacks
/// </summary>
public class BuildingLayerController : MonoBehaviour {

    public GameObject mapLayer;
    public GameObject prefabButton;
    public GameObject buildingMenuPanel;

    private BuildingTypesController buildingTypesController;
    private ResourceTypesController resourceTypesController;
    private UnityAction<string> onClick;
    private long resourcesLastCalculated;
    private Dictionary<int, float> productionBalance;
    private Dictionary<int, float> resourceStorage;

    // Use this for initialization
    void Start () {
        if (GameDataController.buildingTypesController == null) {
            Debug.LogError("BuildingTypesController not found");
        }
        else if (GameDataController.resourceTypesController == null) {
           Debug.LogError("ResourceTypesController not found");
        }
        else {
            buildingTypesController = GameDataController.buildingTypesController;
            resourceTypesController = GameDataController.resourceTypesController;
            GenerateBuildingMenu();
        }
    }

    /// <summary>
    /// Generate building menu from database
    /// </summary>
    /// <param name="buildingTypesController"></param>
    private void GenerateBuildingMenu() {
            Debug.Log("GenerateBuildingMenu");
            GameObject hud = GameObject.Find("Hud");
            

            if (hud != null) {
                Component[] components = hud.GetComponentsInChildren(typeof(GridLayoutGroup), true);
                GameObject buildingMenuParent = null;
                foreach (Component c in components) {
                    if (c.gameObject.name.Equals("Building Menu Content")) {
                        buildingMenuParent = c.gameObject;
                    }
                }

                if (buildingMenuParent != null) {
                    Dictionary<long, BuildingTypesModel> buildingTypes = GameDataController.buildingTypesController.GetBuildingTypesModels();
                    int buttonWidth = 100;
                    int buttonHeight = 100;

                    foreach (BuildingTypesModel b in buildingTypes.Values) {
                        BuildingTypesModel tempB = b;
                        GameObject button = (GameObject)Instantiate(prefabButton);

                        button.name = b.GetName() + " Button";
                        button.transform.SetParent(buildingMenuParent.transform);
                        button.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, buttonHeight);
                        button.GetComponent<Button>().onClick.AddListener(() => CreateBuilding(tempB));

                        Text t = button.GetComponentInChildren<Text>();
                        t.text = b.GetName();
                    }
                }
                else {
                    Debug.LogError("Building Menu Content not found");
                }
            }
            else {
                Debug.LogError("Hud not found");
            }
        
    }

    public void CreateBuilding(BuildingTypesModel buildingTypesModel) {
        Debug.Log("Create building " + buildingTypesModel.GetName());
        BuildingModel newBuilding = new BuildingModel(buildingTypesModel);
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
        //cube.SetActive(false);
        cube.transform.parent = this.transform;
        cube.name = buildingModel.buildingType.GetName();
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale = new Vector3(1, 1, 1);
        cube.AddComponent<PlaceBuildingController>().SetReferences(buildingModel, mapLayer);
        cube.AddComponent<BuildingObjectController>().SetReferences(buildingModel);

        buildingModel.CbRegisterResourcesChanged(resourceTypesController.CbOnResourcesChanged);
        buildingMenuPanel.SetActive(false);
    }
}
