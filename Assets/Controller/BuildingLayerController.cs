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

    private Dictionary<long, BuildingTypesModel> buildingTypes;
    private Dictionary<long, ResourceTypesModel> resourceTypes;
    private ResourceController resourceController;
    private UnityAction<string> onClick;
    private long resourcesLastCalculated;
    private Dictionary<int, float> productionBalance;
    private Dictionary<int, float> resourceStorage;

    // Use this for initialization
    void Start () {
        if (GameDataController.buildingTypes == null) {
            Debug.LogError("BuildingTypes not found");
        }
        else if (GameDataController.resourceTypes == null) {
           Debug.LogError("ResourceTypes not found");
        }
        else if (GameDataController.resourceController == null) {
            Debug.LogError("ResourceController not found");
        }
        else if (GameDataController.playerBuildings == null) {
            Debug.LogError("PlayerBuildings not found");
        }
        else {
            buildingTypes = GameDataController.buildingTypes;
            resourceTypes = GameDataController.resourceTypes;
            resourceController = GameDataController.resourceController;
            GenerateBuildingMenu();
            GeneratePlayerBuildings(GameDataController.playerBuildings);
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

    private void GeneratePlayerBuildings(List<BuildingModel> buildings) {
        foreach(BuildingModel building in buildings) {
            RenderBuilding(building, true);
        }
    }

    private void CreateBuilding(BuildingTypesModel buildingTypesModel) {
        Debug.Log("Trying to create building " + buildingTypesModel.GetName());

        BuildingModel newBuilding = new BuildingModel(buildingTypesModel);
        RenderBuilding(newBuilding, false);
    }

    /// <summary>
    /// Create new building GameObject and render it,
    /// attach BuildingModel, PlaceBuildingController and BuildingObjectController to the GameObject,
    /// pass the needed references to those components
    /// </summary>
    /// <param name="buildingModel">BuildingModel of the building</param>
    private void RenderBuilding(BuildingModel buildingModel, bool placeInstantly) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); // TODO: Placeholder 
        //cube.SetActive(false);
        cube.transform.parent = this.transform;
        cube.name = buildingModel.buildingType.GetName();
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale = new Vector3(1, 1, 1);
        if (!placeInstantly) cube.AddComponent<PlaceBuildingController>().SetReferences(buildingModel, mapLayer);
        cube.AddComponent<BuildingObjectController>().SetReferences(buildingModel, placeInstantly);

        buildingModel.CbRegisterResourcesChanged(resourceController.CbOnResourcesChanged);
        buildingMenuPanel.SetActive(false);
    }
}
