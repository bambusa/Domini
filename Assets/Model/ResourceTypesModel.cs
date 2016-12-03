public class ResourceTypesModel {

    private long resource_id;
    private string name;
    private string description;

    /// <summary>
    /// Instantiate the ResourceTypesModel with all fields
    /// </summary>
    public ResourceTypesModel(long resource_id, string name, string description) {
        this.resource_id = resource_id;
        this.name = name;
        this.description = description;
    }

    /// <summary>
    /// Get the primary key of the "resource" table column
    /// </summary>
    public long GetId() {
        return resource_id;
    }

    /// <summary>
    /// Get the localized name
    /// </summary>
    public string GetName() {
        return name;
    }

    /// <summary>
    /// Get the localized description
    /// </summary>
    public string GetDescription() {
        return description;
    }
}
