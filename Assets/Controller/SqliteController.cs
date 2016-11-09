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
    public string relativeDatabasePath = "/Plugins/SQLite/Domini0_3_1.db";

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

    /// <summary>
    /// Load all generic game data from the database
    /// </summary>
    private void LoadGameDataFromDatabase() {
        //Debug.Log("LoadGameDataFromDatabase");

        // Open database connection
        dbConn.Open();
        IDbCommand dbcmd = null;
        string sqlQuery = null;
        IDataReader reader = null;

        //// Get ResourceTypes
        //resourceTypesController = new ResourceTypesController();
        //dbcmd = dbConn.CreateCommand();
        //sqlQuery = "SELECT r FROM resource;";
        //dbcmd.CommandText = sqlQuery;
        //reader = dbcmd.ExecuteReader();
        //while (reader.Read()) {
        //    int resource_id = reader.GetInt32(0);
        //    string name = reader.GetString(1);

        //    ResourceTypesModel r = new ResourceTypesModel(resource_id, name);
        //    resourceTypesController.AddResourceType(r);
        //    //Debug.Log("Found resource " + name);
        //}
        //reader.Close();

        //// Get BuildingTypes
        //buildingTypesController = new BuildingTypesController();
        //dbcmd = dbConn.CreateCommand();
        //sqlQuery = "SELECT * FROM building;";
        //dbcmd.CommandText = sqlQuery;
        //reader = dbcmd.ExecuteReader();
        //while (reader.Read()) {
        //    int building_id = reader.GetInt32(0);
        //    string name = reader.GetString(1);

        //    ResourceTypesModel b = new ResourceTypesModel(building_id, name);
        //    buildingTypesController.AddBuildingType(b);
        //    //Debug.Log("Found building " + name);
        //}
        //reader.Close();

        // create query for getting all buildings with costs, produces and consumes for all levels
        buildingTypesController = new BuildingTypesController();
        dbcmd = dbConn.CreateCommand();
        sqlQuery = "SELECT building.building_id AS building_building_id, building.type_name AS building_type_name" +
            ", building_costs.resource_id AS building_costs_resource_id, building_costs.level AS building_costs_level, building_costs.value AS building_costs_value" +
            ", building_produces.resource_id AS building_produces_resource_id, building_produces.level AS building_produces_level, building_produces.value AS building_produces_value" +
            ", building_consumes.resource_id AS building_consumes_resource_id, building_consumes.level AS building_consumes_level, building_consumes.value AS building_consumes_value" +
            ", resource_costs.type_name AS resource_costs_type_name" +
            ", resource_produces.type_name AS resource_produces_type_name" +
            ", resource_consumes.type_name AS resource_consumes_type_name" +
            " FROM building" +
            " LEFT JOIN building_costs ON building.building_id = building_costs.building_id" +
            " LEFT JOIN building_produces ON building.building_id = building_produces.building_id" +
            " LEFT JOIN building_consumes ON building.building_id = building_consumes.building_id" +
            " LEFT JOIN resource AS resource_costs ON building_costs.resource_id = resource_costs.resource_id" +
            " LEFT JOIN resource AS resource_produces ON building_produces.resource_id = resource_produces.resource_id" +
            " LEFT JOIN resource AS resource_consumes ON building_consumes.resource_id = resource_consumes.resource_id" +
            " ORDER BY building_building_id ASC, building_costs_level ASC, building_produces_level ASC, building_consumes_level ASC;";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        // query result column indexes
        int buildingBuildingIdIndex = reader.GetOrdinal("building_building_id");
        int buildingTypeNameIndex = reader.GetOrdinal("building_type_name");
        int buildingCostsResourceIdIndex = reader.GetOrdinal("building_costs_resource_id");
        int buildingCostsLevelIndex = reader.GetOrdinal("building_costs_level");
        int buildingCostsValueIndex = reader.GetOrdinal("building_costs_value");
        int buildingProducesResourceIdIndex = reader.GetOrdinal("building_produces_resource_id");
        int buildingProducesLevelIndex = reader.GetOrdinal("building_produces_level");
        int buildingProducesValueIndex = reader.GetOrdinal("building_produces_value");
        int buildingConsumesResourceIdIndex = reader.GetOrdinal("building_consumes_resource_id");
        int buildingConsumesLevelIndex = reader.GetOrdinal("building_consumes_level");
        int buildingConsumesValueIndex = reader.GetOrdinal("building_consumes_value");
        int resourceCostsTypeNameIndex = reader.GetOrdinal("resource_costs_type_name");
        int resourceProducesTypeNameIndex = reader.GetOrdinal("resource_produces_type_name");
        int resourceConsumesTypeNameIndex = reader.GetOrdinal("resource_consumes_type_name");

        // model dictionaries
        Dictionary<int, Dictionary<string, double>> buildingCosts = null; // <level, <resource_type_name, value>>
        Dictionary<string, double> buildingCostEntries = new Dictionary<string, double>();
        Dictionary<int, Dictionary<string, double>> buildingProduces = null; // <level, <resource_type_name, value>>
        Dictionary<string, double> buildingProduceEntries = new Dictionary<string, double>();
        Dictionary<int, Dictionary<string, double>> buildingConsumes = null; // <level, <resource_type_name, value>>
        Dictionary<string, double> buildingConsumeEntries = new Dictionary<string, double>();
        
        // current model
        long buildingId = -1;
        string buildingTypeName = "";
        int buildingCostLevel = -1;
        int buildingProduceLevel = -1;
        int buildingConsumeLevel = -1;

        while (reader.Read()) {
            Debug.Log("Building row: (" + reader.GetInt64(buildingBuildingIdIndex) + ") " + reader.GetString(buildingTypeNameIndex));
            long thisId = reader.GetInt64(buildingBuildingIdIndex);

            // if new building type save the old data and begin new model
            if (thisId > buildingId) {
                if (buildingId != -1) {
                    // save previous model
                    Debug.Log("Save BuildingTypesModel: " + buildingTypeName);
                    BuildingTypesModel b = new BuildingTypesModel(buildingId, buildingTypeName, new Dictionary<int, Dictionary<string, double>>(buildingCosts), new Dictionary<int, Dictionary<string, double>>(buildingProduces), new Dictionary<int, Dictionary<string, double>>(buildingConsumes));
                    buildingTypesController.AddBuildingType(b);
                }

                buildingId = thisId;
                buildingTypeName = reader.GetString(buildingTypeNameIndex);
                buildingCostLevel = -1;
                buildingProduceLevel = -1;
                buildingConsumeLevel = -1;
                buildingCosts = new Dictionary<int, Dictionary<string, double>>();
                buildingProduces = new Dictionary<int, Dictionary<string, double>>();
                buildingConsumes = new Dictionary<int, Dictionary<string, double>>();
            }


            // if new building level
            int thisCostLevel = -1;
            if (!reader.IsDBNull(buildingCostsLevelIndex)) thisCostLevel = reader.GetInt32(buildingCostsLevelIndex);
            int thisProduceLevel = -1;
            if (!reader.IsDBNull(buildingProducesLevelIndex)) thisProduceLevel = reader.GetInt32(buildingProducesLevelIndex);
            int thisConsumeLevel = -1;
            if (!reader.IsDBNull(buildingConsumesLevelIndex)) thisConsumeLevel = reader.GetInt32(buildingConsumesLevelIndex);

            // if new cost level save the previous data and begin new dictionary
            if (thisCostLevel > buildingCostLevel) {
                Debug.Log("Cost row: " + reader.GetString(resourceCostsTypeNameIndex));
                if (buildingCostEntries != null && buildingCostEntries.Count > 0) {
                    Debug.Log("Save buildingCosts for level " + buildingCostLevel + ": " + buildingCostEntries.Count);
                    buildingCosts.Add(buildingCostLevel, new Dictionary<string, double>(buildingCostEntries));
                    buildingCostEntries = new Dictionary<string, double>();
                }

                // new cost row
                if (!reader.IsDBNull(resourceCostsTypeNameIndex) && !reader.IsDBNull(buildingCostsValueIndex)) {
                    buildingCostEntries.Add(reader.GetString(resourceCostsTypeNameIndex), reader.GetDouble(buildingCostsValueIndex));
                }  
            }

            // if new produce level save the previous data and begin new dictionary
            if (thisProduceLevel > buildingProduceLevel) {
                Debug.Log("Produce row: " + reader.GetString(resourceProducesTypeNameIndex));
                if (buildingProduceEntries != null && buildingProduceEntries.Count > 0) {
                    Debug.Log("Save buildingProduces for level " + buildingProduceLevel + ": " + buildingProduceEntries.Count);
                    buildingProduces.Add(buildingProduceLevel, new Dictionary<string, double>(buildingProduceEntries));
                    buildingProduceEntries = new Dictionary<string, double>();
                }

                // new Produce row
                if (!reader.IsDBNull(resourceProducesTypeNameIndex) && !reader.IsDBNull(buildingProducesValueIndex)) {
                    buildingProduceEntries.Add(reader.GetString(resourceProducesTypeNameIndex), reader.GetDouble(buildingProducesValueIndex));
                }
            }

            // if new consume level save the previous data and begin new dictionary
            if (thisConsumeLevel > buildingConsumeLevel) {
                Debug.Log("Consume row: " + reader.GetString(resourceConsumesTypeNameIndex));
                if (buildingConsumeEntries != null && buildingConsumeEntries.Count > 0) {
                    Debug.Log("Save buildingConsumes for level " + buildingConsumeLevel + ": " + buildingConsumeEntries.Count);
                    buildingConsumes.Add(buildingConsumeLevel, new Dictionary<string, double>(buildingConsumeEntries));
                    buildingConsumeEntries = new Dictionary<string, double>();
                }

                // new Consume row
                if (!reader.IsDBNull(resourceConsumesTypeNameIndex) && !reader.IsDBNull(buildingConsumesValueIndex)) {
                    buildingConsumeEntries.Add(reader.GetString(resourceConsumesTypeNameIndex), reader.GetDouble(buildingConsumesValueIndex));
                }
            }
        }
        reader.Close();

        if (buildingId != -1) {
            Debug.Log("Save last BuildingTypesModel: " + buildingTypeName);
            BuildingTypesModel b = new BuildingTypesModel(buildingId, buildingTypeName, new Dictionary<int, Dictionary<string, double>>(buildingCosts), new Dictionary<int, Dictionary<string, double>>(buildingProduces), new Dictionary<int, Dictionary<string, double>>(buildingConsumes));
            buildingTypesController.AddBuildingType(b);
        }

        // Close database connection
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbConn.Close();
        dbConn = null;
    }
}