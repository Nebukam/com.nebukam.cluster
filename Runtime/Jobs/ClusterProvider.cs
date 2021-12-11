// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com.
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

using System.Collections.Generic;
using Unity.Collections;

using Nebukam.JobAssist;
using static Nebukam.JobAssist.Extensions;

namespace Nebukam.Cluster
{

    public interface IClusterProvider<T_SLOT, T_SLOT_INFOS, T_BRAIN> : IProcessor
        where T_SLOT : ISlot
        where T_SLOT_INFOS : struct, ISlotInfos<T_SLOT>
        where T_BRAIN : struct, IClusterBrain
    {
        ISlotCluster<T_SLOT, T_BRAIN> slotCluster { get; set; }
        List<T_SLOT> lockedSlots { get; }
        NativeArray<T_SLOT_INFOS> outputSlotInfos { get; }
        NativeHashMap<ByteTrio, int> outputSlotCoordinateMap { get; }
    }

    public class ClusterProvider<S, T, B> : Processor<ClusterMappingJob<S, T, B>>, IClusterProvider<S, T, B>
        where S : Slot, ISlot
        where T : struct, ISlotInfos<S>
        where B : struct, IClusterBrain
    {

        protected ISlotCluster<S, B> m_slotCluster = null;
        protected List<S> m_lockedSlots = new List<S>();
        protected NativeArray<T> m_outputSlotInfos = default;
        protected NativeHashMap<ByteTrio, int> m_outputSlotCoordMap = default;

        public ISlotCluster<S, B> slotCluster
        {
            get { return m_slotCluster; }
            set { m_slotCluster = value; }
        }
        public List<S> lockedSlots { get { return m_lockedSlots; } }
        public NativeArray<T> outputSlotInfos { get { return m_outputSlotInfos; } }
        public NativeHashMap<ByteTrio, int> outputSlotCoordinateMap { get { return m_outputSlotCoordMap; } }

        protected override void InternalLock()
        {
            int count = m_slotCluster.Count;
            m_lockedSlots.Clear();
            m_lockedSlots.Capacity = count;
            for (int i = 0; i < count; i++) { m_lockedSlots.Add(m_slotCluster[i]); }
        }

        protected override void Prepare(ref ClusterMappingJob<S, T, B> job, float delta)
        {

            int slotCount = m_lockedSlots.Count;

            MakeLength(ref m_outputSlotInfos, slotCount);
            if(MakeLength(ref m_outputSlotCoordMap, slotCount))
                m_outputSlotCoordMap.Clear();

            S slot;
            T slotInfos;

            for (int i = 0; i < slotCount; i++)
            {

                slot = m_lockedSlots[i];
                slotInfos = default;

                slotInfos.index = i;
                slotInfos.coord = slot.m_coordinates;

                slotInfos.Capture(slot);

                m_outputSlotInfos[i] = slotInfos;

            }

            job.m_brain = m_slotCluster.brain;
            job.m_inputSlotInfos = m_outputSlotInfos;
            job.m_coordinateMap = m_outputSlotCoordMap;

        }

        protected override void InternalDispose()
        {
            m_lockedSlots.Clear();
            m_lockedSlots = null;
            m_slotCluster = null;

            m_outputSlotInfos.Release();
            m_outputSlotCoordMap.Release();
        }

    }
}
