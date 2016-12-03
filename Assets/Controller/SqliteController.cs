using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine; // TODO: Delete

/// <summary>
/// Controller for all SQLite database actions
/// </summary>
public class SqliteController {

    /// <summary>
    /// Relative path to the Assets folder of the SQLite file 
    /// </summary>
    public string relativeDatabasePath = "/Plugins/SQLite/Domini0_1.db";

    private BuildingTypesController buildingTypesController;
    private ResourceTypesController resourceTypesController;
    private IDbConnection dbConn;

    /// <summary>
    /// Constructor sets up the SQLite connection
    /// </summary>
    /// <param name="dataPath">Path to the Assets folder, provided by "Application.dataPath" in Unity classes</param>
    public SqliteController(string dataPath) {
        string conn = "URI=file:" + dataPath + relativeDatabasePath; // Path to database.
        dbConn = (IDbConnection)new SqliteConnection(conn);
        LoadGameDataFromDatabase();
    }

    public BuildingTypesController GetBuildingTypesController() {
        return buildingTypesController;
    }

    public ResourceTypesController GetResourceTypesController() {
        return resourceTypesController;
    }

    /// <summary>
    /// Load all generic game data from the database
    /// </summary>
    private void LoadGameDataFromDatabase() {
        Debug.Log("LoadGameDataFromDatabase");

        // Open database connection
        dbConn.Open();
        IDbCommand dbcmd = null;
        string sqlQuery = null;
        IDataReader reader = null;

        // query for getting the default language
        long defaultLanguageId = -1;
        resourceTypesController = new ResourceTypesController();
        dbcmd = dbConn.CreateCommand();
        sqlQuery = "SELECT * FROM language WHERE is_default = 1 LIMIT 1";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        int languageIdIndex = reader.GetOrdinal("language_id");
        while (reader.Read()) {
            defaultLanguageId = reader.GetInt64(languageIdIndex);
        }
        reader.Close();
        dbcmd.Dispose();

        if (defaultLanguageId > 0) {

            // query for getting all resource types
            Dictionary<long, ResourceTypesModel> resourceTypeIds = new Dictionary<long, ResourceTypesModel>();
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT resource.resource_id, resource_translation.name, resource_translation.description" +
                " FROM resource" +
                " LEFT JOIN resource_translation ON resource.resource_id = resource_translation.resource_id" +
                " WHERE resource_translation.language_id = " + defaultLanguageId + ";";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int resourceIdIndex = reader.GetOrdinal("resource_id");
            int resourceNameIndex = reader.GetOrdinal("name");
            int resourceDescIndex = reader.GetOrdinal("description");
            while (reader.Read()) {
                long id = reader.GetInt64(resourceIdIndex);
                string name = reader.GetString(resourceNameIndex);
                string description = null;
                if (!reader.IsDBNull(resourceDescIndex)) description = reader.GetString(resourceDescIndex);
                ResourceTypesModel resourceType = new ResourceTypesModel(id, name, description);
                resourceTypesController.AddResourceType(resourceType);
                resourceTypeIds.Add(id, resourceType);
            }
            reader.Close();
            dbcmd.Dispose();



            // query for getting all building types with resource costs, production, consume and storage
            buildingTypesController = new BuildingTypesController();
            sqlQuery = "SELECT building.building_id AS building_building_id, building.calculation_level AS building_calculation_level" +
                ", building_translation.name AS building_translation_name, building_translation.description AS building_translation_description" +
                ", building_costs.resource_id AS building_costs_resource_id, building_costs.level AS building_costs_level, building_costs.value AS building_costs_value" +
                ", building_produces.resource_id AS building_produces_resource_id, building_produces.level AS building_produces_level, building_produces.value AS building_produces_value" +
                ", building_consumes.resource_id AS building_consumes_resource_id, building_consumes.level AS building_consumes_level, building_consumes.value AS building_consumes_value" +
                ", building_stores.resource_id AS building_stores_resource_id, building_stores.level AS building_stores_level, building_stores.value AS building_stores_value" +
                ", resource_cost.type_name AS resource_cost_type_name" +
                ", resource_prod.type_name AS resource_prod_type_name" +
                ", resource_cons.type_name AS resource_cons_type_name" +
                ", resource_store.type_name AS resource_store_type_name" +
                " FROM building" +
                " LEFT JOIN building_translation ON building.building_id = building_translation.building_id" +
                " LEFT JOIN building_costs ON building.building_id = building_costs.building_id" +
                " LEFT JOIN building_produces ON building.building_id = building_produces.building_id" +
                " LEFT JOIN building_consumes ON building.building_id = building_consumes.building_id" +
                " LEFT JOIN building_stores ON building.building_id = building_stores.building_id" +
                " LEFT JOIN resource AS resource_cost ON building_costs.resource_id = resource_cost.resource_id" +
                " LEFT JOIN resource AS resource_prod ON building_produces.resource_id = resource_prod.resource_id" +
                " LEFT JOIN resource AS resource_cons ON building_consumes.resource_id = resource_cons.resource_id" +
                " LEFT JOIN resource AS resource_store ON building_stores.resource_id = resource_store.resource_id" +
                " WHERE building_translation.language_id = " + defaultLanguageId +
                " ORDER BY building_calculation_level ASC, building_building_id ASC, building_costs_level ASC, building_produces_level ASC, building_consumes_level ASC, building_stores_level ASC;";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            // query result column indexes
            int buildingIdIndex = reader.GetOrdinal("building_building_id");
            int buildingCalculationLevelIndex = reader.GetOrdinal("building_building_calculation_level");
            int buildingNameIndex = reader.GetOrdinal("building_translation_name");
            int buildingDescriptionIndex = reader.GetOrdinal("building_translation_description");
            int buildingCostsResourceIdIndex = reader.GetOrdinal("building_costs_resource_id");
            int buildingCostsLevelIndex = reader.GetOrdinal("building_costs_level");
            int buildingCostsValueIndex = reader.GetOrdinal("building_costs_value");
            int buildingProducesResourceIdIndex = reader.GetOrdinal("building_produces_resource_id");
            int buildingProducesLevelIndex = reader.GetOrdinal("building_produces_level");
            int buildingProducesValueIndex = reader.GetOrdinal("building_produces_value");
            int buildingConsumesResourceIdIndex = reader.GetOrdinal("building_consumes_resource_id");
            int buildingConsumesLevelIndex = reader.GetOrdinal("building_consumes_level");
            int buildingConsumesValueIndex = reader.GetOrdinal("building_consumes_value");
            int buildingStoresResourceIdIndex = reader.GetOrdinal("building_stores_resource_id");
            int buildingStoresLevelIndex = reader.GetOrdinal("building_stores_level");
            int buildingStoresValueIndex = reader.GetOrdinal("building_stores_value");
            int resourceCostNameIndex = reader.GetOrdinal("resource_cost_type_name");
            int resourceProdNameIndex = reader.GetOrdinal("resource_prod_type_name");
            int resourceConsNameIndex = reader.GetOrdinal("resource_cons_type_name");
            int resourceStoreNameIndex = reader.GetOrdinal("resource_store_type_name");

            // model dictionaries
            Dictionary<ResourceTypesModel, float> buildingCostsLevel = new Dictionary<ResourceTypesModel, float>(); // <resource type, value>
            Dictionary<ResourceTypesModel, float> buildingConsumesLevel = new Dictionary<ResourceTypesModel, float>(); // <resource type, value>
            Dictionary<ResourceTypesModel, float> buildingProducesLevel = new Dictionary<ResourceTypesModel, float>(); // <resource type, value>
            Dictionary<ResourceTypesModel, float> buildingStoresLevel = new Dictionary<ResourceTypesModel, float>(); // <resource type, value>
            Dictionary<int, Dictionary<ResourceTypesModel, float>> buildingCosts = new Dictionary<int, Dictionary<ResourceTypesModel, float>>(); // <building level, buildingCostsLevel>
            Dictionary<int, Dictionary<ResourceTypesModel, float>> buildingProduces = new Dictionary<int, Dictionary<ResourceTypesModel, float>>(); // <building level, buildingConsumesLevel>
            Dictionary<int, Dictionary<ResourceTypesModel, float>> buildingConsumes = new Dictionary<int, Dictionary<ResourceTypesModel, float>>(); // <building level, buildingProducesLevel>
            Dictionary<int, Dictionary<ResourceTypesModel, float>> buildingStores = new Dictionary<int, Dictionary<ResourceTypesModel, float>>(); // <building level, buildingStoresLevel>

            // current model
            long buildingId = -1;
            string buildingName = "";
            string buildingDescription = "";
            int costLevel = -1;
            int consumeLevel = -1;
            int produceLevel = -1;
            int storeLevel = -1;

            while (reader.Read()) {
                Debug.Log("Building row: " + reader.GetString(buildingNameIndex) + " (" + reader.GetInt64(buildingIdIndex) + ")");

                // gather building costs
                int thisCostLevel = reader.GetInt32(buildingCostsLevelIndex);
                if (thisCostLevel > costLevel && costLevel != -1) {
                    // save buildingCosts level
                    buildingCosts.Add(costLevel, new Dictionary<ResourceTypesModel, float>(buildingCostsLevel));
                    buildingCostsLevel = new Dictionary<ResourceTypesModel, float>();
                }
                long buildingCostsResourceId = reader.GetInt64(buildingCostsResourceIdIndex);
                if (resourceTypeIds.ContainsKey(buildingCostsResourceId)) {
                    costLevel = reader.GetInt32(buildingCostsLevelIndex);
                    float buildingCostsValue = reader.GetFloat(buildingCostsValueIndex);
                    ResourceTypesModel resourceType = resourceTypeIds[buildingCostsResourceId];
                    buildingCostsLevel.Add(resourceType, buildingCostsValue);
                }

                // gather building consumes
                int thisConsumeLevel = reader.GetInt32(buildingConsumesLevelIndex);
                if (thisConsumeLevel > consumeLevel && consumeLevel != -1) {
                    // save buildingCnsumes level
                    buildingConsumes.Add(consumeLevel, new Dictionary<ResourceTypesModel, float>(buildingConsumesLevel));
                    buildingConsumesLevel = new Dictionary<ResourceTypesModel, float>();
                }
                long buildingConsumesResourceId = reader.GetInt64(buildingConsumesResourceIdIndex);
                if (resourceTypeIds.ContainsKey(buildingConsumesResourceId)) {
                    consumeLevel = reader.GetInt32(buildingConsumesLevelIndex);
                    float buildingConsumesValue = reader.GetFloat(buildingConsumesValueIndex);
                    ResourceTypesModel resourceType = resourceTypeIds[buildingConsumesResourceId];
                    buildingConsumesLevel.Add(resourceType, buildingConsumesValue);
                }

                // gather building produces
                int thisProducesLevel = reader.GetInt32(buildingProducesLevelIndex);
                if (thisProducesLevel > produceLevel && produceLevel != -1) {
                    // save buildingProduces level
                    buildingProduces.Add(produceLevel, new Dictionary<ResourceTypesModel, float>(buildingProducesLevel));
                    buildingProducesLevel = new Dictionary<ResourceTypesModel, float>();
                }
                long buildingProducesResourceId = reader.GetInt64(buildingCostsResourceIdIndex);
                if (resourceTypeIds.ContainsKey(buildingProducesResourceId)) {
                    produceLevel = reader.GetInt32(buildingProducesLevelIndex);
                    float buildingProducesValue = reader.GetFloat(buildingProducesValueIndex);
                    ResourceTypesModel resourceType = resourceTypeIds[buildingProducesResourceId];
                    buildingCostsLevel.Add(resourceType, buildingProducesValue);
                }

                // gather building stores
                int thisStoresLevel = reader.GetInt32(buildingStoresLevelIndex);
                if (thisStoresLevel > storeLevel && storeLevel != -1) {
                    // save buildingStores level
                    buildingStores.Add(storeLevel, new Dictionary<ResourceTypesModel, float>(buildingStoresLevel));
                    buildingStoresLevel = new Dictionary<ResourceTypesModel, float>();
                }
                long buildingStoresResourceId = reader.GetInt64(buildingStoresResourceIdIndex);
                if (resourceTypeIds.ContainsKey(buildingStoresResourceId)) {
                    costLevel = reader.GetInt32(buildingStoresLevelIndex);
                    float buildingStoresValue = reader.GetFloat(buildingStoresValueIndex);
                    ResourceTypesModel resourceType = resourceTypeIds[buildingStoresResourceId];
                    buildingStoresLevel.Add(resourceType, buildingStoresValue);
                }

                // if new building type save the old data and begin new model
                long thisBuildingId = reader.GetInt64(buildingIdIndex);
                if (thisBuildingId > buildingId && buildingId != -1) {
                    // save previous model
                    Debug.Log("Save BuildingTypesModel: " + buildingName + " (" + buildingId + ")");
                    buildingCosts.Add(costLevel, new Dictionary<ResourceTypesModel, float>(buildingCostsLevel));
                    buildingConsumes.Add(consumeLevel, new Dictionary<ResourceTypesModel, float>(buildingConsumesLevel));
                    buildingProduces.Add(produceLevel, new Dictionary<ResourceTypesModel, float>(buildingProducesLevel));
                    buildingStores.Add(storeLevel, new Dictionary<ResourceTypesModel, float>(buildingStoresLevel));
                    BuildingTypesModel b = new BuildingTypesModel(buildingId, buildingName, buildingDescription, new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingCosts), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingProduces), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingConsumes), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingStores));
                    buildingTypesController.AddBuildingType(b);

