using UnityEngine;
using System.Collections;

public class SpawnZombies : MonoBehaviour {

	private float spawnTimer = 5f;
	public GameObject spawnItem;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0) {
			spawnTimer = 5f;
			Instantiate(spawnItem, transform.position, Quaternion.identity);
		}
	}
}
