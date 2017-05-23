using UnityEngine;
using System.Collections.Generic;

public class Abilities : MonoBehaviour {

  private LineRenderer lr;
	private Rigidbody rb;

  public GameObject child;

	private bool targetFound = false;
	private bool grappleDeployed = false;
	private Vector3 grappleGravity;
	private Vector3 directionToGrapple;
	private Vector3 contactPoint;
  private Vector3 lastPoint;
  private Vector3 aimPoint;
  private List<Vector3> lineRendererPoints = new List<Vector3>(2);
  public LayerMask layerMask;
	private float grapplePayoutSpeed = 2;
	private float grappleRetractSpeed = 4;
	private float ropeLength;
	private float num7;
	private float distanceToGrapple;
  private RaycastHit blockedBy;

  void Start () {
		lr = GetComponent<LineRenderer>();
		rb = GetComponent<Rigidbody>();

    lineRendererPoints.Add(transform.position);
    lr.positionCount = 1;
		grappleGravity = Physics.gravity * rb.mass;
  }

  /*void OnDrawGizmosSelected() {
    Gizmos.DrawSphere(lastPoint, 1);
  }*/

	private void AddTensionDueToGravity() {
		float num = Vector3.Dot(grappleGravity, directionToGrapple);
		if (num < 0f && distanceToGrapple >= ropeLength) {
			Vector3 ropeTension = directionToGrapple * num * -1f;
			rb.AddForce(ropeTension);
		}
	}

	Vector3 MousePointInWorld() {
		Ray mousePoint = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane zPlane = new Plane(new Vector3(0, 0, 1), new Vector3(0,0,0));
		float dist;
		zPlane.Raycast(mousePoint, out dist);
		return mousePoint.GetPoint(dist);
	}

	RaycastHit RaycastToMouse() {
		Vector3 direction = MousePointInWorld() - transform.position;
		RaycastHit hitInfo;
		Physics.Raycast(transform.position, direction, out hitInfo, Mathf.Infinity, layerMask);
		return hitInfo;
	}

	void Update () {

    //grappling hook
		if (Input.GetKey("mouse 0") && !grappleDeployed) {
			rb.useGravity = true;
			grappleDeployed = false;
      lr.material.color = Color.white;
			if (RaycastToMouse().collider != null) {
        aimPoint = RaycastToMouse().point;
				lr.positionCount = 2;
				lr.SetPosition(1, aimPoint);
				targetFound = true;
			} else {
				lr.positionCount = 1;
				targetFound = false;
			}
		}

    if (Input.GetKeyUp("mouse 0")) {
			lr.material.color = Color.black;
			if (targetFound) {
				contactPoint = aimPoint;
        lineRendererPoints.Add(contactPoint);
				rb.useGravity = false;
				targetFound = false;
				grappleDeployed = true;
				ropeLength = Vector3.Distance(transform.position, contactPoint);
			} else {
				grappleDeployed = false;
				lr.positionCount = 1;
        lineRendererPoints.Clear();
        lineRendererPoints.Add(transform.position);
				targetFound = false;
				rb.useGravity = true;
			}
		}

		if (grappleDeployed) {
			rb.AddForce(grappleGravity);
			directionToGrapple = (contactPoint - transform.position).normalized;
			distanceToGrapple = (contactPoint - transform.position).magnitude;
			num7 = Vector3.Dot(rb.velocity, directionToGrapple);

			float retracting = 0;
			if (Input.GetKey("w")) {
				retracting = 1;
			} else if (Input.GetKey("s")) {
				retracting = -1;
			} else if (!Input.GetKey("w") & !Input.GetKey("s")) {
				retracting = 0;
			}

			if (num7 <= 0f && distanceToGrapple >= ropeLength) {
				rb.velocity = rb.velocity - num7 * directionToGrapple;
				AddTensionDueToGravity();
				num7 = Vector3.Dot(rb.velocity, directionToGrapple);
			}

			if (retracting == 1) {
				if (num7 < grappleRetractSpeed && distanceToGrapple != ropeLength) {
					rb.velocity = rb.velocity - num7 * directionToGrapple;
					rb.velocity = rb.velocity + grappleRetractSpeed * directionToGrapple;
					num7 = Vector3.Dot(rb.velocity, directionToGrapple);
				}
				ropeLength -= grappleRetractSpeed * Time.deltaTime;
			} else {
				if (retracting == -1) {
					ropeLength += grapplePayoutSpeed * Time.deltaTime;
				}
			}

      //begin raycasting to lastPoint
      if (!Physics.Raycast(transform.position,
          (lastPoint - transform.position),
          ((lastPoint - transform.position).normalized * ((lastPoint - transform.position).magnitude - 0.1f)).magnitude,
          layerMask) &&
          Vector3.Angle((transform.position - contactPoint), (lastPoint - contactPoint)) > 170) {
        //there is nothing blocking from the lastPoint AND it's on the negative side of the plane
        //remove index 1 from lineRenderer
        if (lineRendererPoints.Count > 2) {
          contactPoint = lastPoint;
          lineRendererPoints.RemoveAt(1);
          try {
            lastPoint = lineRendererPoints[2];
          } catch(System.ArgumentOutOfRangeException) {
            lastPoint = new Vector3(0, 0, 0);
          }
          ropeLength = (contactPoint - transform.position).magnitude;
        }
        lr.positionCount = lineRendererPoints.Count;
        lr.SetPositions(lineRendererPoints.ToArray());
      }

      //begin raycasting to contactPoint
      if (Physics.Raycast(transform.position, (contactPoint - transform.position), out blockedBy, ((contactPoint - transform.position).normalized * ((contactPoint - transform.position).magnitude - 0.1f)).magnitude, layerMask)) {
        //there is something in front of the last contact point
        //add a new point to the LineRenderer
        lastPoint = contactPoint;
        contactPoint = blockedBy.point;
        ropeLength = (contactPoint - transform.position).magnitude;
        lineRendererPoints.Insert(1, contactPoint);
        lr.positionCount = lineRendererPoints.Count;
        lr.SetPositions(lineRendererPoints.ToArray());
      }
		}
		lr.SetPosition(0, transform.position);
    lineRendererPoints[0] = transform.position;
  }
}
