using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Controller for managing the ResourceTypesModels
/// </summary>
public class BuildingTypesController {

    /// <summary>
    /// Dictionary of all BuildingTypesModels with the database "building_id" as key
    /// </summary>
    private Dictionary<string, BuildingTypesModel> buildingTypes;

    
    public BuildingTypesController() {
        buildingTypes = new Dictionary<string, BuildingTypesModel>();
    }

    /// <summary>
    /// Add a BuildingTypesModel to the dictionary
    /// </summary>
    /// <returns>Returns true if the model has been added to the dictionary, false if the key already existed.</returns>
    public bool AddBuildingType(BuildingTypesModel buildingType) {
        if (!buildingTypes.ContainsKey(buildingType.GetTypeName())) {
            buildingTypes.Add(buildingType.GetTypeName(), buildingType);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get the whole Dictionary of BuildingTypesModels
    /// </summary>
    public Dictionary<string, BuildingTypesModel> GetBuildingTypesModels() {
        return buildingTypes;
    }

    /// <summary>
    /// Get a BuildingTypesModel out of the dictionary with the provided key
    /// </summary>
    /// <param name="resource_id">The database "building_id" of the wanted model</param>
    public BuildingTypesModel GetBuildingTypesModel(string typeName) {
        if (buildingTypes.ContainsKey(typeName)) {
            return buildingTypes[typeName];
        }
        return null;
    }
}
