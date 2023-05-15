using easyar;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Navigation : MonoBehaviour
{
    public ARSession Session;
    /// <summary>
    /// ������
    /// </summary>
    public LineRenderer lineRenderer;
    /// <summary>
    /// ��������
    /// </summary>
    public NavMeshAgent agent;
    /// <summary>
    /// ����·��
    /// </summary>
    private NavMeshPath path;
    /// <summary>
    /// ���
    /// </summary>
    public Transform player;
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public Text Status;
    /// <summary>
    /// ����Ŀ�ĵ�
    /// </summary>
    public Transform arrival;
    private string deviceModel = string.Empty;
    private MegaTrackerFrameFilter.LocalizationResponse debugInfo;
    private static Optional<DateTime> trialCounter;
    private MegaTrackerFrameFilter megaTracker;
    private bool inputBlocked;

    private void Awake()
    {
        megaTracker = Session.GetComponentInChildren<MegaTrackerFrameFilter>(true);
        megaTracker.LocalizationUpdate += (response) =>
        {
            debugInfo = response;
           // HandleLocalizationStatusChange(response.Status);
        };

        Session.StateChanged += (state) =>
        {
            if (state == ARSession.SessionState.Ready)
            {
                if (trialCounter.OnNone)
                {
                    trialCounter = DateTime.Now;
                }
            }
        };
    }

    void Start()
    {
        agent.transform.position = player.transform.position;
        path = new NavMeshPath();
        //��ʼ������·��
        //HideElement();
        InvokeRepeating("DisplayPath", 0, 0.5f);
        //arrival.gameObject.SetActive(true); //��ʾĿ���
        //Transform go = Instantiate(prefab, Blocks);
        //go.position = arrival.position;
    }

    private void Update()
    {
        Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
       "Frame Source: " + ((Session.Assembly != null && Session.Assembly.FrameSource) ? Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
       "Tracking Status: " + Session.TrackingStatus + Environment.NewLine +
       "Mega Tracker Parameters:" + Environment.NewLine +
       $"\tRequest: Timeout ({megaTracker.RequestTimeParameters.Timeout}), RequestInterval ({megaTracker.RequestTimeParameters.RequestInterval})" + Environment.NewLine +
       $"\tResultPose: Localization ({megaTracker.ResultPoseType.EnableLocalization}), Stabilization ({megaTracker.ResultPoseType.EnableStabilization})" + Environment.NewLine +
       $"\tAI����λ��:" + agent.transform.position + "Ŀ�ĵ�λ��:" + arrival.transform.position + "���λ��:" + player.transform.position;

        if (debugInfo != null)
        {
            Status.text += "Localization Debug Info: " + Environment.NewLine +
                $"\tTimestamp: {debugInfo.Timestamp:F3}" + Environment.NewLine +
                "\tStatus: " + debugInfo.Status + Environment.NewLine +
                "\tServer Response Duration (s): " + debugInfo.ServerResponseDuration + Environment.NewLine +
                "\tServer Calculation Duration (s): " + debugInfo.ServerCalculationDuration + Environment.NewLine;
            foreach (var block in debugInfo.Blocks)
            {
                Status.text += $"\tBlock: {block.Info.Name} ({block.Info.ID})" + Environment.NewLine;
            }
            if (debugInfo.ErrorMessage.OnSome)
            {
                Status.text += "\tError Message: " + debugInfo.ErrorMessage + Environment.NewLine;
            }
        }

        if (Session.State > ARSession.SessionState.Ready)
        {
            if (Session.Assembly.FrameSource is NrealFrameSource && trialCounter != DateTime.MinValue)
            {
                var nrealFrameSource = (NrealFrameSource)Session.Assembly.FrameSource;
                Status.text += Environment.NewLine +
                    Environment.NewLine +
                    $"EasyAR received frame count from Nreal: {nrealFrameSource.ReceivedFrameCount / 100 * 100}+";

                StartCoroutine(CheckData(nrealFrameSource.ReceivedFrameCount, (c) => { inputBlocked = nrealFrameSource.ReceivedFrameCount == c; }));
                if (inputBlocked)
                {
                    Status.text += Environment.NewLine + "!! WARNING: RGB Camera input has been blocked for 1+ seconds, please check your device !!";
                }
            }
        }

        // avoid misunderstanding when using personal edition, not necessary in your own projects
        if (!string.IsNullOrEmpty(Engine.errorMessage()))
        {
            trialCounter = DateTime.MinValue;
        }
        if (trialCounter.OnSome)
        {
            if (Session.State >= ARSession.SessionState.Ready && (FrameSource.IsCustomCamera(Session.Assembly.FrameSource) || trialCounter.Value == DateTime.MinValue))
            {
                var time = Math.Max(0, (int)(trialCounter.Value - DateTime.Now).TotalSeconds + 100);
                Status.text += $"\n\nEasyAR License for {Session.Assembly.FrameSource.GetType()} will timeout for current process within {time} seconds. (Personal Edition Only)";
            }
        }
    }

    private IEnumerator CheckData(int count, Action<int> callback)
    {
        yield return new WaitForSeconds(1);
        callback(count);
    }

    /// <summary>
    /// ��ʾ·��
    /// </summary>
    private void DisplayPath()
    {
        agent.transform.position = player.transform.position;
        //agent.enabled = true;
        agent.CalculatePath(arrival.position, path);
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);
        //agent.enabled = false;
    }

    public void EndGame()
    {
        agent.enabled = false;
        CancelInvoke("DisplayPath");
    }

    public void switchArrival(Transform arrival)
    {
        this.arrival = arrival;
        Debug.Log("Ŀ�ĵ�λ�øı�:" + arrival.transform.position);
    }
}