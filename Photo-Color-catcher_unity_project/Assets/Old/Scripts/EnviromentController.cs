using UnityEngine;
using System.Collections;
using System.Globalization;
using System;

public class EnviromentController : MonoBehaviour {

	[SerializeField]
	private GameObject snow1;

	[SerializeField]
	private GameObject snow2;

	[SerializeField]
	private GameObject snow3;

	[SerializeField]
	private GameObject water1;

	[SerializeField]
	private GameObject water2;

	[SerializeField]
	private GameObject water3;

	[SerializeField]
	private GameObject water4;

	[SerializeField]
	private GameObject leaf1;

	[SerializeField]
	private GameObject leaf2;

	[SerializeField]
	private GameObject leaf3;

	[SerializeField]
	private GameObject day;

	[SerializeField]
	private GameObject night;

	[SerializeField]
	private GameObject morning;

	[SerializeField]
	private GameObject afternoon;

	private int month;

	private int hour;

	// Use this for initialization
	void Start () {
		month = System.DateTime.Now.Month;
		hour = System.DateTime.Now.Hour;
	}
	
	// Update is called once per frame
	void Update () {
		DayControl ();
		HourControl ();
	}

	private void DayControl(){
		if (month == 1 || month == 2 || month == 12) {

			snow1.SetActive (true);
			snow2.SetActive (true);
			snow3.SetActive (true);

			water1.SetActive (false);
			water2.SetActive (false);
			water3.SetActive (false);
			water4.SetActive (false);

			leaf1.SetActive (false);
			leaf2.SetActive (false);
			leaf3.SetActive (false);

		}

		if (month == 3 || month == 4 || month == 5) {

			water1.SetActive (true);
			water2.SetActive (true);
			water3.SetActive (true);
			water4.SetActive (true);

			leaf1.SetActive (false);
			leaf2.SetActive (false);
			leaf3.SetActive (false);

			snow1.SetActive (false);
			snow2.SetActive (false);
			snow3.SetActive (false);
		}

		if (month == 6 || month == 7 || month == 8) {
			water1.SetActive (false);
			water2.SetActive (false);
			water3.SetActive (false);
			water4.SetActive (false);		

			leaf1.SetActive (false);
			leaf2.SetActive (false);
			leaf3.SetActive (false);

			snow1.SetActive (false);
			snow2.SetActive (false);
			snow3.SetActive (false);
		}

		if (month == 9 || month == 10 || month == 11) {

			leaf1.SetActive (true);
			leaf2.SetActive (true);
			leaf3.SetActive (true);

			water1.SetActive (false);
			water2.SetActive (false);
			water3.SetActive (false);
			water4.SetActive (false);

			snow1.SetActive (false);
			snow2.SetActive (false);
			snow3.SetActive (false);
		}
	}

	private void HourControl(){
		if (hour >= 0 && hour < 7 || hour >= 21) {
			night.SetActive (true);
			morning.SetActive (false);
			day.SetActive (false);
			afternoon.SetActive (false);
		}
		if (hour >= 7 && hour < 10) {
			morning.SetActive (true);
			night.SetActive (false);
			day.SetActive (false);
			afternoon.SetActive (false);
		}
		if (hour >= 10 && hour < 19) {
			day.SetActive (true);
			morning.SetActive (false);
			night.SetActive (false);
			afternoon.SetActive (false);
		}
		if (hour >= 19 && hour < 21) {
			afternoon.SetActive (true);
			morning.SetActive (false);
			day.SetActive (false);
			night.SetActive (false);
		}
	}
}
