using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class localiazation : MonoBehaviour
{
    // Start is called before the first frame update
    public SparseSpatialMapWorkerFrameFilter mapWorker;
    void Start()
    {
        mapWorker.Localizer.startLocalization();    //���ػ���ͼ
    }

    // Update is called once per frame
    
}
