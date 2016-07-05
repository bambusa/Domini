using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine; // TODO: Delete

public class SqliteController {

    private BuildingTypesController buildingTypesController;
    private IDbConnection dbConn;

    public SqliteController(string dataPath) {
        string conn = "URI=file:" + dataPath + "/Plugins/SQLite/Domini0_3.db"; // Path to database.
        dbConn = (IDbConnection)new SqliteConnection(conn);
    }

    public BuildingTypesController GetBuildingTypesController() {
        return LoadGameDataFromDatabase();
    }

    public BuildingTypesController LoadGameDataFromDatabase() {
        //Debug.Log("LoadGameDataFromDatabase");

        // Open database connection
        buildingTypesController = new BuildingTypesController();
        dbConn.Open();

        // Get BuildingTypes
        IDbCommand dbcmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT * FROM building;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read()) {
            //int building_id = reader.GetInt32(0);
            string name = reader.GetString(1);

            BuildingTypesModel b = new BuildingTypesModel(name);
            buildingTypesController.AddBuildingType(b);
            //Debug.Log("Found building " + name);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;

        // Close database connection
        dbConn.Close();
        dbConn = null;
        return buildingTypesController;
    }
}
