using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaler : MonoBehaviour
{

	[SerializeField] private Slider slider;
	
	// Use this for initialization
	void Start ()
	{
		slider = slider ?? GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (slider != null)
			Time.timeScale = slider.value;
	}
}
