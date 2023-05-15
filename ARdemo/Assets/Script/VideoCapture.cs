using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoCapture : MonoBehaviour
{
    public Button RecordBtn;
    public VideoRecorder videoRecorder;
    private bool IsRecording;
    private string filePath;
    /// <summary>
    /// µ÷ÊÔÐÅÏ¢
    /// </summary>
    public Text Status;

    // Start is called before the first frame update
    void Awake()
    {    
        videoRecorder.FilePathType = WritablePathType.PersistentDataPath;
        videoRecorder.StatusUpdate += (status, msg) =>
        {
            if (status == RecordStatus.OnStarted)
            {
                Status.text += "Recording status ok";
            }
            if (status == RecordStatus.FailedToStart || status == RecordStatus.FileFailed || status == RecordStatus.LogError)
            {
                Status.text += "Recording Error: " + status + ", details: " + msg;
            }
        };
        RefreshUIState();
    }

    private void Start()
    {
        IsRecording = false;
    }

    public void OnClickPlayButton()
    {
        if (IsRecording)
        {
            this.StopVideoCapture();
        }
        else
        {
            this.StartVideoCapture();
        }
    }

    public void StartVideoCapture()
    {
        if (IsRecording)
        {
            Status.text += Environment.NewLine + "IsRecording" + IsRecording;
            return;
        }
        if (!videoRecorder.IsReady)
        {
            Status.text += Environment.NewLine + "Recording is not ready";
            return;
        }
        filePath = "EasyAR_Recording_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mp4";
        videoRecorder.FilePath = filePath;
        videoRecorder.StartRecording();
        IsRecording = true;
        Status.text += Environment.NewLine + "Recording start success";
        RefreshUIState();
    }


    /// <summary> Stops video capture. </summary>
    public void StopVideoCapture()
    {
        if (!IsRecording)
        {
            return;
        }
        if (videoRecorder.StopRecording())
        {
            IsRecording = false;
            RefreshUIState();
            Status.text = "Recording stop success"+ Environment.NewLine +
                    "Filename: " + filePath + Environment.NewLine +
                    "PersistentDataPath: " + Application.persistentDataPath;
        }else
        {
            Status.text = "Recording failed";
        }
    }

    void RefreshUIState()
    {
        RecordBtn.GetComponent<UnityEngine.UI.Image>().color = IsRecording ? Color.green : Color.red;
    }
}
