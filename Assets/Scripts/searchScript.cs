using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchScript : MonoBehaviour
{
    public float speed = 0.01f;
    public float foodRequirment = 1.0f;                                                      // food required to survive | generate new offspring
    public float idleDistance = 3.0f;
    public float radius = 7.3f;
    public float liftUp = 0.26f;
    public float mutateChance = 0.2f;
    public float speedMutationRange = 0.0025f;
    public float radiusMutationRange = 0.0025f;
    public int speciesIndex = -1;
    public string speciesFood;
    public GameObject spawnOnDie;
    public bool isPredator;

    private Vector3 targetDest;
    private CapsuleCollider searchCollider;
    private bool hasObjective = false;
    private GameObject targetObject;
    private int foodEaten = 0;
    private List<GameObject> activeCollisions = new List<GameObject>();
    private simulationManager simulationManagerScript;
                                                                                            // keeping active collisions list up to date (SEARCH COLLIDER IN CHILDISH)
    public void CollisionEnter(Collider _collision)
    {
        if(_collision.gameObject.CompareTag(speciesFood) || _collision.gameObject.CompareTag("Trash"))
            activeCollisions.Add(_collision.gameObject);
    }

    public void CollisionExit(Collider _collision)
    {
        if (_collision.gameObject.CompareTag(speciesFood) || _collision.gameObject.CompareTag("Trash"))
            activeCollisions.Remove(_collision.gameObject);
    }
    
    public void Mutate(GameObject _creature)                                                                    // species MUTATION
    {   
        if(Random.Range(0f, 1f) < mutateChance)                                                                 // if need to mutate speed
        {
            float _mutationAmount;
            _mutationAmount = Random.Range(-speedMutationRange, speedMutationRange);
            _creature.GetComponent<searchScript>().speed += _mutationAmount;
            _creature.GetComponent<searchScript>().foodRequirment *= 1 + (_mutationAmount / speed);

        }

        if (Random.Range(0f, 1f) < mutateChance)                                                                 // if need to mutate lookRadius
        {
            float _mutationAmount;
            _mutationAmount = Random.Range(-radiusMutationRange, radiusMutationRange);
            if (!((_creature.GetComponentInChildren<CapsuleCollider>().radius + _mutationAmount) <= 0))
            {
                _creature.GetComponentInChildren<CapsuleCollider>().radius += _mutationAmount;
                _creature.GetComponent<searchScript>().foodRequirment *= 1 + (_mutationAmount / radius);
            }

        }
    }

    public void StageEnd()                                                                                      // spawning an offspting or dying
    {
        float survivalCoef;
        GameObject offspring;
        survivalCoef = (((float) foodEaten) / foodRequirment) - 1f;
        if(survivalCoef < 0.0f)                                                                                    // die if didn't eat enough OR NOT LUCKY
        {
            if (Random.Range(0f, 1f) > (survivalCoef + 1f)) 
            {
                simulationManagerScript.creaturesList.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
        } else
        {
            for(int g = 0; g < (int) survivalCoef; ++g)                                                             // spawn offspting based on food eaten
            {
                offspring = Instantiate(this.gameObject, simulationManagerScript.creaturesParent);
                Mutate(offspring);
                offspring.name = "Creature";
                simulationManagerScript.creaturesList.Add(offspring);
            }
            if(Random.Range(0.0f, 1.0f) < (survivalCoef - (int) survivalCoef))                                      // spawn an offspring if LUCKY enought
            {
                offspring = Instantiate(this.gameObject, simulationManagerScript.creaturesParent);
                Mutate(offspring);
                offspring.name = "Creature";
                simulationManagerScript.creaturesList.Add(offspring);
            }
        }
    }

    public void Cleanup()                                                                                             // reset the game object
    {
        foodEaten = 0;
        gameObject.GetComponent<circlePositionRandomizer>().RandomizePosition();
    }
    
    private void OnTriggerStay(Collider other)                                                                         // small collider check (EAT COLLIDER)
    {
        GameObject _spawnObject;
        if (hasObjective)
        {
            if (other.gameObject.CompareTag(speciesFood))
            {
                ++foodEaten;
                activeCollisions.Remove(other.gameObject);
                simulationManagerScript.foodList.Remove(other.gameObject);
                hasObjective = false;
                Destroy(other.gameObject);
                if (spawnOnDie != null)                                                                              // if it has object TO SPAWN ON DIE -> spawn it
                {
                    _spawnObject = Instantiate(spawnOnDie, simulationManagerScript.foodParent);
                    simulationManagerScript.foodList.Add(_spawnObject);
                    _spawnObject.transform.localPosition = this.transform.localPosition;
                }
            } else if(other.gameObject.CompareTag("Trash"))                                                         // if the TRASH WAS EATEN
            {
                simulationManagerScript.foodList.Remove(other.gameObject);
                hasObjective = false;
                Destroy(other.gameObject);
                simulationManagerScript.creaturesList.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    private GameObject SearchForClosest()                                                               // searches the closest food in List of active Collisions, LIST SOULD NOT BE EMPTY!
    {
        Vector2 _creaturePos = new Vector2(transform.position.x, transform.position.z);
        Vector2 _foodPos = new Vector2();
        float _currentDist;
        float _minDist = 3.4e20f;
        GameObject _closestFood = activeCollisions[0];
        
        for (int i = 0; i < activeCollisions.Count; ++i)
        {
            _foodPos.x = activeCollisions[i].transform.position.x;
            _foodPos.y = activeCollisions[i].transform.position.z;
            _currentDist = Mathf.Sqrt(((_foodPos.x - _creaturePos.x) * (_foodPos.x - _creaturePos.x)) + ((_foodPos.y - _creaturePos.y) * (_foodPos.y - _creaturePos.y))); // distance from creature to the food
            if (_currentDist < _minDist)
            {
                _minDist = _currentDist;
                _closestFood = activeCollisions[i];
            }
        }

        return _closestFood;
    }
                                                                                                                   // function for generating idling
    private Vector3 IdleWalkGen(float _distance)                                                                            
    {
        Vector3 _targetDest;
        _distance /= 2;
                                                                                                                    // generate a new destination & regenerate if it's outside of the circle
        do
        {
            _targetDest = new Vector3(Random.Range(-_distance, _distance), 0.0f, Random.Range(-_distance, _distance));
            _targetDest += transform.localPosition;
        } while ((Mathf.Sqrt((_targetDest.x * _targetDest.x) + (_targetDest.z * _targetDest.z))) > radius);

        return _targetDest;
    }
    
    private void Awake()
    {
        searchCollider = GetComponentInChildren<CapsuleCollider>();
        targetDest = transform.localPosition;
        simulationManagerScript = GameObject.Find("Simulation Manager").GetComponent<simulationManager>();
        targetDest = IdleWalkGen(idleDistance);                                                                     // first Idling destination
    }

    private void FixedUpdate()
    {                                                                                                                // if creature is not going for food
        if(hasObjective)
        {
            if(targetObject == null)
                hasObjective = false;
            else
                if(isPredator)
                    targetDest = targetObject.transform.localPosition;
        } else
        {
            activeCollisions.RemoveAll(item => item == null);                                                        // remove all destroyed food from list
            if (!activeCollisions.Count.Equals(0))                                                                   // if some food is in the visibility range
            {
                targetObject = SearchForClosest();
                targetDest = targetObject.transform.localPosition;
                hasObjective = true;
            }
            if (transform.localPosition == targetDest)                                                                    // if creature has came to it's idling destination
            {
                targetDest = IdleWalkGen(idleDistance);                                                              // create a new idling destination
                                                                                                                        
            }
            
        }
                                                                                                                        // making creature face it's direction
        transform.LookAt(new Vector3(targetDest.x, liftUp, targetDest.z), Vector3.up);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetDest, speed);
        
    }
}
