using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trigger : MonoBehaviour
{
    public Transform nextTarget;
    public Navigation navigation;
    public Text status;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        status.text += "Ω¯»ÎTrigger";
    }

    void OnTriggerExit(Collider other)
    {
        status.text += "target=" + nextTarget;
        navigation.arrival = nextTarget;
        navigation.arrival.gameObject.SetActive(true);
    }

}
