using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    /// <summary>
    /// �ؼ���
    /// </summary>
    public KeyPoint keyPoint;
    /// <summary>
    /// ·��
    /// </summary>
    public Road road;
    /// <summary>
    /// Ŀ�ĵ�
    /// </summary>
    public Transform arrival;

    void Start()
    {
        //gameObject.GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    GameObject.Find("SceneMaster").SendMessage("SelectButtonClicked", transform);
        //});
    }
}
