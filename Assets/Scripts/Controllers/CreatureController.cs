using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public float magnitude = 4;
    public float speed = 1;
    public float seeingRange = 2;
    public string id;

    private Rigidbody rb;
    private bool onFloor = false;
    private bool isActing = false;
    private Vector3 nextForce = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Update is called once per frame
    void Update()
    {
        float input = GetDistanceToNextObstacleNormalized();
        GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, input);
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (onFloor && nextForce != Vector3.zero)
        {
            rb.velocity = nextForce;
            nextForce = Vector3.zero;
        }

        if (!isActing && onFloor)
        {
            Act();
        }
    }

    void Act()
    {
        isActing = true;
        float input = GetDistanceToNextObstacleNormalized();
        ActInputDto inputDto = new ActInputDto(new float[] { input });
        StartCoroutine(GannHttpClient.Act(id, inputDto, onActSuccess));
    }

    void onActSuccess(ActOutputDto outputDto)
    {
        nextForce = Vector3.up * magnitude * outputDto.outputs[0];
        isActing = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Floor"))
        {
            onFloor = true;
        }

        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            Die();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Floor"))
        {
            onFloor = false;
        }
    }

    void Die()
    {
        float totalDistanceTravelled = transform.position.z;
        float fitness = Mathf.Pow(totalDistanceTravelled, 2);
        StartCoroutine(GannHttpClient.UpdateFitness(id, new UpdateFitnessDto(fitness), OnUpdateFitnessSuccess));
    }

    private void OnUpdateFitnessSuccess(EmptyDto _)
    {
        Destroy(gameObject);
    }

    private GameObject GetNextObstacle()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        GameObject closestObstacle = null;

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle.transform.position.z < transform.position.z)
            {
                continue;
            }

            if (closestObstacle == null)
            {
                closestObstacle = obstacle;
            }
            else if (obstacle.transform.position.z < closestObstacle.transform.position.z)
            {
                closestObstacle = obstacle;
            }
        }

        return closestObstacle;
    }

    private float GetDistanceToNextObstacle()
    {
        GameObject closestObstacle = GetNextObstacle();

        if (closestObstacle != null)
        {
            return closestObstacle.transform.position.z - transform.position.z;
        }

        return Mathf.Infinity;
    }

    private float GetDistanceToNextObstacleNormalized()
    {
        float distanceToNextObstacle = GetDistanceToNextObstacle();
        // return values from 0 to 1
        // 0 is far and 1 is close
        return distanceToNextObstacle > seeingRange ? 0 : (1 - distanceToNextObstacle/seeingRange);
    }
}
