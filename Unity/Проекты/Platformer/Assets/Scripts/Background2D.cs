using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background2D : MonoBehaviour
{

	public Transform BG1;
	public float speedBG1 = 0.005f;

	public Transform BG2;
	public float speedBG2 = 0.010f;

	public Transform BG3;
	public float speedBG3 = 0.015f;

	private Camera cam;

	void Awake()
	{
		cam = GetComponent<Camera>();
	}

	void Start()
	{
	}

	Vector3 GetVector(Vector3 position, float speed)
	{
		float posX = position.x;
		posX += cam.velocity.x * speed;
		return new Vector3(posX, position.y, position.z);
	}

	void Update()
	{
		if (BG1) BG1.position = GetVector(BG1.position, speedBG1);
		if (BG2) BG2.position = GetVector(BG2.position, speedBG2);
		if (BG3) BG3.position = GetVector(BG3.position, speedBG3);
	}
}
