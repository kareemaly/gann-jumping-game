using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GannController : MonoBehaviour
{
    public GameObject creaturePrefab;
    public GameObject obstaclePrefab;

    private List<GameObject> creatures = new List<GameObject>();
    private bool isGenerating = false;

    // Start is called before the first frame update
    void Start()
    {
        GenerateFirstPopulation();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject aliveCreature = creatures.Find(c => c != null);
        bool allDead = creatures.TrueForAll(c => c == null);

        if (allDead && !isGenerating)
        {
            GenerateNextPopulation();
        }

        if (aliveCreature != null)
        {
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                aliveCreature.transform.position.z - 1
            );
        }
    }

    private void GenerateFirstPopulation()
    {
        isGenerating = true;
        StartCoroutine(GannHttpClient.FirstPopulation(GeneratePopulation));
    }

    private void GenerateNextPopulation()
    {
        isGenerating = true;
        StartCoroutine(GannHttpClient.NextPopulation(GeneratePopulation));
    }

    private void GeneratePopulation(PopulationDto population)
    {
        int noOfCreatures = population.creatures.Count;

        for (int i = 0; i < noOfCreatures; i++)
        {
            Vector3 creaturePos = new Vector3((noOfCreatures/2 - i) * 0.2f, 0.2f, 0);
            GameObject creature = Instantiate(creaturePrefab, creaturePos, Quaternion.identity);
            creature.GetComponent<CreatureController>().id = population.creatures[i].id;
            creatures.Add(creature);
        }

        GenerateObstacles();
        isGenerating = false;
    }

    private void GenerateObstacles()
    {
        int noOfObstacles = 20;

        for (int i = 0; i < noOfObstacles; i++)
        {
            Vector3 obstaclePos = new Vector3(0, 0.05f, (i + 1) * 4);
            Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity);
        }
    }
}
