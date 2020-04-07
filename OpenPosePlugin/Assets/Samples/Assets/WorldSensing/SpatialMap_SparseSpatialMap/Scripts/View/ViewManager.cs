//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance;
        public GameObject EasyARSession;
        public SparseSpatialMapController MapControllerPrefab;
        public MainViewController MainView;
        public CreateViewController CreateView;
        public EditViewController EditView;
        public GameObject PreviewView;
        public Text Status;
        public Toggle EditPointCloudUI;
        public Toggle PreviewPointCloudUI;
        public Text RecycleBinText;
        public bool MainViewRecycleBinClearMapCacheOnly;

        private GameObject easyarObject;
        private ARSession session;
        private VIOCameraDeviceUnion vioCamera;
        private SparseSpatialMapWorkerFrameFilter mapFrameFilter;
        private List<MapMeta> selectedMaps = new List<MapMeta>();
        private MapSession mapSession;

        private void Awake()
        {
            Instance = this;
            MainView.gameObject.SetActive(false);
            CreateView.gameObject.SetActive(false);
            EditView.gameObject.SetActive(false);
            PreviewView.SetActive(false);
            if (MainViewRecycleBinClearMapCacheOnly)
            {
                RecycleBinText.text = "Delete only maps caches";
            }
            else
            {
                RecycleBinText.text = "Delete all maps and caches";
            }
        }

        private void Start()
        {
            LoadMainView();
        }

        private void Update()
        {
            if (session)
            {
                Status.text = "VIO Device" + Environment.NewLine +
                    "\tType: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                    "\tTracking Status: " + (session.WorldRootController == null ? "-" : session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                    "Sparse Spatial Map" + Environment.NewLine +
                    "\tWorking Mode: " + mapFrameFilter.WorkingMode + Environment.NewLine +
                    "\tLocalization Mode: " + mapFrameFilter.LocalizerConfig.LocalizationMode + Environment.NewLine +
                    "Localized Map" + Environment.NewLine +
                    "\tName: " + (mapFrameFilter.LocalizedMap == null ? "-" : (mapFrameFilter.LocalizedMap.MapInfo == null ? "-" : mapFrameFilter.LocalizedMap.MapInfo.Name)) + Environment.NewLine +
                    "\tID: " + (mapFrameFilter.LocalizedMap == null ? "-" : (mapFrameFilter.LocalizedMap.MapInfo == null ? "-" : mapFrameFilter.LocalizedMap.MapInfo.ID)) + Environment.NewLine +
                    "\tPoint Cloud Count: " + (mapFrameFilter.LocalizedMap == null ? "-" : mapFrameFilter.LocalizedMap.PointCloud.Count.ToString());

                if (mapFrameFilter.LocalizedMap == null)
                {
                    EditPointCloudUI.gameObject.SetActive(false);
                    PreviewPointCloudUI.gameObject.SetActive(false);
                }
                else
                {
                    EditPointCloudUI.gameObject.SetActive(true);
                    PreviewPointCloudUI.gameObject.SetActive(true);
                }
            }
            else
            {
                Status.text = string.Empty;
            }
        }

        private void OnDestroy()
        {
            DestroySession();
        }

        public void SelectMaps(List<MapMeta> metas)
        {
            selectedMaps = metas;
            MainView.EnablePreview(selectedMaps.Count > 0);
            MainView.EnableEdit(selectedMaps.Count == 1);
        }

        public void LoadMainView()
        {
            DestroySession();
            SelectMaps(new List<MapMeta>());
            MainView.gameObject.SetActive(true);
        }

        public void LoadCreateView()
        {
            CreateSession();
            mapSession.SetupMapBuilder(MapControllerPrefab);
            CreateView.SetMapSession(mapSession);
            CreateView.gameObject.SetActive(true);
        }

        public void LoadEditView()
        {
            CreateSession();
            mapSession.LoadMapMeta(MapControllerPrefab, true);
            EditView.SetMapSession(mapSession);
            EditView.gameObject.SetActive(true);
            EditPointCloudUI.isOn = true;
        }

        public void LoadPreviewView()
        {
            CreateSession();
            mapSession.LoadMapMeta(MapControllerPrefab, false);
            PreviewView.SetActive(true);
            PreviewPointCloudUI.isOn = false;
        }

        public void ShowParticle(bool show)
        {
            if (mapSession == null)
            {
                return;
            }
            foreach (var map in mapSession.Maps)
            {
                if (map.Controller) { map.Controller.ShowPointCloud = show; }
            }
        }

        private void CreateSession()
        {
            easyarObject = Instantiate(EasyARSession);
            easyarObject.SetActive(true);
            session = easyarObject.GetComponent<ARSession>();
            vioCamera = easyarObject.GetComponentInChildren<VIOCameraDeviceUnion>();
            mapFrameFilter = easyarObject.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();

            mapSession = new MapSession(mapFrameFilter, selectedMaps);
        }

        private void DestroySession()
        {
            if (mapSession != null)
            {
                mapSession.Dispose();
                mapSession = null;
            }
            if (easyarObject) { Destroy(easyarObject); }
        }
    }
}
