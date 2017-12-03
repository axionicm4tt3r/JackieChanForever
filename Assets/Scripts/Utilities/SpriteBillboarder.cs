using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarder : MonoBehaviour {

	Vector3 directionToCamera;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPosition = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).transform.position;
		var dir = playerPosition - transform.position;
		var angle = Mathf.Atan2(dir.z, dir.x);
		if (angle < 0.0)
			angle += 360.0f;
		var spriteIndex = Mathf.RoundToInt(angle / 45.0f);

		Vector3 playerPositionXZ = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
		Quaternion facePlayerAngles = Quaternion.LookRotation(transform.position - playerPositionXZ, Vector3.up);

		transform.rotation = facePlayerAngles;//Quaternion.Euler(currentEulerAngles.x, currentEulerAngles.y, currentEulerAngles.z);
	}
}
