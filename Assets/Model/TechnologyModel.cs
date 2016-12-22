using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechnologyModel : MonoBehaviour {

    private long technology_id;
    private string name;
    private string description;
    private Dictionary<ResourceTypesModel, float> costs;
    private List<TechnologyModel> needsPredecessor;
    private Dictionary<BuildingTypesModel, int> unlocksBuildings;
    private EpochModel unlocksEpoch;

    public TechnologyModel(long technology_id, string name, string description, Dictionary<ResourceTypesModel, float> costs, List<TechnologyModel> needsPredecessor, Dictionary<BuildingTypesModel, int> unlocksBuildings, EpochModel unlocksEpoch) {
        this.technology_id = technology_id;
        this.name = name;
        this.description = description;
        this.costs = costs;
        this.needsPredecessor = needsPredecessor;
        this.unlocksBuildings = unlocksBuildings;
        this.unlocksEpoch = unlocksEpoch;
    }

    private long GetId() {
        return technology_id;
    }

    private string GetName() {
        return name;
    }

    private string GetDescription() {
        return description;
    }

    private Dictionary<ResourceTypesModel, float> GetCosts() {
        return costs;
    }

    private List<TechnologyModel> GetPredecessors() {
        return needsPredecessor;
    }

    private Dictionary<BuildingTypesModel, int> GetUnlockBuildings() {
        return unlocksBuildings;
    }

    private EpochModel GetUnlockEpoch() {
        return unlocksEpoch;
    }
}
