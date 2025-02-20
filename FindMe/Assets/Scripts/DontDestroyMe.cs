using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AC;

public class DontDestroyMe : MonoBehaviour

{


	private void Start()

	{
		DontDestroyOnLoad(gameObject);
	}


}