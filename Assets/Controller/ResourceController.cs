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
    /// Dictionary of current storage change of each resource type
    /// </summary>
    private Dictionary<ResourceTypesModel, float> resourceCapacity;
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
            resourceCapacity = new Dictionary<ResourceTypesModel, float>();
            foreach (ResourceTypesModel resource in resourceTypes.Values)
            {
                resourceCapacity.Add(resource, 0);
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
            Debug.Log("Existing building changed of " + productionBuildings.Count);
        }
        else {
            productionBuildings.Add(changedBuilding);
            Debug.Log("New building added to " + productionBuildings.Count);
        }

        // re-calculate production
        currentResourceChange = new Dictionary<ResourceTypesModel, float>();
        foreach (ResourceTypesModel resource in resourceTypes.Values) {
            currentResourceChange[resource] = 0;
        }

        // TODO: real calculation

        // iterate through built production buildings
        foreach (BuildingModel building in productionBuildings) {
            Debug.Log("Production building " + building.buildingType.GetName());
            int buildingLevel = building.GetLevel();

            // iterate through building consume
            Debug.Log("Consume Entries: " + building.buildingType.GetConsumes().Count);
            if (building.buildingType.GetConsumes().ContainsKey(buildingLevel)) {
                foreach (KeyValuePair<ResourceTypesModel, float> pair in building.buildingType.GetConsumes()[buildingLevel]) {
                    ResourceTypesModel resource = pair.Key;
                    float value = pair.Value;
                    Debug.Log("Consume of " + resource.GetName() + ": " + value);
                    currentResourceChange[resource] -= value;
                }
            }

            // iterate through building production
            Debug.Log("Production Entries: " + building.buildingType.GetProduces().Count);
            if (building.buildingType.GetProduces().ContainsKey(buildingLevel)) {
                foreach (KeyValuePair<ResourceTypesModel, float> pair in building.buildingType.GetProduces()[buildingLevel]) {
                    ResourceTypesModel resource = pair.Key;
                    float value = pair.Value;
                    Debug.Log("Production of " + resource.GetName() + ": " + value);
                    currentResourceChange[resource] += value;
                }
            }

            // iterate through building capacity
            Debug.Log("Stores Entries: " + building.buildingType.GetStores().Count);
            if (building.buildingType.GetStores().ContainsKey(buildingLevel))
            {
                foreach (KeyValuePair<ResourceTypesModel, float> pair in building.buildingType.GetStores()[buildingLevel])
                {
                    ResourceTypesModel resource = pair.Key;
                    float value = pair.Value;
                    Debug.Log("Capacity of " + resource.GetName() + ": " + value);
                    resourceCapacity[resource] += value;
                }
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
        if (currentResourceChange != null)
        {
            foreach (KeyValuePair<ResourceTypesModel, float> pair in currentResourceChange)
            {
                ResourceTypesModel resource = pair.Key;
                float value = pair.Value;
                Debug.Log("update resource storage for " + resource.GetName() + ": " + value);

                // re-calculate resource storage
                if (resourceStorage.ContainsKey(resource))
                {
                    if (resourceStorage[resource] + value < 0) resourceStorage[resource] = 0;
                    else if (resourceStorage[resource] + value > resourceCapacity[resource]) resourceStorage[resource] = resourceCapacity[resource];
                    else resourceStorage[resource] += value;
                }
                else resourceStorage.Add(resource, value);
                Debug.Log("New storage for " + resource.GetName() + ": " + resourceStorage[resource]);

                // update resource top bar
                GameObject item = resourceBarItems[resource];
                Component[] children = item.GetComponentsInChildren(typeof(Text), true);
                foreach (Component c in children)
                {
                    if (c.gameObject.name.Equals("Value"))
                    {
                        c.GetComponent<Text>().text = resourceStorage[resource].ToString();
                    }
                }
            }
        }
        else
        {
            Debug.LogError("currentResourceChange is null");
        }
    }

}
