using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using UnityEngine; // TODO: Delete

/// <summary>
/// Controller for all SQLite database actions
/// </summary>
public class SqliteController {

    /// <summary>
    /// Relative path to the SQLite database file in the assets folder 
    /// </summary>
    public string relativeDatabasePath = "/Plugins/SQLite/Domini0_1.db";
    string conn;

    private Dictionary<long, BuildingTypesModel> buildingTypes;
    private Dictionary<long, ResourceTypesModel> resourceTypes;
    private Dictionary<long, EpochModel> epochs;
    private Dictionary<long, TechnologyModel> technologies;
    private PlayerModel player;
    private List<BuildingModel> playerBuildings;
    private EpochModel playerEpoch;
    private Dictionary<ResourceTypesModel, float> playerResources;
    private List<TechnologyModel> playerTechnologies;
    private IDbConnection dbConn;

    /// <summary>
    /// Constructor sets up the SQLite connection
    /// </summary>
    /// <param name="dataPath">Path to the Assets folder, provided by "Application.dataPath" in Unity classes</param>
    public SqliteController(string dataPath) {
       conn  = "URI=file:" + dataPath + relativeDatabasePath; // Path to database.
    }

    /// <summary>
    /// Load data from database, 
    /// resource types, building types with costs etc., technologies and epochs
    /// </summary>
    /// <returns>boolean if all data was read successfully</returns>
    public bool LoadGameData() {
        Debug.Log("LoadGameData()");
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        player = LoadPlayer();
        if (player == null) {
            Debug.LogError("Error while loading player");
            dbConn.Close();
            return false;
        }

        long playerLanguageId = LoadPlayerLanguage();
        if (playerLanguageId <= 0) {
            Debug.LogError("Error while loading player language");
            dbConn.Close();
            return false;
        }

        resourceTypes = LoadResourceTypes(playerLanguageId);
        if (resourceTypes == null || resourceTypes.Count == 0) {
            Debug.LogError("Error while loading resource types");
            dbConn.Close();
            return false;
        }

        buildingTypes = LoadBuildingTypes(playerLanguageId);
        if (buildingTypes == null || buildingTypes.Count == 0) {
            Debug.LogError("Error while loading building types");
            dbConn.Close();
            return false;
        }

        epochs = LoadEpochs(playerLanguageId);
        if (epochs == null || epochs.Count == 0) {
            Debug.LogError("Error while loading epochs");
            dbConn.Close();
            return false;
        }

        technologies = LoadTechnologies(playerLanguageId);
        if (technologies == null || technologies.Count == 0) {
            Debug.LogError("Error while loading technologies");
            dbConn.Close();
            return false;
        }

        playerBuildings = LoadPlayerBuildings();
        if (playerBuildings == null) {
            Debug.LogError("Error while loading player buildings");
            dbConn.Close();
            return false;
        }

        playerEpoch = LoadPlayerEpoch();
        if (playerEpoch == null) {
            Debug.LogError("Error while loading player epoch");
            dbConn.Close();
            return false;
        }

        playerResources = LoadPlayerResources();
        if (playerResources == null) {
            Debug.LogError("Error while loading player resources");
            dbConn.Close();
            return false;
        }

        playerTechnologies = LoadPlayerTechnologies();
        if (playerTechnologies == null) {
            Debug.LogError("Error while loading player technologies");
            dbConn.Close();
            return false;
        }

        dbConn.Close();
        return true;
    }

    /// <summary>
    /// Get translation language of player,
    /// fallback is the default language in database
    /// </summary>
    /// <returns>database id of player language</returns>
    private long LoadPlayerLanguage() {
        Debug.Log("LoadPlayerLanguage()");
        long languageId = -1;
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM language WHERE is_default = 1 LIMIT 1";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int languageIdIndex = reader.GetOrdinal("language_id");
        while (reader.Read()) {
            languageId = reader.GetInt64(languageIdIndex);
            string defaultLanguageName = reader.GetString(reader.GetOrdinal("name"));
            Debug.Log("Default language: " + defaultLanguageName);
        }

        reader.Close();
        dbcmd.Dispose();
        return languageId;
    }

