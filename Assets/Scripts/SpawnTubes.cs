using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTubes : MonoBehaviour
{
    public GameObject Tubes;
    public GameObject Spawner1;
    public GameObject Spawner2;
    public float minHeight;
    public float maxHeight;
    private void Start()
    {
        Spawner1.transform.position = new Vector3(Spawner1.transform.position.x, Random.Range(minHeight, maxHeight), Spawner1.transform.position.z);
        Instantiate(Tubes, Spawner1.transform);
        Spawner2.transform.position = new Vector3(Spawner2.transform.position.x, Random.Range(minHeight, maxHeight), Spawner2.transform.position.z);
        Instantiate(Tubes, Spawner2.transform);
    }
}
