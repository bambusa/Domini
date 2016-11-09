using System.Collections.Generic;

/// <summary>
/// Model of the building data in the database
/// </summary>
public class BuildingTypesModel {

    private long building_id;
    private string typeName;
    private Dictionary<int, Dictionary<string, double>> costs;
    private Dictionary<int, Dictionary<string, double>> produces;
    private Dictionary<int, Dictionary<string, double>> consumes;

    /// <summary>
    /// Instantiate the BuildingTypesModel with all fields
    /// </summary>
	public BuildingTypesModel(long building_id, string typeName, Dictionary<int, Dictionary<string, double>> costs, Dictionary<int, Dictionary<string, double>> produces, Dictionary<int, Dictionary<string, double>> consumes) {
        this.building_id = building_id;
        this.typeName = typeName;
        this.costs = costs;
        this.produces = produces;
        this.consumes = consumes;
    }

    /// <summary>
    /// Get the primary key of the "building" table column
    /// </summary>
    public long GetId() {
        return building_id;
    }

    /// <summary>
    /// Get the type name of the "building" table column
    /// </summary>
    public string GetTypeName() {
        return typeName;
    }
}
