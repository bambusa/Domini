using UnityEngine;
using System.Collections;

/// <summary>
/// General button click listener for the menu overlay
/// </summary>
public class HudButtonListener : MonoBehaviour {

    public GameObject buildingLayer;
    public GameObject buildingMenuPanel;

    BuildingLayerController buildingController;
    Renderer buildingMenuPanelRenderer;

    /// <summary>
    /// Check needed references to BuildingLayer for placing buildings
    /// </summary>
    void onStart() {
        Debug.Log("Hud");
        if (buildingLayer == null) {
            Debug.LogError("BuildingLayer not set");
        }
        buildingController = buildingLayer.GetComponent<BuildingLayerController>();
        if (buildingController == null) {
            Debug.LogError("BuildingController not found");
        }
    }

    /// <summary>
    /// Open building menu when build button gets clicked
    /// </summary>
    public void onClickBuildButton() {
        Debug.Log("BuildButton clicked");
        if (!buildingMenuPanel.activeSelf) {
            buildingMenuPanel.SetActive(true);
        } else if (buildingMenuPanel.activeSelf) {
            buildingMenuPanel.SetActive(false);
        }
    }
}
