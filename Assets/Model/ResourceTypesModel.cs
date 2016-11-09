public class ResourceTypesModel {

    private long resource_id;
    private string typeName;

    /// <summary>
    /// Instantiate the ResourceTypesModel with all fields
    /// </summary>
    public ResourceTypesModel(long resource_id, string name) {
        this.resource_id = resource_id;
        this.typeName = name;
    }

    /// <summary>
    /// Get the primary key of the "resource" table column
    /// </summary>
    public long GetId() {
        return resource_id;
    }

    /// <summary>
    /// Get the primary key of the "resource" table column
    /// </summary>
    public string GetTypeName() {
        return typeName;
    }
}
