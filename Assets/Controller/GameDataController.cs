using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class GameDataController : MonoBehaviour {

    private SqliteController sqliteController;

	// Use this for initialization
	void Start () {
        sqliteController = new SqliteController();
	}
}
