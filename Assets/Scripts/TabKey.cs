using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class TabKey : MonoBehaviour
{
    EventSystem system;
    public Selectable premierInput;
    
    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
        premierInput.Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if(next != null)
            {
                next.Select();
            }
        }
    }
}
