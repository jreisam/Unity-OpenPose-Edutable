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

namespace SpatialMap_SparseSpatialMap
{
    [Serializable]
    public class MapMeta
    {
        public SparseSpatialMapController.MapManagerSourceData Map = new SparseSpatialMapController.MapManagerSourceData();
        public List<PropInfo> Props = new List<PropInfo>();

        public MapMeta(SparseSpatialMapController.SparseSpatialMapInfo map, List<PropInfo> props)
        {
            Map = new SparseSpatialMapController.MapManagerSourceData() { Name = map.Name, ID = map.ID };
            Props = props;
        }

        [Serializable]
        public class PropInfo
        {
            public string Name = string.Empty;
            public float[] Position = new float[3];
            public float[] Rotation = new float[4];
            public float[] Scale = new float[3];
        }
    }
}
