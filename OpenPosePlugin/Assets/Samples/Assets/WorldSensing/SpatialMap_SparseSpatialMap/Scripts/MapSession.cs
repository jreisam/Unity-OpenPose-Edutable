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

namespace SpatialMap_SparseSpatialMap
{
    public class MapSession : IDisposable
    {
        public SparseSpatialMapWorkerFrameFilter MapWorker;
        public List<MapData> Maps = new List<MapData>();

        private SparseSpatialMapController builderMapController;

        public MapSession(SparseSpatialMapWorkerFrameFilter mapWorker, List<MapMeta> maps)
        {
            MapWorker = mapWorker;
            if (maps != null)
            {
                foreach (var meta in maps)
                {
                    Maps.Add(new MapData() { Meta = meta });
                }
            }
        }

        ~MapSession()
        {
            foreach (var map in Maps)
            {
                if (map.Controller) { UnityEngine.Object.Destroy(map.Controller.gameObject); }
                foreach (var prop in map.Props) { if (prop) { UnityEngine.Object.Destroy(prop); } }
            }
            if (builderMapController) { UnityEngine.Object.Destroy(builderMapController.gameObject); }
        }

        public bool IsSaving { get; private set; }
        public bool Saved { get; private set; }

        public static void ClearCache()
        {
            if (SparseSpatialMapManager.isAvailable())
            {
                using (var manager = SparseSpatialMapManager.create())
                {
                    manager.clear();
                }
            }
        }

        public void Dispose()
        {
            foreach (var map in Maps)
            {
                if (map.Controller) { UnityEngine.Object.Destroy(map.Controller.gameObject); }
                foreach (var prop in map.Props) { if (prop) { UnityEngine.Object.Destroy(prop); } }
            }
            if (builderMapController) { UnityEngine.Object.Destroy(builderMapController.gameObject); }
            GC.SuppressFinalize(this);
        }

        public void SetupMapBuilder(SparseSpatialMapController mapControllerPrefab)
        {
            builderMapController = UnityEngine.Object.Instantiate(mapControllerPrefab);
            builderMapController.SourceType = SparseSpatialMapController.DataSource.MapBuilder;
            builderMapController.MapWorker = MapWorker;
            builderMapController.ShowPointCloud = true;
        }

        public void LoadMapMeta(SparseSpatialMapController mapControllerPrefab, bool particleShow)
        {
            if (!MapWorker || MapWorker.Localizer == null) { return; }
            if (Maps.Count > 1)
            {
                // This will take effect after the next MapWorker Start or OnEnable is called.
                // In this sample, we are sure about this according to the calling sequence.
                MapWorker.LocalizerConfig.LocalizationMode = LocalizationMode.KeepUpdate;
            }
            foreach (var m in Maps)
            {
                var meta = m.Meta;
                var controller = UnityEngine.Object.Instantiate(mapControllerPrefab);
                controller.SourceType = SparseSpatialMapController.DataSource.MapManager;
                controller.MapManagerSource = meta.Map;
                controller.MapWorker = MapWorker;
                controller.ShowPointCloud = particleShow;
                controller.MapLoad += (map, status, error) =>
                {
                    GUIPopup.EnqueueMessage("Load map {name = " + map.Name + ", id = " + map.ID + "} into " + MapWorker.name + Environment.NewLine +
                        " => " + status + (string.IsNullOrEmpty(error) ? "" : " <" + error + ">"), status ? 3 : 5);
                    if (!status)
                    {
                        return;
                    }
                    GUIPopup.EnqueueMessage("Notice: By default (MainViewRecycleBinClearMapCacheOnly == false)," + Environment.NewLine +
                        "load map will not trigger a download in this sample." + Environment.NewLine +
                        "Map cache is used (SparseSpatialMapManager.clear not called alone)." + Environment.NewLine +
                        "Statistical request count will not be increased (more details on EasyAR website).", 5);

                    foreach (var propInfo in meta.Props)
                    {
                        GameObject prop = null;
                        foreach (var templet in PropCollection.Instance.Templets)
                        {
                            if (templet.Object.name == propInfo.Name)
                            {
                                prop = UnityEngine.Object.Instantiate(templet.Object);
                                break;
                            }
                        }
                        if (!prop)
                        {
                            Debug.LogError("Missing prop templet: " + propInfo.Name);
                            continue;
                        }
                        prop.transform.parent = controller.transform;
                        prop.transform.localPosition = new UnityEngine.Vector3(propInfo.Position[0], propInfo.Position[1], propInfo.Position[2]);
                        prop.transform.localRotation = new Quaternion(propInfo.Rotation[0], propInfo.Rotation[1], propInfo.Rotation[2], propInfo.Rotation[3]);
                        prop.transform.localScale = new UnityEngine.Vector3(propInfo.Scale[0], propInfo.Scale[1], propInfo.Scale[2]);
                        prop.name = propInfo.Name;
                        m.Props.Add(prop);
                    }
                };

                controller.MapLocalized += () =>
                {
                    GUIPopup.EnqueueMessage("Localized map {name = " + controller.MapInfo.Name + "}", 3);
                    var parameter = controller.PointCloudParticleParameter;
                    parameter.StartColor = new Color32(11, 205, 255, 255);
                    controller.PointCloudParticleParameter = parameter;
                };
                controller.MapStopLocalize += () =>
                {
                    GUIPopup.EnqueueMessage("Stopped localize map {name = " + controller.MapInfo.Name + "}", 3);
                    var parameter = controller.PointCloudParticleParameter;
                    parameter.StartColor = new Color32(163, 236, 255, 255);
                    controller.PointCloudParticleParameter = parameter;
                };
                m.Controller = controller;
            }
            MapWorker.Localizer.startLocalization();
        }

        public void Save(string name, Optional<Image> preview)
        {
            IsSaving = true;
            MapWorker.BuilderMapController.MapHost += (map, isSuccessful, error) =>
            {
                if (isSuccessful)
                {
                    var mapMeta = new MapMeta(map, new List<MapMeta.PropInfo>());
                    MapMetaManager.Save(mapMeta);
                    GUIPopup.EnqueueMessage("Map Generated", 3);
                    GUIPopup.EnqueueMessage("Notice: map name can be changed on website," + Environment.NewLine +
                        "while the SDK side will not get updated until map cache cleared and a re-download has been triggered.", 5);
                    Saved = true;
                }
                else
                {
                    GUIPopup.EnqueueMessage("Fail to generate Map" + (string.IsNullOrEmpty(error) ? "" : "\n error: " + error), 5);
                }
                IsSaving = false;
            };
            try
            {
                MapWorker.BuilderMapController.Host(name, preview);
            }
            catch (Exception e)
            {
                GUIPopup.EnqueueMessage("Fail to host map: " + e.Message, 5);
                IsSaving = false;
            }
        }

        public Optional<Vector3> HitTestOne(Vector2 pointInView)
        {
            if (!MapWorker || !MapWorker.LocalizedMap)
            {
                return Optional<Vector3>.CreateNone();
            }
            foreach (var point in MapWorker.LocalizedMap.HitTest(pointInView))
            {
                return MapWorker.LocalizedMap.transform.TransformPoint(point);
            }
            return Optional<Vector3>.CreateNone();
        }

        public class MapData
        {
            public MapMeta Meta;
            public SparseSpatialMapController Controller;
            public List<GameObject> Props = new List<GameObject>();
        }
    }
}
