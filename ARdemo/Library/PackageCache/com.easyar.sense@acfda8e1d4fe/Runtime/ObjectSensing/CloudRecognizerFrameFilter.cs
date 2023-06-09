﻿//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en"><see cref="MonoBehaviour"/> which controls <see cref="CloudRecognizer"/> in the scene, providing a few extensions in the Unity environment. There is no need to use <see cref="CloudRecognizer"/> directly.</para>
    /// <para xml:lang="zh">在场景中控制<see cref="CloudRecognizer"/>的<see cref="MonoBehaviour"/>，在Unity环境下提供功能扩展。不需要直接使用<see cref="CloudRecognizer"/>。</para>
    /// </summary>
    public class CloudRecognizerFrameFilter : FrameFilter
    {
        /// <summary>
        /// <para xml:lang="en">Timeout in milliseconds when communicating with server.</para>
        /// <para xml:lang="zh">与服务器通信的超时时间（毫秒）。</para>
        /// </summary>
        public int ResolveTimeout = 3000;

        /// <summary>
        /// <para xml:lang="en">Use global service config or not. The global service config can be changed on the inspector after click Unity menu EasyAR -> Sense -> Configuration.</para>
        /// <para xml:lang="zh">是否使用全局服务器配置。全局配置可以点击Unity菜单EasyAR -> Sense -> Configuration后在属性面板里面进行填写。</para>
        /// </summary>
        public bool UseGlobalServiceConfig = true;

        /// <summary>
        /// <para xml:lang="en">Cloud recognizer key type.</para>
        /// <para xml:lang="zh">云识别服务密钥类型。</para>
        /// </summary>
        [HideInInspector]
        public KeyType ServerKeyType = KeyType.Public;

        /// <summary>
        /// <para xml:lang="en">Service config when <see cref="UseGlobalServiceConfig"/> == false, only valid for this object.</para>
        /// <para xml:lang="zh"><see cref="UseGlobalServiceConfig"/> == false时使用的服务器配置，只对该物体有效。</para>
        /// </summary>
        [HideInInspector, SerializeField]
        public CloudRecognizerServiceConfig ServiceConfig = new CloudRecognizerServiceConfig();

        /// <summary>
        /// <para xml:lang="en">Service config when <see cref="UseGlobalServiceConfig"/> == false, only valid for this object.</para>
        /// <para xml:lang="zh"><see cref="UseGlobalServiceConfig"/> == false时使用的服务器配置，只对该物体有效。</para>
        /// </summary>
        [HideInInspector, SerializeField]
        public PrivateCloudRecognizerServiceConfig PrivateServiceConfig = new PrivateCloudRecognizerServiceConfig();

        /// <senseapi/>
        private CloudRecognizer cloudRecognizer;
        private Queue<Request> pendingRequets = new Queue<Request>();
        private bool failToCreate;

        /// <summary>
        /// <para xml:lang="en">Cloud recognizer key type.</para>
        /// <para xml:lang="zh">云识别服务密钥类型。</para>
        /// </summary>
        public enum KeyType
        {
            Public,
            Private
        }

        public override int BufferRequirement
        {
            get { return 0; }
        }

        protected virtual void OnDestroy()
        {
            cloudRecognizer?.Dispose();
        }

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
            session.FrameUpdate += OnFrameUpdate;
            StartCoroutine(AutoCreate());
        }

        /// <summary>
        /// <para xml:lang="en">Send recognition request. The lowest available request interval is 300ms</para>
        /// <para xml:lang="zh">发送云识别请求。最低可用请求间隔是300ms。</para>
        /// </summary>
        public void Resolve(Action<InputFrame> start, Action<Optional<CloudRecognizationResult>, string> finish)
        {
            if (cloudRecognizer == null)
            {
                if (failToCreate)
                {
                    finish?.Invoke(null, "FailToCreate");
                }
                else
                {
                    finish?.Invoke(null, "Unavailable");
                }
                return;
            }
            if (!enabled)
            {
                finish?.Invoke(null, "Disabled");
                return;
            }
            var request = new Request
            {
                StartCallback = start,
                FinishCallback = finish,
            };
            pendingRequets.Enqueue(request);
            StartCoroutine(CheckRequest(request));
        }


        private void OnFrameUpdate(OutputFrame outputFrame)
        {
            if (cloudRecognizer == null)
            {
                return;
            }
            while (pendingRequets.Count > 0)
            {
                using (var iFrame = outputFrame.inputFrame())
                {
                    var request = pendingRequets.Dequeue();
                    if (request.StartCallback != null)
                    {
                        request.StartCallback(iFrame);
                    }
                    {
                        cloudRecognizer.resolve(iFrame, ResolveTimeout, EasyARController.Scheduler, (result) => { request.FinishCallback(result, string.Empty); });
                    }
                }
            }
        }

        private IEnumerator AutoCreate()
        {
            while (!enabled) { yield return null; }
            if (!CloudRecognizer.isAvailable()) { throw new UIPopupException(typeof(CloudRecognizer) + " not available"); }

            if (UseGlobalServiceConfig)
            {
                var config = new CloudRecognizerServiceConfig();
                if (EasyARSettings.Instance)
                {
                    config = EasyARSettings.Instance.GlobalCloudRecognizerServiceConfig;
                }
                NotifyEmptyConfig(config);
                try
                {
                    cloudRecognizer = CloudRecognizer.create(config.ServerAddress.Trim(), config.APIKey.Trim(), config.APISecret.Trim(), config.CloudRecognizerAppID.Trim());
                    failToCreate = false;
                }
                catch (ArgumentNullException)
                {
                    failToCreate = true;
                    throw new UIPopupException($"Fail to create {nameof(CloudRecognizer)}, check logs for detials.");
                }
            }
            else
            {
                if (ServerKeyType == KeyType.Public)
                {
                    NotifyEmptyConfig(ServiceConfig);
                    try
                    {
                        cloudRecognizer = CloudRecognizer.create(ServiceConfig.ServerAddress, ServiceConfig.APIKey, ServiceConfig.APISecret, ServiceConfig.CloudRecognizerAppID);
                        failToCreate = false;
                    }
                    catch (ArgumentNullException)
                    {
                        failToCreate = true;
                        throw new UIPopupException($"Fail to create {nameof(CloudRecognizer)}, check logs for detials.");
                    }

                }
                else if (ServerKeyType == KeyType.Private)
                {
                    NotifyEmptyPrivateConfig(PrivateServiceConfig);
                    try
                    {
                        cloudRecognizer = CloudRecognizer.createByCloudSecret(PrivateServiceConfig.ServerAddress, PrivateServiceConfig.CloudRecognitionServiceSecret, PrivateServiceConfig.CloudRecognizerAppID);
                        failToCreate = false;
                    }
                    catch (ArgumentNullException)
                    {
                        failToCreate = true;
                        throw new UIPopupException($"Fail to create {nameof(CloudRecognizer)}, check logs for detials.");
                    }
                }
            }
        }

        private IEnumerator CheckRequest(Request req)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            while (pendingRequets.Count > 0 && pendingRequets.Contains(req))
            {
                var request = pendingRequets.Dequeue();
                request.FinishCallback?.Invoke(null, "NoFrame");
            }
        }

        private void NotifyEmptyConfig(CloudRecognizerServiceConfig config)
        {
            if (string.IsNullOrEmpty(config.ServerAddress) ||
                string.IsNullOrEmpty(config.APIKey) ||
                string.IsNullOrEmpty(config.APISecret) ||
                string.IsNullOrEmpty(config.CloudRecognizerAppID))
            {
                throw new UIPopupException(
                    "Service config (for authentication) NOT set, please set" + Environment.NewLine +
                    "globally in EasyAR Settings (Project Settings > EasyAR) or" + Environment.NewLine +
                    "locally on <CloudRecognizerFrameFilter> Component." + Environment.NewLine +
                    "Get from EasyAR Develop Center (www.easyar.com) -> CRS -> Database Details.");
            }
        }

        private void NotifyEmptyPrivateConfig(PrivateCloudRecognizerServiceConfig config)
        {
            if (string.IsNullOrEmpty(config.ServerAddress) ||
                string.IsNullOrEmpty(config.CloudRecognitionServiceSecret) ||
                string.IsNullOrEmpty(config.CloudRecognizerAppID))
            {
                throw new UIPopupException(
                    "Service config (for authentication) NOT set, please set" + Environment.NewLine +
                    "globally in EasyAR Settings (Project Settings > EasyAR) or" + Environment.NewLine +
                    "locally on <CloudRecognizerFrameFilter> Component." + Environment.NewLine +
                    "Get from EasyAR Develop Center (www.easyar.com) -> CRS -> Database Details.");
            }
        }

        /// <summary>
        /// <para xml:lang="en">Service config for <see cref="easyar.CloudRecognizer"/>.</para>
        /// <para xml:lang="zh"><see cref="easyar.CloudRecognizer"/>服务器配置。</para>
        /// </summary>
        [Serializable]
        public class CloudRecognizerServiceConfig
        {
            /// <summary>
            /// <para xml:lang="en">Server Address, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">服务器地址，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string ServerAddress = string.Empty;
            /// <summary>
            /// <para xml:lang="en">API Key, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">API Key，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string APIKey = string.Empty;
            /// <summary>
            /// <para xml:lang="en">API Secret, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">API Secret，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string APISecret = string.Empty;
            /// <summary>
            /// <para xml:lang="en">Cloud Recognizer AppID, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">云识别AppID，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string CloudRecognizerAppID = string.Empty;
        }

        [Serializable]
        public class PrivateCloudRecognizerServiceConfig
        {
            public string ServerAddress = string.Empty;
            public string CloudRecognitionServiceSecret = string.Empty;
            public string CloudRecognizerAppID = string.Empty;
        }

        private class Request
        {
            public Action<InputFrame> StartCallback;
            public Action<Optional<CloudRecognizationResult>, string> FinishCallback;
        }
    }
}
