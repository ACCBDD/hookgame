using UnityEngine;

public class PlayerMove : MonoBehaviour {

	public float speed = 50;
	public float rotateForce;
	private float jumpForce = 200;
	private float distToGround;
	private float maxSpeed = 5;
	private float airSpeed = 1;

	public Rigidbody rb;

	private RaycastHit temp2;

	private Vector3 groundedPoint;

	private bool canWalk;
	private bool temp1;

	void Start () {
		rb = GetComponent<Rigidbody>();
		distToGround = GetComponent<Collider>().bounds.extents.y;
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
		//add force to move player
		if (Mathf.Abs(rb.velocity.x) < maxSpeed && canWalk) {
			rb.AddForce(new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0), ForceMode.Force);
		}

		//if in air, use airSpeed, not speed
		if (!canWalk) {
			if (!IsGrounded())
				rb.AddForce(new Vector3(Input.GetAxis("Horizontal") * airSpeed, 0, 0), ForceMode.Force);
				if (Input.GetKey(KeyCode.Q)) {
					rb.AddTorque(new Vector3(0, 0, 1) * rotateForce);
				} else if (Input.GetKey(KeyCode.E)) {
					rb.AddTorque(new Vector3(0, 0, 1) * -1 * rotateForce);
				}
		} else {
			transform.rotation = Quaternion.identity;
			if (!IsGrounded()) {
				canWalk = false;
			}
		}

		//if on ground and slowed down
		if (IsGroundedExtended(out groundedPoint) && (rb.velocity.magnitude < 5) && (rb.angularVelocity.magnitude < 0.001)) {
			transform.position = new Vector3(transform.position.x, groundedPoint.y + distToGround, transform.position.z);
			transform.rotation = Quaternion.identity;
			canWalk = true;
		}

		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
			rb.AddForce(Vector3.up * jumpForce);
		}
	}
}
