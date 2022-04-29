using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour 
{
	public PageName pageName;

	public void isVisible(bool state)
	{
		gameObject.SetActive(state);
	}
}
