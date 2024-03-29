﻿// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Nebukam.Cluster
{

    [CreateAssetMenu(fileName = "SlotModelData", menuName = "N:Toolkit/Cluster/SlotModelData", order = 1)]
    public class SlotModelData : ScriptableObject
    {

        protected SlotModel m_slotModel = new SlotModel();

        [Header("Slot settings")]
        public float3 slotSize = float3(1f);
        public float3 slotAnchor = float3(0f);

        public SlotModel model
        {
            get
            {
                m_slotModel.m_slotSize = slotSize;
                m_slotModel.anchor = slotAnchor;
                return m_slotModel;
            }
        }

        public static implicit operator SlotModel(SlotModelData m) { return m.model; }

    }

}
