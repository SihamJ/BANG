using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class HostConnect : MonoBehaviour
{
    NetworkManager manager;
    public InputField ip_InputField;
    public GameObject HostConnect_go;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();

    }

    public void Hostfunction()
    {
        manager.StartHost();

        HostConnect_go.SetActive(false);
    }


    public void ConnectFunction()
    {
        manager.networkAddress = ip_InputField.text;
        manager.StartClient();
        Debug.Log(ip_InputField.text);

        HostConnect_go.SetActive(false);

    }


}