    /// <summary>
    /// Get all resource types from database
    /// </summary>
    /// <param name="languageId">language id</param>
    /// <returns>Dictionary of resource types with database id as key</returns>
    private Dictionary<long, ResourceTypesModel> LoadResourceTypes(long languageId) {
        Debug.Log("LoadResourceTypes()");
        Dictionary<long, ResourceTypesModel> resourceTypes = new Dictionary<long, ResourceTypesModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT resource.resource_id, resource_translation.name, resource_translation.description" +
            " FROM resource" +
            " LEFT JOIN resource_translation ON resource.resource_id = resource_translation.resource_id" +
            " WHERE resource_translation.language_id = " + languageId + ";";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int resourceIdIndex = reader.GetOrdinal("resource_id");
        int resourceNameIndex = reader.GetOrdinal("name");
        int resourceDescIndex = reader.GetOrdinal("description");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(resourceIdIndex);
            string name = reader.GetString(resourceNameIndex);
            string description = null;
            if (!reader.IsDBNull(resourceDescIndex)) description = reader.GetString(resourceDescIndex);
            Debug.Log("Found resource type: " + name + " (" + id + ")");
            ResourceTypesModel resourceType = new ResourceTypesModel(id, name, description);
            resourceTypes.Add(id, resourceType);
        }
        reader.Close();
        dbcmd.Dispose();
        return resourceTypes;
    }

    /// <summary>
    /// Get all building types from database
    /// </summary>
    /// <param name="languageId">language id</param>
    /// <returns>Dictionary of building types with database id as key</returns>
    private Dictionary<long, BuildingTypesModel> LoadBuildingTypes(long languageId) {
        Debug.Log("LoadBuildingTypes()");
        Dictionary<long, BuildingTypesModel>  buildingTypes = new Dictionary<long, BuildingTypesModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM building" +
            " LEFT JOIN building_translation ON building.building_id = building_translation.building_id" +
            " WHERE building_translation.language_id = " + languageId +
            " ORDER BY building.calculation_level ASC, building.building_id ASC;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int buildingIdIndex = reader.GetOrdinal("building_id");
        int buildingCalculationLevelIndex = reader.GetOrdinal("calculation_level");
        int buildingNameIndex = reader.GetOrdinal("name");
        int buildingDescriptionIndex = reader.GetOrdinal("description");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(buildingIdIndex);
            string name = reader.GetString(buildingNameIndex);
            string description = reader.GetString(buildingDescriptionIndex);
            int calculationLevel = reader.GetInt32(buildingCalculationLevelIndex);
            Debug.Log("Found building type: " + name + " (" + id + ")");

            Dictionary<int, Dictionary<ResourceTypesModel, float>> buildingCosts, buildingConsumes, buildingProduces, buildingStorage;
            buildingCosts = LoadBuildingResources("building_costs", id);
            buildingConsumes = LoadBuildingResources("building_consumes", id);
            buildingProduces = LoadBuildingResources("building_produces", id);
            buildingStorage = LoadBuildingResources("building_stores", id);

            BuildingTypesModel buildingType = new BuildingTypesModel(id, name, description, calculationLevel, buildingCosts, buildingProduces, buildingConsumes, buildingStorage);
            buildingTypes.Add(id, buildingType);
        }
        reader.Close();
        dbcmd.Dispose();
        return buildingTypes;
    }

    /// <summary>
    /// Get costs, consume, production or storage of a building.
    /// </summary>
    /// <param name="queryParam">table name</param>
    /// <param name="buildingId">database id of building</param>
    /// <returns>Returns dictionary with resource value and resource type as key, within a dictionary with building level as key</returns>
    private Dictionary<int, Dictionary<ResourceTypesModel, float>> LoadBuildingResources(string queryParam, long buildingId) {
        Debug.Log("LoadBuildingResources(" + queryParam + ", " + buildingId + ")");
        Dictionary < int, Dictionary < ResourceTypesModel, float>> buildingResources = new Dictionary<int, Dictionary<ResourceTypesModel, float>>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM " + queryParam +
            " WHERE building_id = " + buildingId +
            " ORDER BY level ASC";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int resourceIdIndex = reader.GetOrdinal("resource_id");
        int levelIndex = reader.GetOrdinal("level");
        int valueIndex = reader.GetOrdinal("value");

        // query processing
        while (reader.Read()) {
            long resourceId = reader.GetInt64(resourceIdIndex);
            if (resourceTypes.ContainsKey(resourceId)) {
                ResourceTypesModel resource = resourceTypes[resourceId];
                int level = reader.GetInt32(levelIndex);
                float value = reader.GetFloat(valueIndex);

                if (!buildingResources.ContainsKey(level)) {
                    buildingResources.Add(level, new Dictionary<ResourceTypesModel, float>());
                }
                buildingResources[level].Add(resource, value);
            }
            else {
                Debug.LogError("Resource type not found for ID: " + resourceId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return buildingResources;
    }

    /// <summary>
    /// Get all epochs from database
    /// </summary>
    /// <param name="languageId">language database id</param>
    /// <returns>Dictionary of all epochs with database id as key</returns>
    private Dictionary<long, EpochModel> LoadEpochs(long languageId) {
        Debug.Log("LoadEpochs(" + languageId + ")");
        epochs = new Dictionary<long, EpochModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM epoch LEFT JOIN epoch_translation ON epoch.epoch_id = epoch_translation.epoch_id WHERE epoch_translation.language_id = " + languageId;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int epochIdIndex = reader.GetOrdinal("epoch_id");
        int epochNameIndex = reader.GetOrdinal("name");
        int epochDescriptionIndex = reader.GetOrdinal("description");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(epochIdIndex);
            string name = reader.GetString(epochNameIndex);
            string description = null;
            if (!reader.IsDBNull(epochDescriptionIndex)) description = reader.GetString(epochDescriptionIndex);
            EpochModel epoch = new EpochModel(id, name, description);
            epochs.Add(id, epoch);
        }
        reader.Close();
        dbcmd.Dispose();
        return epochs;
    }

    /// <summary>
    /// Get all technologies from database
    /// </summary>
    /// <param name="languageId">language database id</param>
    /// <returns>Dictionary of all technologies with database id as key</returns>
    private Dictionary<long, TechnologyModel> LoadTechnologies(long languageId) {
        Debug.Log("LoadTechnologies(" + languageId + ")");
        Dictionary<long, TechnologyModel> technologies = new Dictionary<long, TechnologyModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM technology" +
            " LEFT JOIN technology_translation ON technology.technology_id = technology_translation.technology_id" +
            " WHERE technology_translation.language_id = " + languageId +
            " ORDER BY technology_id ASC;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int technologyIdIndex = reader.GetOrdinal("technology_id");
        int technologyNameIndex = reader.GetOrdinal("name");
        int technologyDescriptionIndex = reader.GetOrdinal("description");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(technologyIdIndex);
            string name = reader.GetString(technologyNameIndex);
            string description = null;
            if (!reader.IsDBNull(technologyDescriptionIndex)) description = reader.GetString(technologyDescriptionIndex);
            Debug.Log("Found technology " + name + " (" + id + ")");

            Dictionary<ResourceTypesModel, float> technologyCosts = LoadTechnologyResources(id);
            List<TechnologyModel> predecessors = LoadTechnologyPredecessors(technologies, id);
            Dictionary<BuildingTypesModel, int> unlocksBuildings = LoadBuildingUnlocks(id);
            EpochModel unlocksEpoch = LoadEpochUnlock(id);

            TechnologyModel technology = new TechnologyModel(id, name, description, technologyCosts, predecessors, unlocksBuildings, unlocksEpoch);
            technologies.Add(id, technology);
        }
        reader.Close();
        dbcmd.Dispose();
        return technologies;
    }

    /// <summary>
    /// Get costs of a technology.
    /// </summary>
    /// <param name="technologyId">database id of technology</param>
    /// <returns>Returns dictionary resource value and resource type as key</returns>
    private Dictionary<ResourceTypesModel, float> LoadTechnologyResources(long technologyId) {
        Debug.Log("LoadTechnologyResources(" + technologyId + ")");
        Dictionary<ResourceTypesModel, float> costs = new Dictionary<ResourceTypesModel, float>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM technology_costs_resource" +
            " WHERE technology_id = " + technologyId +
            " ORDER BY resource_id ASC";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int resourceIdIndex = reader.GetOrdinal("resource_id");
        int valueIndex = reader.GetOrdinal("value");

        // query processing
        while (reader.Read()) {
            long resourceId = reader.GetInt64(resourceIdIndex);
            if (resourceTypes.ContainsKey(resourceId)) {
                ResourceTypesModel resource = resourceTypes[resourceId];
                float value = reader.GetFloat(valueIndex);
                costs.Add(resource, value);
            }
            else {
                Debug.LogError("Resource type not found for ID: " + resourceId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return costs;
    }

    /// <summary>
    /// Get predecessor technologies of a technology
    /// </summary>
    /// <param name="technologyId">technology database id</param>
    /// <returns>List of predecessor technologies</returns>
    private List<TechnologyModel> LoadTechnologyPredecessors(Dictionary<long, TechnologyModel> technologies, long technologyId) {
        Debug.Log("LoadTechnologyPredecessors(" + technologyId + ")");
        List<TechnologyModel> predecessors = new List<TechnologyModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM technology_needs_predecessor" +
            " WHERE technology_id = " + technologyId +
            " ORDER BY technology_id ASC;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int predecessorIdIndex = reader.GetOrdinal("predecessor_technology_id");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(predecessorIdIndex);
            if (technologies.ContainsKey(id)) {
                predecessors.Add(technologies[id]);
            }
            else {
                Debug.LogError("Did not found predecessor technology for ID: " + technologyId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return predecessors;
    }

    /// <summary>
    /// Get buildings and building levels that get unlocked by a technology
    /// </summary>
    /// <param name="technologyId">technology database id</param>
    /// <returns>Dictionary of unlocked buildings and unlocked levels of the buildings</returns>
    private Dictionary<BuildingTypesModel, int> LoadBuildingUnlocks(long technologyId) {
        Debug.Log("LoadBuildingUnlocks(" + technologyId + ")");
        Dictionary<BuildingTypesModel, int> unlocks = new Dictionary<BuildingTypesModel, int>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM technology_unlocks_building" +
            " WHERE technology_id = " + technologyId +
            " ORDER BY technology_id ASC;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int buildingIdIndex = reader.GetOrdinal("building_id");
        int levelIndex = reader.GetOrdinal("level");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(buildingIdIndex);
            if (buildingTypes.ContainsKey(id)) {
                BuildingTypesModel buildingType = buildingTypes[id];
                int level = reader.GetInt32(levelIndex);
                unlocks.Add(buildingType, level);
            }
            else {
                Debug.LogError("Did not found unlocked building for ID: " + id);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return unlocks;
    }

    /// <summary>
    /// Get the epoch that gets unlocked by a technology
    /// </summary>
    /// <param name="technologyId">technology database id</param>
    /// <returns>Unlocked epoch</returns>
    private EpochModel LoadEpochUnlock(long technologyId) {
        Debug.Log("LoadEpochUnlock(" + technologyId + ")");
        EpochModel unlocks = null;
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM technology_unlocks_epoch" +
            " WHERE technology_id = " + technologyId +
            " ORDER BY technology_id ASC LIMIT 1;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // query result column indexes
        int epochIdIndex = reader.GetOrdinal("epoch_id");

        // query processing
        while (reader.Read()) {
            long id = reader.GetInt64(epochIdIndex);
            if (epochs.ContainsKey(epochIdIndex)) {
                unlocks = epochs[epochIdIndex];
            }
            else {
                Debug.LogError("Did not found unlocked epoch for ID: " + id);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return unlocks;
    }

    /// <summary>
    /// Get Player data
    /// </summary>
    /// <returns>Player model</returns>
    private PlayerModel LoadPlayer() {
        Debug.Log("LoadPlayer()");
        PlayerModel player = null;
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT username, email FROM player LIMIT 1";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int playerNameIndex = reader.GetOrdinal("username");
        int playerDescriptionIndex = reader.GetOrdinal("email");
        while (reader.Read()) {
            string name = reader.GetString(playerNameIndex);
            string description = null;
            if (!reader.IsDBNull(playerDescriptionIndex)) description = reader.GetString(playerDescriptionIndex);
            player = new PlayerModel(name, description);
        }
        reader.Close();
        dbcmd.Dispose();
        return player;
    }

    /// <summary>
    /// Get all buildings, the player has built
    /// </summary>
    /// <returns>List of building models</returns>
    private List<BuildingModel> LoadPlayerBuildings() {
        Debug.Log("LoadPlayerBuildings()");
        List<BuildingModel> playerBuildings = new List<BuildingModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM player_has_building";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int playerBuildingIdIndex = reader.GetOrdinal("building_id");
        int playerBuildingLevelIndex = reader.GetOrdinal("level");
        int playerBuildingPosXIndex = reader.GetOrdinal("pos_x");
        int playerBuildingPosZIndex = reader.GetOrdinal("pos_z");

        while (reader.Read()) {
            long playerBuildingId = reader.GetInt64(playerBuildingIdIndex);
            int playerBuildingLevel = reader.GetInt32(playerBuildingLevelIndex);
            int playerBuildingPosX = reader.GetInt32(playerBuildingPosXIndex);
            int playerBuildingPosZ = reader.GetInt32(playerBuildingPosZIndex);

            if (buildingTypes.ContainsKey(playerBuildingId)) {
                BuildingTypesModel playerBuilding = buildingTypes[playerBuildingId];
                BuildingModel building = new BuildingModel(playerBuilding, playerBuildingLevel, playerBuildingPosX, playerBuildingPosZ);
                playerBuildings.Add(building);
            }
            else {
                Debug.LogError("Error while loading player building type for ID: " + playerBuildingId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return playerBuildings;
    }

    /// <summary>
    /// Get the highest epoch the player has reached
    /// </summary>
    /// <returns>Epoch model</returns>
    private EpochModel LoadPlayerEpoch() {
        Debug.Log("LoadPlayerEpoch()");
        EpochModel epoch = null;
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM player_has_epoch ORDER BY epoch_id DESC LIMIT 1";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int playerEpochIdIndex = reader.GetOrdinal("epoch_id");

        while (reader.Read()) {
            long playerEpochId = reader.GetInt64(playerEpochIdIndex);
            if (epochs.ContainsKey(playerEpochId)) {
                 epoch = epochs[playerEpochId];
            }
            else {
                Debug.LogError("Error while loading player epoch for ID: " + playerEpochId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return epoch;
    }

    /// <summary>
    /// Get all resources the player has in the storage
    /// </summary>
    /// <returns>List of resource values with resource type as key</returns>
    private Dictionary<ResourceTypesModel, float> LoadPlayerResources() {
        Debug.Log("LoadPlayerResources()");
        Dictionary<ResourceTypesModel, float> playerResources = new Dictionary<ResourceTypesModel, float>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM player_has_resource";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int playerResourceIdIndex = reader.GetOrdinal("resource_id");
        int playerResourceValueIndex = reader.GetOrdinal("value");

        while (reader.Read()) {
            long playerResourceId = reader.GetInt64(playerResourceIdIndex);
            float playerResourceValue = reader.GetFloat(playerResourceValueIndex);
            if (resourceTypes.ContainsKey(playerResourceId)) {
                ResourceTypesModel resourceType = resourceTypes[playerResourceId];
                playerResources.Add(resourceType, playerResourceValue);
            }
            else {
                Debug.LogError("Error while loading player resources for ID: " + playerResourceId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return playerResources;
    }

    /// <summary>
    /// Get all technologies the player has researched
    /// </summary>
    /// <returns>List of technologies</returns>
    private List<TechnologyModel> LoadPlayerTechnologies() {
        Debug.Log("LoadPlayerTechnologies()");
        List<TechnologyModel> playerTechnologies = new List<TechnologyModel>();
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM player_has_technology";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int playerTechnologyIdIndex = reader.GetOrdinal("technology_id");

        while (reader.Read()) {
            long playerTechnologyId = reader.GetInt64(playerTechnologyIdIndex);
            if (technologies.ContainsKey(playerTechnologyId)) {
                TechnologyModel playerTechnology = technologies[playerTechnologyId];
                playerTechnologies.Add(playerTechnology);
            }
            else {
                Debug.LogError("Error while loading player technology for ID: " + playerTechnologyId);
            }
        }
        reader.Close();
        dbcmd.Dispose();
        return playerTechnologies;
    }

    public Dictionary<long, BuildingTypesModel> GetBuildingTypes() {
        return buildingTypes;
    }

    public Dictionary<long, ResourceTypesModel> GetResourceTypes() {
        return resourceTypes;
    }

    public Dictionary<long, EpochModel> GetEpochs() {
        return epochs;
    }

    public Dictionary<long, TechnologyModel> GetTechnologies() {
        return technologies;
    }

    public PlayerModel GetPlayer() {
        return player;
    }

    public List<BuildingModel> GetPlayerBuildings() {
        return playerBuildings;
    }

    public EpochModel GetPlayerEpoch() {
        return playerEpoch;
    }

    public Dictionary<ResourceTypesModel, float> GetPlayerResources() {
        return playerResources;
    }

    public List<TechnologyModel> GetPlayerTechnologies() {
        return playerTechnologies;
    }

    /// <summary>
    /// Load all generic game data from the database
    /// </summary>
    private void LoadGameDataFromDatabase() {

        // Open database connection
        dbConn.Open();
        IDbCommand dbcmd = null;
        string sqlQuery = null;
        IDataReader reader = null;

        // query for getting the default language
        long defaultLanguageId = -1;
        string defaultLanguageName = null;
        dbcmd = dbConn.CreateCommand();
        sqlQuery = "SELECT * FROM language WHERE is_default = 1 LIMIT 1";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        int languageIdIndex = reader.GetOrdinal("language_id");
        while (reader.Read()) {
            defaultLanguageId = reader.GetInt64(languageIdIndex);
            defaultLanguageName = reader.GetString(reader.GetOrdinal("name"));
            Debug.Log("Default language: " + defaultLanguageName);
        }
        reader.Close();
        dbcmd.Dispose();

        if (defaultLanguageId > 0) {

            ///
            /// Resource Types
            /// 
            Debug.Log("Getting resource types...");
            resourceTypes = new Dictionary<long, ResourceTypesModel>();
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
                Debug.Log("Found resource type: " + name + " (" + id + ")");
                ResourceTypesModel resourceType = new ResourceTypesModel(id, name, description);
                resourceTypes.Add(id, resourceType);
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Building Types
            /// 
            Debug.Log("Getting building types...");
            buildingTypes = new Dictionary<long, BuildingTypesModel>();
            dbcmd = dbConn.CreateCommand();
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
                " ORDER BY building_building_id ASC, building_calculation_level ASC, building_costs_level ASC, building_produces_level ASC, building_consumes_level ASC, building_stores_level ASC;";
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
            string buildingName = null;
            string buildingDescription = null;
            int costLevel = -1;
            int consumeLevel = -1;
            int produceLevel = -1;
            int storeLevel = -1;

            while (reader.Read()) {
                if (buildingId == -1 && !reader.IsDBNull(buildingIdIndex)) buildingId = reader.GetInt64(buildingIdIndex);
                if (costLevel == -1 && !reader.IsDBNull(buildingCostsLevelIndex)) costLevel = reader.GetInt32(buildingCostsLevelIndex);
                if (consumeLevel == -1 && !reader.IsDBNull(buildingConsumesLevelIndex)) consumeLevel = reader.GetInt32(buildingConsumesLevelIndex);
                if (produceLevel == -1 && !reader.IsDBNull(buildingProducesLevelIndex)) produceLevel = reader.GetInt32(buildingProducesLevelIndex);
                if (storeLevel == -1 && !reader.IsDBNull(buildingStoresLevelIndex)) storeLevel = reader.GetInt32(buildingStoresLevelIndex);

                //Debug.Log("Database row: " + reader.GetString(buildingNameIndex) + " (" + reader.GetInt64(buildingIdIndex) + ")");

                if (!reader.IsDBNull(buildingCostsLevelIndex)) {
                    // gather building costs
                    int thisCostLevel = reader.GetInt32(buildingCostsLevelIndex);
                    if (!buildingCosts.ContainsKey(thisCostLevel)) {
                        // save buildingCosts level
                        buildingCosts.Add(thisCostLevel, new Dictionary<ResourceTypesModel, float>());
                    }
                    long buildingCostsResourceId = reader.GetInt64(buildingCostsResourceIdIndex);
                    if (resourceTypes.ContainsKey(buildingCostsResourceId)) {
                        float buildingCostsValue = reader.GetFloat(buildingCostsValueIndex);
                        ResourceTypesModel resourceType = resourceTypes[buildingCostsResourceId];
                        if (!buildingCosts[thisCostLevel].ContainsKey(resourceType)) {
                            buildingCosts[thisCostLevel].Add(resourceType, buildingCostsValue);
                        }
                    }
                }

                if (!reader.IsDBNull(buildingConsumesLevelIndex)) {
                    // gather building consumes
                    int thisConsumeLevel = reader.GetInt32(buildingConsumesLevelIndex);
                    if (!buildingConsumes.ContainsKey(thisConsumeLevel)) {
                        // save buildingConsumes level
                        buildingConsumes.Add(thisConsumeLevel, new Dictionary<ResourceTypesModel, float>());
                    }
                    long buildingConsumesResourceId = reader.GetInt64(buildingConsumesResourceIdIndex);
                    if (resourceTypes.ContainsKey(buildingConsumesResourceId)) {
                        float buildingConsumesValue = reader.GetFloat(buildingConsumesValueIndex);
                        ResourceTypesModel resourceType = resourceTypes[buildingConsumesResourceId];
                        if (!buildingConsumes[thisConsumeLevel].ContainsKey(resourceType)) {
                            buildingConsumes[thisConsumeLevel].Add(resourceType, buildingConsumesValue);
                        }
                    }
                }

                if (!reader.IsDBNull(buildingProducesLevelIndex)) {
                    // gather building produces
                    int thisProducesLevel = reader.GetInt32(buildingProducesLevelIndex);
                    if (!buildingProduces.ContainsKey(thisProducesLevel)) {
                        // save buildingProduces level
                        buildingProduces.Add(thisProducesLevel, new Dictionary<ResourceTypesModel, float>());
                    }
                    long buildingProducesResourceId = reader.GetInt64(buildingProducesResourceIdIndex);
                    if (resourceTypes.ContainsKey(buildingProducesResourceId)) {
                        float buildingProducesValue = reader.GetFloat(buildingProducesValueIndex);
                        ResourceTypesModel resourceType = resourceTypes[buildingProducesResourceId];
                        if (!buildingProduces[thisProducesLevel].ContainsKey(resourceType)) {
                            buildingProduces[thisProducesLevel].Add(resourceType, buildingProducesValue);
                        }
                    }
                }

                if (!reader.IsDBNull(buildingStoresLevelIndex)) {
                    // gather building stores
                    int thisStoresLevel = reader.GetInt32(buildingStoresLevelIndex);
                    if (!buildingStores.ContainsKey(thisStoresLevel)) {
                        // save buildingStores level
                        buildingStores.Add(thisStoresLevel, new Dictionary<ResourceTypesModel, float>());
                    }
                    long buildingStoresResourceId = reader.GetInt64(buildingStoresResourceIdIndex);
                    if (resourceTypes.ContainsKey(buildingStoresResourceId)) {
                        float buildingStoresValue = reader.GetFloat(buildingStoresValueIndex);
                        ResourceTypesModel resourceType = resourceTypes[buildingStoresResourceId];
                        if (!buildingStores[thisStoresLevel].ContainsKey(resourceType)) {
                            buildingStores[thisStoresLevel].Add(resourceType, buildingStoresValue);
                        }
                    }
                }

                // if new building type save the old data and begin new model
                long thisBuildingId = reader.GetInt64(buildingIdIndex);
                if (thisBuildingId > buildingId) {
                    // save previous model
                    //Debug.Log("Save building type: " + buildingName + " (" + buildingId + ")");
                    //buildingCosts.Add(costLevel, new Dictionary<ResourceTypesModel, float>(buildingCostsLevel));
                    //buildingConsumes.Add(consumeLevel, new Dictionary<ResourceTypesModel, float>(buildingConsumesLevel));
                    //buildingProduces.Add(produceLevel, new Dictionary<ResourceTypesModel, float>(buildingProducesLevel));
                    //buildingStores.Add(storeLevel, new Dictionary<ResourceTypesModel, float>(buildingStoresLevel));
                    BuildingTypesModel b = new BuildingTypesModel(buildingId, buildingName, buildingDescription, -1, new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingCosts), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingProduces), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingConsumes), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingStores));
                    buildingTypes.Add(buildingId, b);

                    buildingDescription = null;
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
            dbcmd.Dispose();

            if (buildingId != -1) {
                //Debug.Log("Save (last) building type: " + buildingName + " (" + buildingId + ")");
                //if (costLevel != -1) buildingCosts.Add(costLevel, new Dictionary<ResourceTypesModel, float>(buildingCostsLevel));
                //if (consumeLevel != -1) buildingConsumes.Add(consumeLevel, new Dictionary<ResourceTypesModel, float>(buildingConsumesLevel));
                //if (produceLevel != -1) buildingProduces.Add(produceLevel, new Dictionary<ResourceTypesModel, float>(buildingProducesLevel));
                //if (storeLevel != -1) buildingStores.Add(storeLevel, new Dictionary<ResourceTypesModel, float>(buildingStoresLevel));
                BuildingTypesModel b = new BuildingTypesModel(buildingId, buildingName, buildingDescription, -1, new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingCosts), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingProduces), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingConsumes), new Dictionary<int, Dictionary<ResourceTypesModel, float>>(buildingStores));
                buildingTypes.Add(buildingId, b);
            }



            ///
            /// Epochs
            /// 
            Debug.Log("Getting epochs...");
            epochs = new Dictionary<long, EpochModel>();
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT epoch.epoch_id, epoch_translation.name, epoch_translation.description FROM epoch LEFT JOIN epoch_translation ON epoch.epoch_id = epoch_translation.epoch_id WHERE epoch_translation.language_id = " + defaultLanguageId;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int epochIdIndex = reader.GetOrdinal("epoch_id");
            int epochNameIndex = reader.GetOrdinal("name");
            int epochDescriptionIndex = reader.GetOrdinal("description");
            while(reader.Read()) {
                long id = reader.GetInt64(epochIdIndex);
                string name = reader.GetString(epochNameIndex);
                string description = null;
                if (!reader.IsDBNull(epochDescriptionIndex)) description = reader.GetString(epochDescriptionIndex);
                EpochModel epoch = new EpochModel(id, name, description);
                epochs.Add(id, epoch);
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Technologies
            /// 
            Debug.Log("Getting technologies...");
            technologies = new Dictionary<long, TechnologyModel>();
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT technology.technology_id AS technology_technology_id" +
                ", technology_translation.name AS technology_translation_name, technology_translation.description AS technology_translation_description" +
                ", technology_costs_resource.resource_id AS technology_costs_resource_id, technology_costs_resource.value AS technology_costs_value" +
                ", technology_needs_predecessor.technology_id AS predecessor_technology_id" +
                ", technology_unlocks_building.building_id AS technology_unlocks_building_id" +
                ", technology_unlocks_epoch.epoch_id AS technology_unlocks_epoch_id" +
                " FROM technology" +
                " LEFT JOIN technology_translation ON technology.technology_id = technology_translation.technology_id" +
                " LEFT JOIN technology_costs_resource ON technology.technology_id = technology_costs_resource.technology_id" +
                " LEFT JOIN technology_needs_predecessor ON technology.technology_id = technology_needs_predecessor.technology_id" +
                " LEFT JOIN technology_unlocks_building ON technology.technology_id = technology_unlocks_building.technology_id" +
                " LEFT JOIN technology_unlocks_epoch ON technology.technology_id = technology_unlocks_epoch.technology_id" +
                " WHERE technology_translation.language_id = " + defaultLanguageId +
                " ORDER BY technology_technology_id ASC, technology_costs_resource_id ASC, predecessor_technology_id ASC, technology_unlocks_building_id ASC, technology_unlocks_epoch_id ASC";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int technologyIdIndex = reader.GetOrdinal("technology_technology_id");
            int technologyNameIndex = reader.GetOrdinal("technology_translation_name");
            int technologyDescriptionIndex = reader.GetOrdinal("technology_translation_description");
            int technologyCostResourceIdIndex = reader.GetOrdinal("technology_costs_resource_id");
            int technologyCostValueIndex = reader.GetOrdinal("technology_costs_value");
            int technologyPredecessorIdIndex = reader.GetOrdinal("predecessor_technology_id");
            int technologyUnlocksBuildingIdIndex = reader.GetOrdinal("technology_unlocks_building_id");
            int technologyUnlocksEpochIdIndex = reader.GetOrdinal("technology_unlocks_epoch_id");

            // current model
            long technologyId = -1;
            string technologyName = null;
            string technologyDescription = null;
            Dictionary<ResourceTypesModel, float> costs = new Dictionary<ResourceTypesModel, float>();
            List<TechnologyModel> needsPredecessors = new List<TechnologyModel>();
            Dictionary<BuildingTypesModel, int> unlocksBuildings = new Dictionary<BuildingTypesModel, int>();
            EpochModel unlocksEpoch = null;

            while (reader.Read()) {
                if (technologyId == -1 && !reader.IsDBNull(technologyIdIndex)) technologyId = reader.GetInt64(technologyIdIndex);

                if (!reader.IsDBNull(technologyCostResourceIdIndex)) {
                    long technologyCostResourceId = reader.GetInt64(technologyCostResourceIdIndex);
                    ResourceTypesModel technologyCostsResourceType = resourceTypes[technologyCostResourceId];
                    float technologyCostsValue = reader.GetFloat(technologyCostValueIndex);
                    if (!costs.ContainsKey(technologyCostsResourceType)) costs.Add(technologyCostsResourceType, technologyCostsValue);
                }

                if (!reader.IsDBNull(technologyPredecessorIdIndex)) {
                    long technologyPredecessorId = reader.GetInt64(technologyPredecessorIdIndex);
                    TechnologyModel needsPredecessor = technologies[technologyPredecessorId];
                    if (!needsPredecessors.Contains(needsPredecessor)) needsPredecessors.Add(needsPredecessor);
                }

                if (!reader.IsDBNull(technologyUnlocksBuildingIdIndex)) {
                    long technologyUnlocksBuildingId = reader.GetInt64(technologyUnlocksBuildingIdIndex);
                    BuildingTypesModel unlocksBuilding = buildingTypes[technologyUnlocksBuildingId];
                    if (!unlocksBuildings.ContainsKey(unlocksBuilding)) unlocksBuildings.Add(unlocksBuilding, 1);
                }

                if (unlocksEpoch == null && !reader.IsDBNull(technologyUnlocksEpochIdIndex)) {
                    long technologyUnlocksEpochId = reader.GetInt64(technologyUnlocksEpochIdIndex);
                    unlocksEpoch = epochs[technologyUnlocksEpochIdIndex];
                }

                // if new technology save the old data and begin new model
                long thisTechnologyId = reader.GetInt64(technologyIdIndex);
                if (thisTechnologyId > technologyId) {
                    // save previous model
                    Debug.Log("Save technology: " + technologyName + " (" + technologyId + ")");
                    TechnologyModel technology = new TechnologyModel(technologyId, technologyName, technologyDescription, costs, needsPredecessors, unlocksBuildings, unlocksEpoch);
                    technologies.Add(technologyId, technology);

                    technologyDescription = null;
                    costs = new Dictionary<ResourceTypesModel, float>();
                    needsPredecessors = new List<TechnologyModel>();
                    unlocksBuildings = new Dictionary<BuildingTypesModel, int>();
                    unlocksEpoch = null;
                }
                technologyId = thisTechnologyId;
                technologyName = reader.GetString(technologyNameIndex);
                technologyDescription = reader.GetString(technologyDescriptionIndex);
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Player
            /// 
            Debug.Log("Getting player...");
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT username, email FROM player LIMIT 1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int playerNameIndex = reader.GetOrdinal("username");
            int playerDescriptionIndex = reader.GetOrdinal("email");
            while (reader.Read()) {
                string name = reader.GetString(playerNameIndex);
                string description = null;
                if (!reader.IsDBNull(playerDescriptionIndex)) description = reader.GetString(playerDescriptionIndex);
                player = new PlayerModel(name, description);
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Player buildings
            /// 
            Debug.Log("Getting player buildings...");
            playerBuildings = new List<BuildingModel>();
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT * FROM player_has_building";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int playerBuildingIdIndex = reader.GetOrdinal("building_id");
            int playerBuildingLevelIndex = reader.GetOrdinal("level");
            int playerBuildingPosXIndex = reader.GetOrdinal("pos_x");
            int playerBuildingPosZIndex = reader.GetOrdinal("pos_z");

            while (reader.Read()) {
                long playerBuildingId = reader.GetInt64(playerBuildingIdIndex);
                int playerBuildingLevel = reader.GetInt32(playerBuildingLevelIndex);
                int playerBuildingPosX = reader.GetInt32(playerBuildingPosXIndex);
                int playerBuildingPosZ = reader.GetInt32(playerBuildingPosZIndex);
                BuildingTypesModel playerBuilding = buildingTypes[playerBuildingId];
                BuildingModel building = new BuildingModel(playerBuilding, playerBuildingLevel, playerBuildingPosX, playerBuildingPosZ);
                playerBuildings.Add(building);
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Player epoch
            /// 
            Debug.Log("Getting player epoch...");
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT * FROM player_has_epoch ORDER BY epoch_id DESC LIMIT 1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int playerEpochIdIndex = reader.GetOrdinal("epoch_id");

            while (reader.Read()) {
                long playerEpochId = reader.GetInt64(playerEpochIdIndex);
                playerEpoch = epochs[playerEpochId];
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Player resources
            /// 
            Debug.Log("Getting player resources...");
            playerResources = new Dictionary<ResourceTypesModel, float>();
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT * FROM player_has_resource";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int playerResourceIdIndex = reader.GetOrdinal("resource_id");
            int playerResourceValueIndex = reader.GetOrdinal("value");

            while (reader.Read()) {
                long playerResourceId = reader.GetInt64(playerResourceIdIndex);
                float playerResourceValue = reader.GetFloat(playerResourceValueIndex);
                ResourceTypesModel resourceType = resourceTypes[playerResourceId];
                playerResources.Add(resourceType, playerResourceValue);
            }
            reader.Close();
            dbcmd.Dispose();



            ///
            /// Player technologies
            /// 
            Debug.Log("Getting player technologies...");
            playerTechnologies = new List<TechnologyModel>();
            dbcmd = dbConn.CreateCommand();
            sqlQuery = "SELECT * FROM player_has_technology";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int playerTechnologyIdIndex = reader.GetOrdinal("technology_id");

            while (reader.Read()) {
                long playerTechnologyId = reader.GetInt64(playerTechnologyIdIndex);
                TechnologyModel playerTechnology = technologies[playerTechnologyId];
                playerTechnologies.Add(playerTechnology);
            }
            reader.Close();
            dbcmd.Dispose();
        }

        // Close database connection
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbConn.Close();
        dbConn = null;
    }
}