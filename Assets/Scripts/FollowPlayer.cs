using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public Transform target;
	private Vector3 targetPos;
	private float distanceFromPlayer = 44;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		targetPos = new Vector3(target.position.x, target.position.y, -distanceFromPlayer);
		transform.position = targetPos;

		if (distanceFromPlayer > 1 && Input.GetAxis("Mouse ScrollWheel") > 0) {
			distanceFromPlayer -= 2;
		}

		if (distanceFromPlayer < 60 && Input.GetAxis("Mouse ScrollWheel") < 0) {
			distanceFromPlayer += 2;
		}

	}
}
