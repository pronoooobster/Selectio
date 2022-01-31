using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class simulationManager : MonoBehaviour
{
    public List<GameObject> creaturesList = new List<GameObject>();                           // list of ACTIVE creatures
    public List<GameObject> foodList = new List<GameObject>();                                // list of ACTIVE food
    public List<GameObject> initObjects_prefabs = new List<GameObject>();                     // list of objects to SPAWN
    public List<int> initObjects_spawnCount = new List<int>();                                // number of each object to SPAWN

    public float simulationSpeed = 1f;
    public double globalClock_seconds = 0f;
    public float stageTime_seconds = 10f;
    public int generation = 0;
    public float startSpeed = 0.015f;
    public float startSearchRadius = 7.2f;
    private float pollutionLevel = 0f;

    public Transform creaturesParent;
    public Transform foodParent;

    private List<GameObject> _copyCreaturesList = new List<GameObject>();
    private UICommands UIScript;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Slider pollutionLevelSlider;
    [SerializeField] private TextMeshProUGUI generationText;
    [SerializeField] private List<GameObject> trashPrefabs; 
    private List<float> speedMut = new List<float> { 0f, 0f };
    private List<float> areaMut = new List<float> { 0f, 0f };
    private List<float> population = new List<float> { 0f, 0f };
    private List<float> foodReq = new List<float> { 0f, 0f };
    private List<string> _foodTypes = new List<string> { "FoodApple", "FoodLemon", "Creature" };

    public void TimeScaleChange()
    {
        simulationSpeed = timeScaleSlider.value;
        Time.timeScale = timeScaleSlider.value;
    }

    public void LoadSettings()
    {
        initObjects_spawnCount[0] = menuCommands.C1Spawn_i;
        initObjects_spawnCount[1] = menuCommands.C2Spawn_i;
        initObjects_spawnCount[2] = menuCommands.F1Spawn_i;
        initObjects_spawnCount[3] = menuCommands.F2Spawn_i;

        initObjects_prefabs[0].GetComponent<searchScript>().mutateChance = menuCommands.C1Mut_f;
        initObjects_prefabs[1].GetComponent<searchScript>().mutateChance = menuCommands.C2Mut_f;

        initObjects_prefabs[0].GetComponent<searchScript>().speciesFood = _foodTypes[menuCommands.C1Eats_i];
        if (menuCommands.C1Eats_i == 2)
        {
            initObjects_prefabs[0].GetComponent<searchScript>().isPredator = true;
            initObjects_prefabs[0].layer = 8;
            foreach (Transform child in initObjects_prefabs[0].GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Predators"); 
            }
            initObjects_prefabs[0].tag = "Predator";
        }
        else
        {
            initObjects_prefabs[0].GetComponent<searchScript>().isPredator = false;
            initObjects_prefabs[0].layer = 6;
            foreach (Transform child in initObjects_prefabs[0].GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Creatures");
            }
            initObjects_prefabs[0].tag = "Creature";
        }
        initObjects_prefabs[1].GetComponent<searchScript>().speciesFood = _foodTypes[menuCommands.C2Eats_i];
        if (menuCommands.C2Eats_i == 2)
        {
            initObjects_prefabs[1].GetComponent<searchScript>().isPredator = true;
            initObjects_prefabs[1].layer = 8;
            foreach (Transform child in initObjects_prefabs[1].GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Predators");
            }
            initObjects_prefabs[1].tag = "Predator";
        }
        else
        {
            initObjects_prefabs[1].GetComponent<searchScript>().isPredator = false;
            initObjects_prefabs[1].layer = 6;
            foreach (Transform child in initObjects_prefabs[1].GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Creatures");
            }
            initObjects_prefabs[1].tag = "Creature";
        }

        switch (menuCommands.C1Creates_i)
        {
            case 1:
                initObjects_prefabs[0].GetComponent<searchScript>().spawnOnDie = initObjects_prefabs[2];
                break;
            case 2:
                initObjects_prefabs[0].GetComponent<searchScript>().spawnOnDie = initObjects_prefabs[3];
                break;
            default:
                initObjects_prefabs[0].GetComponent<searchScript>().spawnOnDie = null;
                break;
        }

        switch (menuCommands.C2Creates_i)
        {
            case 1:
                initObjects_prefabs[1].GetComponent<searchScript>().spawnOnDie = initObjects_prefabs[2];
                break;
            case 2:
                initObjects_prefabs[1].GetComponent<searchScript>().spawnOnDie = initObjects_prefabs[3];
                break;
            default:
                initObjects_prefabs[1].GetComponent<searchScript>().spawnOnDie = null;
                break;
        }

    }

    public void InitSpawn()                                                                     // spawn all initial objects
    {
        GameObject _spawnObject;
        for (int i = 0; i < initObjects_spawnCount.Count; ++i)
            for (int g = 0; g < initObjects_spawnCount[i]; ++g)
                if (initObjects_prefabs[i].CompareTag("Creature") || initObjects_prefabs[i].CompareTag("Predator"))
                {
                    _spawnObject = Instantiate(initObjects_prefabs[i], creaturesParent);
                    creaturesList.Add(_spawnObject);
                }   
                else {
                    _spawnObject = Instantiate(initObjects_prefabs[i], foodParent);
                    foodList.Add(_spawnObject);
                    _spawnObject.GetComponent<circleSurfacePositionRandomizer>().RandomizePosition();
                }

    }

    public void FoodStageSpawn()                                                                 // spawn new FOOD AT THE END OF STAGE    
    {
        GameObject _spawnObject;
        pollutionLevel = pollutionLevelSlider.value;                                              // get set pollution value
        for (int i = 0; i < initObjects_spawnCount.Count; ++i)
        {
            if (initObjects_prefabs[i].CompareTag("Creature") || initObjects_prefabs[i].CompareTag("Predator"))
                continue;
            for (int g = 0; g < initObjects_spawnCount[i]; ++g)
            {
                if(Random.Range(0f, 100f) < pollutionLevel)                                          // if not lucky -> spawn trash
                    _spawnObject = Instantiate(trashPrefabs[Random.Range(0, 4)], foodParent);
                else
                    _spawnObject = Instantiate(initObjects_prefabs[i], foodParent);
                foodList.Add(_spawnObject);
                _spawnObject.GetComponent<circleSurfacePositionRandomizer>().RandomizePosition();
            }
        }
    }

    private void FixedUpdate()
    {
        Time.timeScale = simulationSpeed;                                                          // GLOBAL TIME update
        globalClock_seconds += Time.fixedDeltaTime;

        if(globalClock_seconds >= stageTime_seconds)                                               // NEXT STAGE
        {
            speedMut = new List<float> { 0f, 0f};
            areaMut = new List<float> { 0f, 0f };
            foodReq = new List<float> { 0f, 0f };
            population = new List<float> { 0f, 0f };

            creaturesList.RemoveAll(item => item == null);                                          // REMOVE ALL EATEN creatures
            _copyCreaturesList = creaturesList.GetRange(0, creaturesList.Count);

            for (int i = 0; i < _copyCreaturesList.Count; ++i)
            {   
                speedMut[_copyCreaturesList[i].GetComponent<searchScript>().speciesIndex] += _copyCreaturesList[i].GetComponent<searchScript>().speed;              // AVG mutation calculation                                     
                areaMut[_copyCreaturesList[i].GetComponent<searchScript>().speciesIndex] += _copyCreaturesList[i].GetComponentInChildren<CapsuleCollider>().radius;
                foodReq[_copyCreaturesList[i].GetComponent<searchScript>().speciesIndex] += _copyCreaturesList[i].GetComponent<searchScript>().foodRequirment;
                ++population[_copyCreaturesList[i].GetComponent<searchScript>().speciesIndex];

                _copyCreaturesList[i].GetComponent<searchScript>().StageEnd();
            }

            for (int i = 0; i < speedMut.Count; ++i)
            {
                speedMut[i] = (speedMut[i] / population[i] - startSpeed) * 100.0f / startSpeed;
                areaMut[i] = (areaMut[i] / population[i] - startSearchRadius) * 100.0f / startSearchRadius;
                foodReq[i] = foodReq[i] / population[i];
            }

            

            _copyCreaturesList.Clear();
            _copyCreaturesList = creaturesList.GetRange(0, creaturesList.Count);
            for (int i = 0; i < _copyCreaturesList.Count; ++i)
            {
                _copyCreaturesList[i].GetComponent<searchScript>().Cleanup();
            }
            _copyCreaturesList.Clear();
            FoodStageSpawn();
            globalClock_seconds = 0.0f;
            ++generation;
            generationText.text = "Gen: " + generation;
            for (int i = 0; i < speedMut.Count; ++i)                                            // sending a command to update graphs
            {
                UIScript.GraphUpdate((i * 3), "Speed", generation, speedMut[i]);
                UIScript.GraphUpdate((i * 3), "Search area", generation, areaMut[i]);
                UIScript.GraphUpdate((i * 3) + 1, "Population", generation, population[i]);
                UIScript.GraphUpdate((i * 3) + 2, "Food demand", generation, foodReq[i]);
            }
        }
    }

    private void Start()
    {
        LoadSettings();
        InitSpawn();
        Time.timeScale = 1.0f;
        UIScript = GameObject.Find("UI").GetComponent<UICommands>();
    }
}
