//================================================================================================================================
//
//  Copyright (c) 2020-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using EasyAR.Mega.Ema;
using System;
using UnityEngine;

namespace Sample
{
    [EasyAR.Mega.Ema.v0_5.AnnotationExtension(typeof(Data), "SampleCompany", "SampleExtension", 1, 0, 0)]
    public class SampleAnnotationExtension : MonoBehaviour, IAnnotationExtension
    {
        public Data ExtensionData;

        public object Export() => ExtensionData;

        public void Import(object obj) => ExtensionData = obj as Data;

        [Serializable] // NOTE: Serializable is used in Unity filed serialization, it is not required in AnnotationExtension.
        public class Data
        {
            public string SampleString;
            public int SampleInt;
            public float SampleFloat;
        }
    }
}
