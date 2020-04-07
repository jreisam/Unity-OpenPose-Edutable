//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialMap_SparseSpatialMap
{
    public class PropCollection : MonoBehaviour
    {
        public static PropCollection Instance;
        public List<Templet> Templets = new List<Templet>();

        private void Awake()
        {
            Instance = this;
        }

        [Serializable]
        public class Templet
        {
            public GameObject Object;
            public Sprite Icon;
        }
    }
}
