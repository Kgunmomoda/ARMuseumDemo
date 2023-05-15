//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#if EASYAR_ENABLE_SENSE
using easyar;
using EasyAR.Mega.Scene;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NRKernal;

namespace Sample
{
    public class Sample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;

        private MegaTrackerFrameFilter megaTracker;
        private MegaTrackerFrameFilter.LocalizationResponse debugInfo;

        private string deviceModel = string.Empty;
        private static Optional<DateTime> trialCounter;
        private bool inputBlocked;

        private void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    using (var buildClass = new AndroidJavaClass("android.os.Build"))
                    {
                        deviceModel = $"(Device = {buildClass.GetStatic<string>("DEVICE")}, Model = {buildClass.GetStatic<string>("MODEL")})";
                    }
                }
                catch (Exception e) { deviceModel = e.Message; }
            }
#endif
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
            var blocks = FindObjectOfType<BlockRootController>();
            Debug.Log("use Mega blocks: " + blocks);

            megaTracker = Session.GetComponentInChildren<MegaTrackerFrameFilter>(true);
            megaTracker.LocalizationUpdate += (response) => { debugInfo = response; };
        }

        private void Update()
        {
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                "Nreal Session Status: " + NRFrame.SessionStatus + Environment.NewLine +
                "Frame Source: " + ((Session.Assembly != null && Session.Assembly.FrameSource) ? Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                "Tracking Status (Async): " + Session.TrackingStatus + Environment.NewLine +
                "Mega Tracker:" + Environment.NewLine +
                $"\tRequest: Timeout ({megaTracker.RequestTimeParameters.Timeout}), RequestInterval ({megaTracker.RequestTimeParameters.RequestInterval})" + Environment.NewLine +
                $"\tResultPose: Localization ({megaTracker.ResultPoseType.EnableLocalization}), Stabilization ({megaTracker.ResultPoseType.EnableStabilization})" + Environment.NewLine;

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
    }
}
#endif
