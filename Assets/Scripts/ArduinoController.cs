using UnityEngine;
using System.Collections;
using System.IO.Ports;


public class ArduinoController : MonoBehaviour {
	SerialPort stream = new SerialPort("COM5", 9600); //Set the port (com4) and the baud rate (9600, is standard on most devices)
	int lectura;
	// Use this for initialization
	void Start () {
		stream.Open(); //Open the Serial Stream.
		stream.ReadTimeout = 1;

	}
	
	// Update is called once per frame
	void Update () {

		if (stream.IsOpen) {

			try{

				lectura = stream.ReadByte();

				if(lectura == 1){
					
					this.gameObject.GetComponent<SpriteRenderer> ().color = new UnityEngine.Color(0,0,1);
		
				} if(lectura==2){
					
					this.gameObject.GetComponent<SpriteRenderer> ().color = new UnityEngine.Color(0,1,1);

				}

			}catch (System.Exception){
				
			}

		}
			
	}
}
