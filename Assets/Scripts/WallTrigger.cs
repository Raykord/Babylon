using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			// Получите вектор от центра стены к центру персонажа
			Vector3 repelDirection = other.transform.position - transform.position;

			// Примените силу отталкивания к персонажу
			Rigidbody playerRb = other.GetComponent<Rigidbody>();
			playerRb.AddForce(repelDirection.normalized, ForceMode.Impulse);
		}
	}
}
