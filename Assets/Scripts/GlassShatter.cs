using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassShatter : MonoBehaviour {

	public float breakThreshold;
	public GameObject shatterPrefab;
	public float glassLength, numShards, shardDist;
	public bool isHorizontal;

	void Start () {
		if (transform.localScale.x > transform.localScale.y) {
			glassLength = transform.localScale.x;
			isHorizontal = true;
		} else {
			glassLength = transform.localScale.y;
			isHorizontal = false;
		}
		numShards = Mathf.Floor(glassLength);
		shardDist = glassLength/numShards;
	}
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if (Mathf.Abs(other.attachedRigidbody.velocity.x) > breakThreshold) {

			Destroy(gameObject);
			for (int i = 0; i<numShards; i++) {
				Vector3 pos;
				if (isHorizontal) {
					pos = new Vector3((-1*glassLength/2) + i*shardDist, 0, 0);
				} else {
					pos = new Vector3(0, (-1*glassLength/2) + i*shardDist, 0);
				}
				Instantiate(shatterPrefab, transform.parent.TransformPoint(pos), Quaternion.identity, transform.parent);
			}
		}
	}
}
