using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Hold reference to all BuildingModels
/// Manage resources of all buildings
/// Create new buildings here
/// </summary>
public class BuildingLayerController : MonoBehaviour {

    public GameObject mapLayer;

    private UnityAction<string> onClick;
    private GameDataController gameDataController;
    private ArrayList buildingModels;
    private Dictionary<int, float> productionBalance;
    private Dictionary<int, float> resourceStorage;

    // Use this for initialization
    void Start () {
        buildingModels = new ArrayList();
        productionBalance = new Dictionary<int, float>();
        GameObject gameData = GameObject.Find("Game Data");
        if (gameData != null) {
            gameDataController = gameData.GetComponent<GameDataController>();
            if (gameDataController != null) {
                GenerateBuildingMenu(gameDataController.GetBuildingTypesController());
            }
            else {
                Debug.LogError("GameDataController not found");
            }
        }
        else {
            Debug.LogError("GameData object not found");
        }
    }

 
    // Update is called once per frame
    void Update () {
	
	}

    /// <summary>
    /// Generate building menu from database
    /// </summary>
    /// <param name="buildingTypesController"></param>
    private void GenerateBuildingMenu(BuildingTypesController buildingTypesController) {
        GameObject hud = GameObject.Find("Hud");
        if (hud != null) {
            Component[] components = hud.GetComponentsInChildren(typeof(HorizontalLayoutGroup), true);
            GameObject buildingMenuParent = null;
            foreach(Component c in components) {
                if (c.gameObject.name.Equals("Building Menu Content")) {
                    buildingMenuParent = c.gameObject;
                }
            }

            if (buildingMenuParent != null) {
                int buttonWidth = 100;
                int buttonHeight = 100;

                foreach (BuildingTypesModel b in buildingTypesController.GetBuildingTypesModels().Values) {
                    GameObject button = new GameObject();
                    button.name = b.GetName() + " Button";
                    button.transform.SetParent(buildingMenuParent.transform);
                    button.AddComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, buttonHeight);
                    button.AddComponent<Image>();
                    button.AddComponent<Button>().onClick.AddListener(() => CreateBuilding(b.GetName()));
                    LayoutElement l = button.AddComponent<LayoutElement>();
                    l.preferredWidth = buttonWidth;
                    l.preferredHeight = buttonHeight;

                    GameObject text = new GameObject();
                    text.name = b.GetName() + " Text";
                    text.transform.SetParent(button.transform);
                    text.AddComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, buttonHeight);
                    text.transform.position = button.transform.position;
                    Text t = text.AddComponent<Text>();
                    t.text = b.GetName();
                    t.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    t.color = Color.black;
                    t.alignment = TextAnchor.MiddleCenter;
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

    public void CreateBuilding(string typeName) {
        Debug.Log("Create building " + typeName);
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
