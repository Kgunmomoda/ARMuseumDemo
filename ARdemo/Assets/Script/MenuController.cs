using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    void Start()
    {
        //SetBuildUI();
    }
    /// <summary>
    /// ���뽨����ͼ
    /// </summary>
    public void BuildMap()
    {
        SceneManager.LoadScene("BuildMap");
    }

    public void KeyPoint()
    {
        SceneManager.LoadScene("KeyPoint");
    }

    public void RoadScene()
    {
        SceneManager.LoadScene("Road");
    }

    public void Navigation()
    {
        SceneManager.LoadScene("TestNav");
    }

    /// <summary>
    /// ɾ��
    /// </summary>
    public void DeleteMap()
    {
        PlayerPrefs.DeleteKey("MapID");
        PlayerPrefs.DeleteKey("MapName");
        SetBuildUI();
    }
    /// <summary>
    /// ���ý�����ͼ���UI
    /// </summary>
    private void SetBuildUI()
    {

    }
    /// <summary>
    /// �˳�Ӧ��
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
