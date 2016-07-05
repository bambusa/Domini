using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class SqliteController {

    private BuildingTypesController buildingTypesController;

	public SqliteController() {
        LoadGameDataFromDatabase();
    }

    public BuildingTypesController GetBuildingTypesController() {
        return buildingTypesController;
    }

    private void LoadGameDataFromDatabase() {
        buildingTypesController = new BuildingTypesController();

    }
}
