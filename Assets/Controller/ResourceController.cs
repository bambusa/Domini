using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controller for managing the resources
/// </summary>
public class ResourceController {

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

    public ResourceController(Dictionary<long, ResourceTypesModel> resourceTypes, Dictionary<ResourceTypesModel, float> resourceStorage) {
        Debug.Log("Init ResourceController");
        this.resourceTypes = resourceTypes;
        this.resourceStorage = resourceStorage;
        currentResourceChange = new Dictionary<ResourceTypesModel, float>();
        foreach (ResourceTypesModel resource in resourceTypes.Values) {
            currentResourceChange.Add(resource, 0);
        }
        productionBuildings = new List<BuildingModel>();
    }

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
    void Update() {
        Debug.Log("Update() ResourceController");

        // If the next update is reached
        if (Time.time >= nextUpdate) {
            nextUpdate = Mathf.FloorToInt(Time.time) + updateInterval;
            UpdateInterval();
        }

    }

    // Update is called once per defined interval (updateInterval)
    void UpdateInterval() {
        foreach (KeyValuePair<ResourceTypesModel, float> pair in currentResourceChange) {
            ResourceTypesModel resource = pair.Key;
            float value = pair.Value;
            resourceStorage[resource] += value;
            Debug.Log("Current " + resource.GetName() + " storage: " + resourceStorage[resource]);
        }
    }

}
