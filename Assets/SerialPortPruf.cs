using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialPortPruf : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM9", 9600);

    void Start()
    {
        OpenConnection();
    }


    void OpenConnection()
    {
        stream.ReadTimeout = 100;
        stream.Open();
    }

    void Update()
    {
        string value = null;
        try { value = stream.ReadLine(); } catch { /*print("A problem was detected");*/ }
        

        if(value != null)
        {
            print("Distance measured: "+value+" cm.");
        }
    }
}
