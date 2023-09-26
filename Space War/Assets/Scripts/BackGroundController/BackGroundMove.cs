using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BackGroundMove : MonoBehaviour {

	[Tooltip("the mesh of the back ground")]
    MeshRenderer mesh;

	void Start () {
        //get the mesh component of the background
        mesh = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        //Make the pixel on the mesh move
        mesh.material.mainTextureOffset += new Vector2(0,Time.deltaTime*-0.5f);
	}
}
