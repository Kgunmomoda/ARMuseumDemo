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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class Sample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public GameObject MsgBox;

        private MegaTrackerFrameFilter megaTracker;
        private MegaTrackerFrameFilter.LocalizationResponse debugInfo;
        private string deviceModel = string.Empty;
        private static Optional<DateTime> trialCounter;

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
            foreach (var f in Session.GetComponentsInChildren<FrameSource>().Where(f => f is CameraDeviceFrameSource))
            {
                f.gameObject.SetActive(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer);
            }
            megaTracker = Session.GetComponentInChildren<MegaTrackerFrameFilter>();
            megaTracker.LocalizationUpdate += (response) =>
            {
                debugInfo = response;
                HandleLocalizationStatusChange(response.Status);
            };
            MsgBox.SetActive(false);

            if (!FindObjectOfType<BlockRootController>())
            {
                throw new UIPopupException("Mega blocks not found, please use Mega Studio to import blocks first.");
            }

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

        private void Update()
        {
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                "Frame Source: " + ((Session.Assembly != null && Session.Assembly.FrameSource) ? Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                "Tracking Status: " + Session.TrackingStatus + Environment.NewLine +
                "Mega Tracker Parameters:" + Environment.NewLine +
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

        private void HandleLocalizationStatusChange(MegaTrackerLocalizationStatus status)
        {
            if (status == MegaTrackerLocalizationStatus.QpsLimitExceeded)
            {
                if (!MsgBox.activeSelf)
                {
                    MsgBox.SetActive(true);
                }
                var text = MsgBox.GetComponentInChildren<Text>();
                text.text = "Message for developer:" + Environment.NewLine +
                "QPS limit exceeded, you can keep random user fail (overall worse tracking quality) or pay for more QPS." + Environment.NewLine +
                "QPS超限，你可以保持随机用户失败（总体跟踪质量下降）或付费提升QPS上限。" + Environment.NewLine +
                Environment.NewLine +
                "Message for user:" + Environment.NewLine +
                "Too many users, please wait patiently." + Environment.NewLine +
                "用户过多，请耐心等待。";
            }
            else if (status == MegaTrackerLocalizationStatus.NotFound)
            {
                if (!MsgBox.activeSelf)
                {
                    MsgBox.SetActive(true);
                }
                var text = MsgBox.GetComponentInChildren<Text>();
                text.text = "Message for developer:" + Environment.NewLine +
                "Service is waking up, you need to let user wait." + Environment.NewLine +
                "服务正在唤醒中，你需要让用户等待。" + Environment.NewLine +
                Environment.NewLine +
                "Message for user:" + Environment.NewLine +
                "Service is waking up, please wait patiently." + Environment.NewLine +
                "服务正在唤醒中，请耐心等待。";
            }
            else
            {
                if (MsgBox.activeSelf)
                {
                    MsgBox.SetActive(false);
                }
            }
        }

        public void TrackSwitch(bool on)
        {
            megaTracker.enabled = on;
        }
    }
}
#endif
