using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPlatformsController : MonoBehaviour
{
	private Color defPlatformColor;



	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "LightingPlatform")
		{
			var cubeRenderer = collision.gameObject.GetComponent<MeshRenderer>();
			defPlatformColor = cubeRenderer.material.GetColor("_EmissionColor");
			cubeRenderer.material.SetColor("_EmissionColor", defPlatformColor * 2);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.tag == "LightingPlatform")
		{
			var cubeRenderer = collision.gameObject.GetComponent<MeshRenderer>();

			cubeRenderer.material.SetColor("_EmissionColor", defPlatformColor);
		}
	}
}
