﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FOVEditor : Editor {

	void OnSceneGUI() {
		FieldOfView fow = (FieldOfView)target;
		Handles.color = Color.white;
		Handles.DrawWireArc(fow.transform.position, new Vector3(0, 0, 1), Vector3.up, 360, fow.viewRadius);
		Vector3 viewAngleA = fow.DirFromAngle(fow.viewAngle / 2, false);
		Vector3 viewAngleB = fow.DirFromAngle(-fow.viewAngle / 2, false);

		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);
	}
}
