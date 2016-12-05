using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load all game data from database and prepare the models
/// </summary>
public class GameDataController : MonoBehaviour {

    private SqliteController sqliteController;
    public static Dictionary<long, BuildingTypesModel> buildingTypes;
    public static Dictionary<long, ResourceTypesModel> resourceTypes;
    public static Dictionary<long, EpochModel> epochs;
    public static Dictionary<long, TechnologyModel> technologies;
    public static List<BuildingModel> playerBuildings;
    public static Dictionary<ResourceTypesModel, float> playerResources;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        sqliteController = new SqliteController(Application.dataPath);
        buildingTypes = sqliteController.GetBuildingTypes();
        resourceTypes = sqliteController.GetResourceTypes();
        epochs = sqliteController.GetEpochs();
        technologies = sqliteController.GetTechnologies();
        playerBuildings = sqliteController.GetPlayerBuildings();
        playerResources = sqliteController.GetPlayerResources();
        
        SceneManager.LoadScene("MainScene");
    }
}
