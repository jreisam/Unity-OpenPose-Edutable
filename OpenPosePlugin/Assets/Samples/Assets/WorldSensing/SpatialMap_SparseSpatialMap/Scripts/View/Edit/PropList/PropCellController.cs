//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class PropCellController : MonoBehaviour
    {
        public Image Icon;

        public event Action PointerDown;
        public event Action PointerUp;

        public PropCollection.Templet Templet { get; private set; }

        public void SetData(PropCollection.Templet templet)
        {
            Templet = templet;
            Icon.sprite = templet.Icon;
        }

        public void OnPointerDown()
        {
            Icon.color = Color.gray;
            if (PointerDown != null)
            {
                PointerDown();
            }
        }

        public void OnPointerUp()
        {
            Icon.color = Color.white;
            if (PointerUp != null)
            {
                PointerUp();
            }
        }
    }
}
