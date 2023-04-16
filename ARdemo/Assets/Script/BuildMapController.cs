using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildMapController : MonoBehaviour
{
    /// <summary>
    /// 游戏控制器
    /// </summary>
    public GameController gameController;
    /// <summary>
    /// 保存按钮
    /// </summary>
    public Button btnSave;
    public ARSession session;
    public SparseSpatialMapWorkerFrameFilter mapWorker;
    public SparseSpatialMapController map;
    public WorldRootController worldRoot;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        btnSave.onClick.AddListener(Save);
        btnSave.interactable = false;
        //追踪状态设置
        worldRoot.TrackingStatusChanged += OnTrackingStatusChanged;
        if (worldRoot.TrackingStatus == MotionTrackingStatus.Tracking)
        {
            btnSave.interactable = true;
        }
        else
        {
            btnSave.interactable = false;
        }
    }

    /// <summary>
    /// 保存地图
    /// </summary>
    private void Save()
    {
        btnSave.interactable = false;
        //地图保存结果反馈
        mapWorker.BuilderMapController.MapHost += (mapInfo, isSuccess, error) =>
        {
            if (isSuccess)
            {
                PlayerPrefs.SetString("MapID", mapInfo.ID);
                PlayerPrefs.SetString("MapName", mapInfo.Name);
                gameController.SendMessage("ShowMessage", "地图保存成功。");
            }
            else
            {
                gameController.SendMessage("ShowMessage", "地图保存出错：" + error);
                btnSave.interactable = true;
            }
        };
        try
        {
            //保存地图
            mapWorker.BuilderMapController.Host(gameController.inputName, null);
            gameController.SendMessage("ShowMessage", "开始保存地图，请稍等。");
        }
        catch (Exception ex)
        {
            gameController.SendMessage("ShowMessage", "保存出错：" + ex.Message);
            btnSave.interactable = true;
        }
    }
    /// <summary>
    /// 摄像机状态变化
    /// </summary>
    /// <param name="status">状态</param>
    private void OnTrackingStatusChanged(MotionTrackingStatus status)
    {
        if (status == MotionTrackingStatus.Tracking)
        {
            btnSave.interactable = true;
            gameController.SendMessage("ShowMessage", "进入追踪状态。");
        }
        else
        {
            btnSave.interactable = false;
            gameController.SendMessage("ShowMessage", "追踪状态异常。");
        }
    }

    public void back()
    {
        SceneManager.LoadScene("demo");
    }
}

