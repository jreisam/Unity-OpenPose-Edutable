//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class PropGridController : MonoBehaviour
    {
        public GameObject PropCellPrefab;
        public Dragger PropDragger;

        private GridLayoutGroup layout;
        private RectTransform rectTransform;
        private List<PropCellController> cells = new List<PropCellController>();

        private void Start()
        {
            layout = GetComponent<GridLayoutGroup>();
            rectTransform = GetComponent<RectTransform>();

            var cellHeight = rectTransform.rect.height * 0.7f;
            var padding = (int)(cellHeight * 0.1f);
            layout.padding.left = padding;
            layout.cellSize = new Vector2(cellHeight, cellHeight);
            layout.spacing = new Vector2(padding, padding);

            foreach (var templet in PropCollection.Instance.Templets)
            {
                var cell = Instantiate(PropCellPrefab);
                cell.transform.SetParent(transform);
                var controller = cell.GetComponent<PropCellController>();
                controller.SetData(templet);
                controller.PointerDown += () =>
                {
                    PropDragger.StartCreate(controller);
                };
                controller.PointerUp += PropDragger.StopCreate;
                cells.Add(controller);
            }
        }

        private void Update()
        {
            var offset = rectTransform.childCount * (layout.cellSize.x + layout.padding.left) + layout.padding.left - rectTransform.rect.width;
            if (offset > 0)
            {
                var offserMax = rectTransform.offsetMax;
                offserMax.x = offset;
                rectTransform.offsetMax = offserMax;
            }
        }
    }
}
