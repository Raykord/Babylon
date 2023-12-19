using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayTrigger : MonoBehaviour
{
    public Transform[] platforms;

    // Start is called before the first frame update
    void Awake()
    {
        platforms = GetComponentsInChildren<Transform>().Skip(1).ToArray();
		foreach (Transform platform in platforms)
		{
			platform.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		StartCoroutine(OnPlatforms());
	}

	IEnumerator OnPlatforms()
	{
		foreach (Transform platform in platforms)
		{
			platform.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
		
	}
}
