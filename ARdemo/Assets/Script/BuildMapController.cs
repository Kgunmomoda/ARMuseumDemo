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
    /// ��Ϸ������
    /// </summary>
    public GameController gameController;
    /// <summary>
    /// ���水ť
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
        //׷��״̬����
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
    /// �����ͼ
    /// </summary>
    private void Save()
    {
        btnSave.interactable = false;
        //��ͼ����������
        mapWorker.BuilderMapController.MapHost += (mapInfo, isSuccess, error) =>
        {
            if (isSuccess)
            {
                PlayerPrefs.SetString("MapID", mapInfo.ID);
                PlayerPrefs.SetString("MapName", mapInfo.Name);
                gameController.SendMessage("ShowMessage", "��ͼ����ɹ���");
            }
            else
            {
                gameController.SendMessage("ShowMessage", "��ͼ�������" + error);
                btnSave.interactable = true;
            }
        };
        try
        {
            //�����ͼ
            mapWorker.BuilderMapController.Host(gameController.inputName, null);
            gameController.SendMessage("ShowMessage", "��ʼ�����ͼ�����Եȡ�");
        }
        catch (Exception ex)
        {
            gameController.SendMessage("ShowMessage", "�������" + ex.Message);
            btnSave.interactable = true;
        }
    }
    /// <summary>
    /// �����״̬�仯
    /// </summary>
    /// <param name="status">״̬</param>
    private void OnTrackingStatusChanged(MotionTrackingStatus status)
    {
        if (status == MotionTrackingStatus.Tracking)
        {
            btnSave.interactable = true;
            gameController.SendMessage("ShowMessage", "����׷��״̬��");
        }
        else
        {
            btnSave.interactable = false;
            gameController.SendMessage("ShowMessage", "׷��״̬�쳣��");
        }
    }

    public void back()
    {
        SceneManager.LoadScene("demo");
    }
}

