using System.Collections.Generic;

/// <summary>
/// Model of the building data in the database
/// </summary>
public class BuildingTypesModel {

    private long building_id;
    private string name;
    private string description;
    private int calculationLevel;
    private Dictionary<int, Dictionary<ResourceTypesModel, float>> costs; // <building level, <resource type, value>>
    private Dictionary<int, Dictionary<ResourceTypesModel, float>> produces; // <building level, <resource type, value>>
    private Dictionary<int, Dictionary<ResourceTypesModel, float>> consumes; // <building level, <resource type, value>>
    private Dictionary<int, Dictionary<ResourceTypesModel, float>> stores; // <building level, <resource type, value>>

    /// <summary>
    /// Instantiate the BuildingTypesModel with all fields
    /// </summary>
	public BuildingTypesModel(long building_id, string name, string description, int calculationLevel, Dictionary<int, Dictionary<ResourceTypesModel, float>> costs, Dictionary<int, Dictionary<ResourceTypesModel, float>> produces, Dictionary<int, Dictionary<ResourceTypesModel, float>> consumes, Dictionary<int, Dictionary<ResourceTypesModel, float>> stores) {
        this.building_id = building_id;
        this.name = name;
        this.description = description;
        this.calculationLevel = calculationLevel;
        this.costs = costs;
        this.produces = produces;
        this.consumes = consumes;
        this.stores = stores;
    }

    /// <summary>
    /// Get the primary key of the "building" table column
    /// </summary>
    public long GetId() {
        return building_id;
    }

    /// <summary>
    /// Get the building name
    /// </summary>
    public string GetName() {
        return name;
    }

    /// <summary>
    /// Get the building description
    /// </summary>
    public string GetDescription() {
        return description;
    }

    /// <summary>
    /// Get the building calculation level
    /// </summary>
    public int GetCalculationLevel() {
        return calculationLevel;
    }

    /// <summary>
    /// Get the building costs dictionary
    /// </summary>
    public Dictionary<int, Dictionary<ResourceTypesModel, float>> GetCosts() {
        return costs;
    }

    /// <summary>
    /// Get the building consume dictionary
    /// </summary>
    public Dictionary<int, Dictionary<ResourceTypesModel, float>> GetConsumes() {
        return consumes;
    }

    /// <summary>
    /// Get the building production dictionary
    /// </summary>
    public Dictionary<int, Dictionary<ResourceTypesModel, float>> GetProduces() {
        return produces;
    }

    /// <summary>
    /// Get the building storage dictionary
    /// </summary>
    public Dictionary<int, Dictionary<ResourceTypesModel, float>> GetStores() {
        return stores;
    }
}
