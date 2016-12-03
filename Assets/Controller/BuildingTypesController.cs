using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Controller for managing the ResourceTypesModels
/// </summary>
public class BuildingTypesController {

    /// <summary>
    /// Dictionary of all BuildingTypesModels with the database "building_id" as key
    /// </summary>
    private Dictionary<long, BuildingTypesModel> buildingTypes;

    
    public BuildingTypesController() {
        buildingTypes = new Dictionary<long, BuildingTypesModel>();
    }

    /// <summary>
    /// Add a BuildingTypesModel to the dictionary
    /// </summary>
    /// <returns>Returns true if the model has been added to the dictionary, false if the key already existed.</returns>
    public bool AddBuildingType(BuildingTypesModel buildingType) {
        if (!buildingTypes.ContainsKey(buildingType.GetId())) {
            buildingTypes.Add(buildingType.GetId(), buildingType);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get the whole Dictionary of BuildingTypesModels
    /// </summary>
    public Dictionary<long, BuildingTypesModel> GetBuildingTypesModels() {
        return buildingTypes;
    }
}
