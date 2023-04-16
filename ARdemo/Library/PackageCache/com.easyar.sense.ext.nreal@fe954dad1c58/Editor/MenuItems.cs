//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace easyar
{
    class MenuItems
    {
        const int priority = 31;
        const string menuPath = "GameObject/EasyAR Sense/Ext: Nreal/";
        const string menuPathMega = "GameObject/EasyAR Mega/Sense/";

        [MenuItem(menuPath + "AR Session (Mega Preset)", priority = priority)]
#if EASYAR_ENABLE_MEGA
        [MenuItem(menuPathMega + "AR Session (Mega Preset) : Nreal", priority = priority)]
#endif
        static void ARSessionPresetMega() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.Mega, () => CreateFrameSources(ARSessionFactory.ARSessionPreset.Mega));

        [MenuItem(menuPath + "AR Session (Sparse SpatialMap Preset)", priority = priority)]
        static void ARSessionPresetSparseSpatialMap() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.SparseSpatialMap, () => CreateFrameSources(ARSessionFactory.ARSessionPreset.SparseSpatialMap));

        [MenuItem(menuPath + "AR Session (Dense SpatialMap Preset)", priority = priority)]
        static void ARSessionPresetDenseSpatialMap() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.DenseSpatialMap, () => CreateFrameSources(ARSessionFactory.ARSessionPreset.DenseSpatialMap));

        [MenuItem(menuPath + "AR Session (Sparse and Dense SpatialMap Preset)", priority = priority)]
        static void ARSessionPresetSparseAndDenseSpatialMap() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.SparseAndDenseSpatialMap, () => CreateFrameSources(ARSessionFactory.ARSessionPreset.SparseAndDenseSpatialMap));

        [MenuItem(menuPath + "AR Session (Image Tracking with Motion Fusion Preset)", priority = priority)]
        static void ARSessionPresetImageTrackingMotionFusion() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.ImageTrackingMotionFusion, () => CreateFrameSources(ARSessionFactory.ARSessionPreset.ImageTrackingMotionFusion));

        [MenuItem(menuPath + "AR Session (Object Tracking with Motion Fusion Preset)", priority = priority)]
        static void ARSessionPresetObjectTrackingMotionFusion() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.ObjectTrackingMotionFusion, () => CreateFrameSources(ARSessionFactory.ARSessionPreset.ObjectTrackingMotionFusion));

        [MenuItem(menuPath + "Frame Source : Nreal", priority = priority)]
        static void Nreal() => ARSessionFactory.AddFrameSource<NrealFrameSource>(Selection.activeGameObject);

        [MenuItem(menuPath + "AR Session (Mega Preset)", true)]
        [MenuItem(menuPath + "AR Session (Sparse SpatialMap Preset)", true)]
        [MenuItem(menuPath + "AR Session (Dense SpatialMap Preset)", true)]
        [MenuItem(menuPath + "AR Session (Sparse and Dense SpatialMap Preset)", true)]
        [MenuItem(menuPath + "AR Session (Image Tracking with Motion Fusion Preset)", true)]
        [MenuItem(menuPath + "AR Session (Object Tracking with Motion Fusion Preset)", true)]
#if EASYAR_ENABLE_MEGA
        [MenuItem(menuPathMega + "AR Session (Mega Preset) : Nreal", true)]
#endif
        static bool MenuValidateRootObject() => !Selection.activeGameObject;

        [MenuItem(menuPath + "Frame Source : Nreal", true)]
        static bool MenuValidateSessionPart() => ARSessionFactory.IsSessionPartAndEmpty(Selection.activeGameObject);

        static List<GameObject> CreateFrameSources(ARSessionFactory.ARSessionPreset preset)
        {
            var sources = new List<GameObject> { new GameObject(ARSessionFactory.DefaultName<NrealFrameSource>(), typeof(NrealFrameSource)) };
            var list = ARSessionFactory.CreateFrameSources(preset);
            if (list != null)
            {
                sources.AddRange(list);
            }
            return sources;
        }
    }
}
