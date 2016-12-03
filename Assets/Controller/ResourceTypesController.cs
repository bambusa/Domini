﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controller for managing the resources
/// </summary>
public class ResourceTypesController {

    /// <summary>
    /// Dictionary of all ResourceTypesModels with the database id
    /// </summary>
    private Dictionary<long, ResourceTypesModel> resourceTypes;

    public ResourceTypesController() {
        resourceTypes = new Dictionary<long, ResourceTypesModel>();
    }

    /// <summary>
    /// Add a ResourceTypesModel to the dictionary
    /// </summary>
    /// <returns>Returns true if the model has been added to the dictionary, false if the key already existed.</returns>
    public bool AddResourceType(ResourceTypesModel resourceType) {
        if (!resourceTypes.ContainsKey(resourceType.GetId())) {
            resourceTypes.Add(resourceType.GetId(), resourceType);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get all ResourceTypes
    /// </summary>
    public Dictionary<long, ResourceTypesModel> GetResourceTypesModels() {
        return resourceTypes;
    }

    public void CbOnResourcesChanged(BuildingModel changedBuilding) {
        Debug.Log("CbOnResourcesChanged (" + changedBuilding.buildingType.GetName() + ")");
    }
}