                    buildingCosts = new Dictionary<int, Dictionary<ResourceTypesModel, float>>();
                    buildingProduces = new Dictionary<int, Dictionary<ResourceTypesModel, float>>();
                    buildingConsumes = new Dictionary<int, Dictionary<ResourceTypesModel, float>>();
                    buildingStores = new Dictionary<int, Dictionary<ResourceTypesModel, float>>();
                }
                buildingId = thisBuildingId;
                buildingName = reader.GetString(buildingNameIndex);
                buildingDescription = reader.GetString(buildingDescriptionIndex);

            }
            reader.Close();

            if (buildingId != -1) {
                Debug.Log("Save BuildingTypesModel: " + buildingName + " (" + buildingId + ")");
                if (costLevel != -1) buildingCosts.Add(costLevel, new Dictionary<ResourceTypesModel, float>(buildingCostsLevel));
                if (consumeLevel != -1) buildingConsumes.Add(consumeLevel, new Dictionary<ResourceTypesModel, float>(buildingConsumesLevel));
                if (produceLevel != -1) buildingProduces.Add(produceLevel, new Dictionary<ResourceTypesModel, float>(buildingProducesLevel));
                if (storeLevel != -1) buildingStores.Add(storeLevel, new Dictionary<ResourceTypesModel, float>(buildingStoresLevel));
                BuildingTypesModel b = new BuildingTypesModel(buildingId, buildingName, buildingDescription, new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingCosts), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingProduces), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingConsumes), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingStores));
                buildingTypesController.AddBuildingType(b);
            }
        }

        // Close database connection
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbConn.Close();
        dbConn = null;
    }
}