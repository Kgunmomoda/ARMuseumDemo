using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicZoneBlue : MonoBehaviour
{
    public GameObject MagicZone;

    // Start is called before the first frame update
    void Awake()
    {
        //MagicZone.SetActive(true);
    }

    // Update is called once per frame
    public void Zonebegin()
    {
        MagicZone.SetActive(true);
    }
    public void Zonestop()
    {
        MagicZone.SetActive(false);
    }
    public void OnTriggerEnter(Collider other)//�Ӵ�ʱ�������������
    {
        MagicZone.SetActive(true);

    }
    public void OnTriggerStay(Collider other)    //ÿ֡����һ��OnTriggerStay()����
    {

    }
    public void OnTriggerExit(Collider other)
    {
        MagicZone.SetActive(false);
    }

}
