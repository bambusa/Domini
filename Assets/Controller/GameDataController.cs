using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load all game data from database and prepare the models
/// </summary>
public class GameDataController : MonoBehaviour {

    private static BuildingTypesController buildingTypesController;
    private SqliteController sqliteController;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        sqliteController = new SqliteController(Application.dataPath);
        buildingTypesController = sqliteController.GetBuildingTypesController();
        
        SceneManager.LoadScene("MainScene");
    }

    public static BuildingTypesController GetBuildingTypesController() {
        return buildingTypesController;
    }
}
