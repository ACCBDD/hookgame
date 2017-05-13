using UnityEngine;
using System.Collections;

public class ZombieMove : MonoBehaviour {

	public Transform target;
	private Rigidbody rb;

	private bool canWalk;
	private bool temp1;

	private RaycastHit temp2;

	private Vector3 groundedPoint;

	private float speed = 5;
	private float distToGround;
	private float airSpeed = 1;
	private float maxSpeed = 5;
	private float jumpForce = 30;

	void Start () {
		target = GameObject.Find("Player").GetComponent("Transform") as Transform;
		distToGround = GetComponent<Collider>().bounds.extents.y;
		rb = GetComponent<Rigidbody>();
		GetComponent<Renderer>().material.color = Color.HSVToRGB(0.32f, 1.0f, Random.Range(0.31f, 0.44f));
	}

	bool IsGrounded() {
		 return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.3f);
	}

	bool IsGroundedExtended(out Vector3 hitPosition) {
		 temp1 = Physics.Raycast(transform.position, -Vector3.up, out temp2, distToGround + 0.3f);
		 hitPosition = temp2.point;
		 return temp1;
	}

	void Update () {

		if (canWalk) {
			transform.rotation = Quaternion.identity;
			if (target.position.y - 2 > transform.position.y && Mathf.Abs(target.position.x - transform.position.x) < 5) {
				rb.AddForce(Vector3.up * jumpForce);
			}
			if (!IsGrounded()) {
				canWalk = false;
			}
		}

		if (IsGroundedExtended(out groundedPoint) && (rb.velocity.magnitude < 5) && (rb.angularVelocity.magnitude < 0.001)) {
			transform.position = new Vector3(transform.position.x, groundedPoint.y + distToGround, transform.position.z);
			transform.rotation = Quaternion.identity;
			canWalk = true;
		}

		if (Mathf.Abs(rb.velocity.x) < maxSpeed) {
			if (canWalk) {
				if (target.position.x > transform.position.x) {
					rb.AddForce(new Vector3(speed, 0, 0), ForceMode.Force);
				} else if (target.position.x < transform.position.x) {
					rb.AddForce(new Vector3(-speed, 0, 0), ForceMode.Force);
				}
			} else {
				if (target.position.x > transform.position.x) {
					rb.AddForce(new Vector3(airSpeed, 0, 0), ForceMode.Force);
				} else if (target.position.x < transform.position.x) {
					rb.AddForce(new Vector3(-airSpeed, 0, 0), ForceMode.Force);
				}
			}

		}

		if (transform.position.y < -50) {
			Object.Destroy(gameObject);
		}

	}

	/*void OnCollisionStay(Collision collisionInfo) {
    if (collisionInfo.gameObject.isStatic && target.position.y > transform.position.y) {
			rb.AddForce(Vector3.up * jumpForce);
		}
	}*/
}
