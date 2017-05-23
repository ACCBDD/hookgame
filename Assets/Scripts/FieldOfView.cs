using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
	public float meshResolution;
	public float edgeDistThreshold;
	public float maskCutawayDist;

	public bool debugLines;

	public int edgeSolveIterations;

	public LayerMask obstacleMask;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	void Start() {
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
	}

	void Update() {
		DrawFieldOfView();
	}

	public Vector3 DirFromAngle(float angleInDeg, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDeg += transform.eulerAngles.z;
		}
		return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), Mathf.Cos(angleInDeg * Mathf.Deg2Rad), 0);
	}

	void DrawFieldOfView() {
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3>();
		ViewCastInfo oldViewCast = new ViewCastInfo();

		for (int i = 0; i <= stepCount; i++) {
			float angle = transform.eulerAngles.z - viewAngle/2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);

			if (i > 0) {
				bool edgeThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeThresholdExceeded)) {
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero) {
						viewPoints.Add(edge.pointA);
					}
					if (edge.pointB != Vector3.zero) {
						viewPoints.Add(edge.pointB);
					}
				}
			}
			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount-2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertexCount-1; i++) {
			if (debugLines)
				Debug.DrawLine(transform.position, viewPoints[i]);
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + (transform.InverseTransformPoint(viewPoints[i]) - transform.position).normalized * maskCutawayDist;

			if (i < vertexCount - 2) {
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}

	EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast) {
		float minAngle = minCast.angle;
		float maxAngle = maxCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeSolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

			bool edgeThresholdExceeded = Mathf.Abs(minCast.dst - newViewCast.dst) > edgeDistThreshold;
			if (newViewCast.hit == minCast.hit && !edgeThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo(minPoint, maxPoint);
	}

	ViewCastInfo ViewCast(float globalAngle) {
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;
		if (Physics.Raycast(transform.position + new Vector3(0,0,-3), dir, out hit, viewRadius, obstacleMask)) {
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		} else {
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	public struct ViewCastInfo {
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle) {
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}
	}

	public struct EdgeInfo {
		public Vector3 pointA, pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
			pointA = _pointA;
			pointB = _pointB;
		}
	}
}
