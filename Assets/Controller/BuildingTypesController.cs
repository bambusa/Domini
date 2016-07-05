using System.Collections.Generic;
using System.Collections;

public class BuildingTypesController {

    private Dictionary<string, BuildingTypesModel> buildingTypes;

    public BuildingTypesController() {
        buildingTypes = new Dictionary<string, BuildingTypesModel>();
    }

    public bool AddBuildingType(BuildingTypesModel buildingType) {
        if (!buildingTypes.ContainsKey(buildingType.GetName())) {
            buildingTypes.Add(buildingType.GetName(), buildingType);
            return true;
        }
        return false;
    }

    public Dictionary<string, BuildingTypesModel> GetBuildingTypesModels() {
        return buildingTypes;
    }

    public BuildingTypesModel GetBuildingTypesModel(string name) {
        if (buildingTypes.ContainsKey(name)) {
            return buildingTypes[name];
        }
        return null;
    }
}
