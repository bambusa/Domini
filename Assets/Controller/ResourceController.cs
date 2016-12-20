using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for managing the resources
/// </summary>
public class ResourceController : MonoBehaviour {

    public GameObject prefabItem;

    /// <summary>
    /// Min interval of update method in seconds
    /// </summary>
    int updateInterval = 1;
    /// <summary>
    /// Timestamp of next update in seconds
    /// </summary>
    int nextUpdate = 0;
    /// <summary>
    /// Dictionary of all ResourceTypesModels with the database id
    /// </summary>
    private Dictionary<long, ResourceTypesModel> resourceTypes;
    /// <summary>
    /// List of all built production buildings
    /// </summary>
    private List<BuildingModel> productionBuildings;
    /// <summary>
    /// Dictionary of current storage value of each resource type
    /// </summary>
    private Dictionary<ResourceTypesModel, float> resourceStorage;
    /// <summary>
    /// Dictionary of current storage change of each resource type
    /// </summary>
    private Dictionary<ResourceTypesModel, float> currentResourceChange;
    /// <summary>
    /// Dictionary of UI Panels in the top bar for displaying the resource storage
    /// </summary>
    private Dictionary<ResourceTypesModel, GameObject> resourceBarItems;

    // Use this for initialization
    void Start() {
        if (GameDataController.resourceTypes == null) {
            Debug.LogError("ResourceTypes not found");
        }
        else if (GameDataController.playerResources == null) {
            Debug.LogError("PlayerResources not found");
        }
        else {
            resourceTypes = GameDataController.resourceTypes;
            resourceStorage = GameDataController.playerResources;
            currentResourceChange = new Dictionary<ResourceTypesModel, float>();
            foreach (ResourceTypesModel resource in resourceTypes.Values) {
                currentResourceChange.Add(resource, 0);
            }
            productionBuildings = new List<BuildingModel>();
            GenerateResourceBar();

            // Enable BuildingLayerController after ResourceController is instantiated
            GameObject buildingLayer = GameObject.Find("Building Layer");
            BuildingLayerController mb = buildingLayer.GetComponent("BuildingLayerController") as BuildingLayerController;
            mb.OnResourcesChanged = CbOnResourcesChanged;
            mb.enabled = true;
        }
    }

    /// <summary>
    /// Generate resource bar from database
    /// </summary>
    /// <param name="buildingTypesController"></param>
    private void GenerateResourceBar() {
        Debug.Log("GenerateResourceBar");
        resourceBarItems = new Dictionary<ResourceTypesModel, GameObject>();
        GameObject hud = GameObject.Find("Hud");

        if (hud != null) {
            Component[] components = hud.GetComponentsInChildren(typeof(HorizontalLayoutGroup), true);
            GameObject resourceItemParent = null;
            foreach (Component c in components) {
                if (c.gameObject.name.Equals("Resource Panel Content")) {
                    resourceItemParent = c.gameObject;
                }
            }

            if (resourceItemParent != null) {
                foreach (ResourceTypesModel resource in resourceTypes.Values) {
                    GameObject item = (GameObject)Instantiate(prefabItem);
                    item.name = resource.GetName() + " Item";
                    item.transform.SetParent(resourceItemParent.transform);

                    Component[] children = item.GetComponentsInChildren(typeof(Text), true);
                    foreach (Component c in children) {
                        if (c.gameObject.name.Equals("Name")) {
                            c.GetComponent<Text>().text = resource.GetName();
                        }
                        else if (c.gameObject.name.Equals("Value")) {
                            if (resourceStorage.ContainsKey(resource)) c.GetComponent<Text>().text = resourceStorage[resource].ToString();
                            else c.GetComponent<Text>().text = "0";
                        }
                    }
                    resourceBarItems.Add(resource, item);
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
    /// Callback gets called when new buildings was built or got updated
    /// </summary>
    /// <param name="changedBuilding">BuildingModel of the changed building</param>
    public void CbOnResourcesChanged(BuildingModel changedBuilding) {
        Debug.Log("CbOnResourcesChanged (" + changedBuilding.buildingType.GetName() + ")");
        if (productionBuildings.Contains(changedBuilding)) {
            Debug.Log("Existing building changed");
            productionBuildings.Add(changedBuilding);
        }
        else {
            Debug.Log("New building added");
        }

        // re-calculate production
        currentResourceChange = new Dictionary<ResourceTypesModel, float>();
        foreach (ResourceTypesModel resource in resourceTypes.Values) {
            currentResourceChange[resource] = 0;
        }

        // TODO: real calculation

        // iterate through built production buildings
        foreach (BuildingModel building in productionBuildings) {
            int buildingLevel = building.GetLevel();

            // iterate through building consume
            foreach(KeyValuePair<ResourceTypesModel, float> pair in building.buildingType.GetConsumes()[buildingLevel]) {
                ResourceTypesModel resource = pair.Key;
                float value = pair.Value;
                currentResourceChange[resource] -= value;
            }

            // iterate through building production
            foreach (KeyValuePair<ResourceTypesModel, float> pair in building.buildingType.GetProduces()[buildingLevel]) {
                ResourceTypesModel resource = pair.Key;
                float value = pair.Value;
                currentResourceChange[resource] += value;
            }
        }
    }

    // Update is called once per frame
    private void Update() {

        // If the next update is reached
        if (Time.time >= nextUpdate) {
            nextUpdate = Mathf.FloorToInt(Time.time) + updateInterval;
            UpdateInterval();
        }

    }

    // Update is called once per defined interval (1 second)
    private void UpdateInterval() {
        foreach (KeyValuePair<ResourceTypesModel, float> pair in currentResourceChange) {
            ResourceTypesModel resource = pair.Key;
            float value = pair.Value;

            // re-calculate resource storage
            if (resourceStorage.ContainsKey(resource)) resourceStorage[resource] += value;
            else resourceStorage.Add(resource, value);

            // update resource top bar
            GameObject item = resourceBarItems[resource];
            Component[] children = item.GetComponentsInChildren(typeof(Text), true);
            foreach (Component c in children) {
                if (c.gameObject.name.Equals("Value")) {
                    c.GetComponent<Text>().text = resourceStorage[resource].ToString();
                }
            }
        }
    }

}
