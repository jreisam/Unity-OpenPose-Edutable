//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using Common;
using easyar;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapLocalizing_Sparse
{
    public class MapLocalizing_SparseSample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public SparseSpatialMapController MapController;
        public TouchController TouchControl;

        private VIOCameraDeviceUnion vioCamera;
        private SparseSpatialMapWorkerFrameFilter sparse;
        private bool onSparse;

        private void Start()
        {
            sparse = Session.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
            TouchControl.TurnOn(TouchControl.gameObject.transform, Camera.main, false, false, true, false);

            if (string.IsNullOrEmpty(MapController.MapManagerSource.ID) || string.IsNullOrEmpty(MapController.MapManagerSource.Name))
            {
                throw new UIPopupException("Map ID or Name NOT set, please set MapManagerSource on: " + MapController + Environment.NewLine +
                    "To create SpatialMap, use <SpatialMap_SparseSpatialMap> sample." + Environment.NewLine +
                    "To get Map ID and Name, use EasyAR Develop Center (www.easyar.com) -> SpatialMap -> Database Details." + Environment.NewLine +
                    "Map ID is used when loading, it can be used to share maps among devices.", 100);
            }

            MapController.MapLoad += (map, status, error) =>
            {
                GUIPopup.EnqueueMessage("Load map {name = " + map.Name + ", id = " + map.ID + "} into " + sparse.name + Environment.NewLine +
                    " => " + status + (string.IsNullOrEmpty(error) ? "" : " <" + error + ">"), status ? 3 : 5);

                if (!status)
                {
                    return;
                }

                GUIPopup.EnqueueMessage("Notice: load map (only the first time each map) will trigger a download in this sample." + Environment.NewLine +
                    "Statistical request count will be increased (more details on EasyAR website)." + Environment.NewLine +
                    "Map cache is used after a successful download." + Environment.NewLine +
                    "Map cache will be cleared if SparseSpatialMapManager.clear is called or app uninstalled.", 5);
            };

            MapController.MapLocalized += () =>
            {
                GUIPopup.EnqueueMessage("Localized map {name = " + MapController.MapInfo.Name + "}", 3);
            };

            MapController.MapStopLocalize += () =>
            {
                GUIPopup.EnqueueMessage("Stopped localize map {name = " + MapController.MapInfo.Name + "}", 3);
            };

            sparse.Localizer.startLocalization();
        }

        private void Update()
        {
            Status.text = "VIO Device" + Environment.NewLine +
                "\tType: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "\tTracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "Sparse Spatial Map" + Environment.NewLine +
                "\tWorking Mode: " + sparse.WorkingMode + Environment.NewLine +
                "\tLocalization Mode: " + sparse.LocalizerConfig.LocalizationMode + Environment.NewLine +
                "Localized Map" + Environment.NewLine +
                "\tName: " + (sparse.LocalizedMap == null ? "-" : (sparse.LocalizedMap.MapInfo == null ? "-" : sparse.LocalizedMap.MapInfo.Name)) + Environment.NewLine +
                "\tID: " + (sparse.LocalizedMap == null ? "-" : (sparse.LocalizedMap.MapInfo == null ? "-" : sparse.LocalizedMap.MapInfo.ID)) + Environment.NewLine +
                "\tPoint Cloud Count: " + (sparse.LocalizedMap == null ? "-" : sparse.LocalizedMap.PointCloud.Count.ToString()) + Environment.NewLine +
                "Cube Location: " + (onSparse ? "On Sparse Spatial Map" : (sparse.TrackingStatus != MotionTrackingStatus.NotTracking ? "Air" : "-")) + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove to Sparse Spatial Map Point: One Finger Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (sparse && sparse.LocalizedMap)
                    {
                        var points = sparse.LocalizedMap.HitTest(viewPoint);
                        foreach (var point in points)
                        {
                            onSparse = true;
                            TouchControl.transform.position = sparse.LocalizedMap.transform.TransformPoint(point);
                            break;
                        }
                    }
                }
            }
        }
    }
}
