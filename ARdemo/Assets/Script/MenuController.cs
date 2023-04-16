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
    /// 进入建立地图
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
    /// 删除
    /// </summary>
    public void DeleteMap()
    {
        PlayerPrefs.DeleteKey("MapID");
        PlayerPrefs.DeleteKey("MapName");
        SetBuildUI();
    }
    /// <summary>
    /// 设置建立地图相关UI
    /// </summary>
    private void SetBuildUI()
    {

    }
    /// <summary>
    /// 退出应用
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
