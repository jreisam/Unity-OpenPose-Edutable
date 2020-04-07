//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class MapGridController : MonoBehaviour
    {
        public GameObject MapCellPrefab;

        private GridLayoutGroup layout;
        private RectTransform rectTransform;
        private List<MapCellController> cells = new List<MapCellController>();

        public int CellCount { get { return cells.Count; } }

        private void OnEnable()
        {
            foreach (var meta in MapMetaManager.LoadAll())
            {
                var cell = Instantiate(MapCellPrefab);
                cell.transform.SetParent(transform);
                var controller = cell.GetComponent<MapCellController>();
                controller.SetData(meta);
                controller.PointerDown += OnCellChange;
                controller.Delete += () =>
                {
                    if (cells.Remove(controller))
                    {
                        MapMetaManager.Delete(controller.MapMeta);
                        Destroy(controller.gameObject);
                        OnCellChange();
                        easyar.GUIPopup.EnqueueMessage(
                            "DELETED: {(Sample) Meta Data}" + Environment.NewLine +
                            "NOT DELETED: {Map Cache, Map on Server}" + Environment.NewLine +
                            "Use recycle bin button to delete map cache" + Environment.NewLine +
                            "Use web develop center to manage maps on server", 5);
                    }
                };
                cells.Add(controller);
            }
        }

        private void Start()
        {
            layout = GetComponent<GridLayoutGroup>();
            rectTransform = GetComponent<RectTransform>();
            var cellWidth = rectTransform.rect.width * 0.9f;
            var padding = (int)(cellWidth * 0.1f);
            layout.padding.top = padding;
            layout.cellSize = new Vector2(cellWidth, cellWidth * 0.5f);
            layout.spacing = new Vector2(padding, padding);
        }

        private void Update()
        {
            var sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = rectTransform.childCount * (layout.cellSize.y + layout.padding.top) + layout.padding.top;
            rectTransform.sizeDelta = sizeDelta;
        }

        private void OnDisable()
        {
            foreach (var cell in cells)
            {
                if (cell) { Destroy(cell.gameObject); }
            }
            cells.Clear();
        }

        public void ClearAll()
        {
            // Notice:
            //   a) When clear both map cache and map list,
            //      load map will not trigger a download (cache is build when upload),
            //      and statistical request count will not be increased in a subsequent load (when edit or preview).
            //   b) When clear map cache only,
            //      load map after clear (only the first time each map) will trigger a download,
            //      and statistical request count will be increased in a subsequent load (when edit or preview).
            //      Map cache is used after a successful download and will be cleared if SparseSpatialMapManager.clear is called or app uninstalled.
            //
            // More about the statistical request count and limitations for different subscription mode can be found at EasyAR website.

            if (!ViewManager.Instance.MainViewRecycleBinClearMapCacheOnly)
            {
                // clear map meta and the list on UI
                foreach (var cell in cells)
                {
                    if (cell)
                    {
                        MapMetaManager.Delete(cell.MapMeta);
                        Destroy(cell.gameObject);
                    }
                }
                cells.Clear();
            }

            // clear map cache
            MapSession.ClearCache();

            // UI notification
            OnCellChange();
            if (!ViewManager.Instance.MainViewRecycleBinClearMapCacheOnly)
            {
                easyar.GUIPopup.EnqueueMessage(
                    "DELETED: {(Sample) Meta Data, Map Cache}" + Environment.NewLine +
                    "NOT DELETED: {Map on Server}" + Environment.NewLine +
                    "Use web develop center to manage maps on server", 5);
            }
            else
            {
                easyar.GUIPopup.EnqueueMessage(
                    "DELETED: {Map Cache}" + Environment.NewLine +
                    "NOT DELETED: {Map on Server, (Sample) Meta Data}" + Environment.NewLine +
                    "Use web develop center to manage maps on server", 5);
            }
        }

        private void OnCellChange()
        {
            ViewManager.Instance.SelectMaps(cells.Where(cell => cell && cell.Selected).Select(cell => cell.MapMeta).ToList());
        }
    }
}
