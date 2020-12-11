using UnityEngine;
using System.Collections;

public class RayScan : MonoBehaviour
{

	public string targetTag = "Player";
	public int rays = 6;
	public int distance = 15;
	public float angle = 20;
	public Vector3 offset;
	private Transform target;

	void Start()
	{
		target = GameObject.FindGameObjectWithTag(targetTag).transform;
	}

	bool GetRaycast(Vector3 dir)
	{
		bool result = false;
		RaycastHit hit = new RaycastHit();
		Vector3 pos = transform.position + offset;
		if (Physics.Raycast(pos, dir, out hit, distance))
		{
			if (hit.transform == target)
			{
				result = true;
				Debug.DrawLine(pos, hit.point, Color.green);
			}
			else
			{
				Debug.DrawLine(pos, hit.point, Color.blue);
			}
		}
		else
		{
			Debug.DrawRay(pos, dir * distance, Color.red);
		}
		return result;
	}

	bool RayToScan()
	{
		bool result = false;
		bool a = false;
		bool b = false;
		float j = 0;
		for (int i = 0; i < rays; i++)
		{
			var x = Mathf.Sin(j);
			var y = Mathf.Cos(j);

			j += angle * Mathf.Deg2Rad / rays;

			Vector3 dir = transform.TransformDirection(new Vector3(x, 0, y));
			if (GetRaycast(dir)) a = true;

			if (x != 0)
			{
				dir = transform.TransformDirection(new Vector3(-x, 0, y));
				if (GetRaycast(dir)) b = true;
			}
		}

		if (a || b) result = true;
		return result;
	}

	void Update()
	{
		if (Vector3.Distance(transform.position, target.position) < distance)
		{
			if (RayToScan())
			{
				// Контакт с целью
			}
			else
			{
				// Поиск цели...
			}
		}
	}
}