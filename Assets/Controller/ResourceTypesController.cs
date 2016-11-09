using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Controller for managing the ResourceTypesModels
/// </summary>
public class ResourceTypesController {

    /// <summary>
    /// Dictionary of all ResourceTypesModels with the database "resource_id" as key
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
    /// Get the whole Dictionary of ResourceTypesModels
    /// </summary>
    public Dictionary<long, ResourceTypesModel> GetBuildingTypesModels() {
        return resourceTypes;
    }

    /// <summary>
    /// Get a ResourceTypesModel out of the dictionary with the provided key
    /// </summary>
    /// <param name="resource_id">The database "resource_id" of the wanted model</param>
    public ResourceTypesModel GetBuildingTypesModel(long resource_id) {
        if (resourceTypes.ContainsKey(resource_id)) {
            return resourceTypes[resource_id];
        }
        return null;
    }
}
