﻿using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.LookAt(Camera.main.transform);
	}
}
