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
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.Common;

namespace Nebukam.Cluster
{

    /// <summary>
    /// A GridChunk represent a 3D abstract grid with finite slot capacity.
    /// Each slot is stored at a given ByteTrio (x, y, z) location
    /// </summary>
    /// <typeparam name="T_SLOT"></typeparam>
    public class SlotClusterFlex<T_SLOT, T_BRAIN> : SlotCluster<T_SLOT, T_BRAIN>
        where T_SLOT : Slot, ISlot, new()
        where T_BRAIN : struct, IClusterBrain
    {

        internal struct SlotIndex
        {
            public int index;
            public T_SLOT slot;
            public SlotIndex(int i, T_SLOT s)
            {
                index = i;
                slot = s;
            }
        }

        protected internal List<T_SLOT> m_slotList = new List<T_SLOT>();
        protected internal Dictionary<ByteTrio, ISlot> m_slots = new Dictionary<ByteTrio, ISlot>();

        /// <summary>
        /// Number of slots in the cluster
        /// </summary>
        public override int Count { get { return m_slotList.Count; } }
        public override T_SLOT this[int index] { get { return m_slotList[index]; } }
        public override int this[IVertex v] { get { return m_slotList.IndexOf((T_SLOT)v); } }

        /// <summary>
        /// Return the slot index of the given coordinates
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override int IndexOf(ByteTrio coord)
        {
            if (m_slots.TryGetValue(m_brain.Clamp(coord), out ISlot slot))
                return m_slotList.IndexOf(slot as T_SLOT);

            return -1;
        }

        protected override int3 OnSizeChanged(ByteTrio oldSize)
        {
            int3 diff = base.OnSizeChanged(oldSize);

            if (m_slotList.Count == 0 || (diff.x >= 0 && diff.y >= 0 && diff.z >= 0))
                return diff;

            //TOOD : Find a more elegant and efficient way to resize.

            List<SlotIndex> outgoing = new List<SlotIndex>();
            T_SLOT slot;
            ByteTrio coord;
            SlotIndex s;
            int
                sizeX = oldSize.x,
                sizeY = oldSize.y,
                sizeZ = oldSize.z;

            for (int i = 0, count = m_slotList.Count; i < count; i++)
            {
                slot = m_slotList[i];
                coord = slot.m_coordinates;

                if (coord.x >= sizeX || coord.y >= sizeY || coord.z >= sizeZ)
                    outgoing.Add(new SlotIndex(i, slot));

            }

            for (int i = 0, count = outgoing.Count; i < count; i++)
            {
                s = outgoing[i];
                m_slotList.RemoveAt(s.index);
                OnSlotRemoved(s.slot);
                s.slot.Release();
            }

            outgoing.Clear();

            return diff;
        }

        /// <summary>
        /// Set the slot occupation at a given coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <param name="releaseExisting"></param>
        /// <returns></returns>
        public override T_SLOT Set(ByteTrio coord, ISlot slot, bool releaseExisting = false)
        {

            T_SLOT vSlot = slot as T_SLOT;

#if UNITY_EDITOR
            if (vSlot == null)
                throw new System.ArgumentException("slot cannot be null.");
#endif

            if (vSlot.cluster == this)
            {
                if (vSlot.m_coordinates == coord)
                    return vSlot;
                else
                    m_slots.Remove(vSlot.m_coordinates); //a slot can only exists at a single coordinate
            }
            else
            {
                m_slotList.Add(vSlot);
            }

            if (m_slots.TryGetValue(coord, out ISlot existingSlot))
            {
                if (existingSlot == vSlot)
                    return vSlot;

                T_SLOT vExistingSlot = existingSlot as T_SLOT;
                m_slotList.Remove(vExistingSlot);
                OnSlotRemoved(vExistingSlot);

                if (releaseExisting)
                    existingSlot.Release();

            }

            vSlot.m_coordinates = coord;
            OnSlotAdded(vSlot);

            return vSlot;
        }

        /// <summary>
        /// Create a slot at the given coordinates and return it.
        /// If a slot already exists at the given location, that slot is returned instead.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override T_SLOT Add(ByteTrio coord)
        {

            if (m_slots.TryGetValue(coord, out ISlot dslot))
                return dslot as T_SLOT;

            T_SLOT slot = Pool.Rent<T_SLOT>();
            slot.m_coordinates = coord;
            m_slotList.Add(slot);
            OnSlotAdded(slot);

            return slot;
        }

        /// <summary>
        /// Remove the slot at the given coordinates and returns it.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override T_SLOT Remove(ByteTrio coord)
        {
            T_SLOT vSlot;
            if (m_slots.TryGetValue(coord, out ISlot slot))
            {
                vSlot = slot as T_SLOT;
                m_slotList.Remove(vSlot);
                m_slots.Remove(coord);
                return vSlot;
            }

            return null;
        }

        /// <summary>
        /// Gets the slot associated with the specified coordinates.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <remarks>Input coordinates are wrapped per-axis using each of the cluster's individual wrap mode.</remarks>
        public override bool TryGet(ByteTrio coord, out ISlot slot)
        {
            return m_slots.TryGetValue(m_brain.Clamp(coord), out slot);
        }

        /// <summary>
        /// Gets the slot associated with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <remarks>Input coordinates are wrapped per-axis using each of the cluster's individual wrap mode.</remarks>
        public override bool TryGet(int x, int y, int z, out ISlot slot)
        {
            return m_slots.TryGetValue(m_brain.Clamp(x, y, z), out slot);
        }

        /// <summary>
        /// Callback when a slot is added to the cluster.
        /// </summary>
        /// <param name="slot"></param>
        protected override void OnSlotAdded(T_SLOT slot)
        {
            ByteTrio coord = slot.m_coordinates;
            m_slots[coord] = slot;
            base.OnSlotAdded(slot);
        }

        /// <summary>
        /// Update all slot positions.
        /// </summary>
        protected override void UpdatePositions()
        {
            T_SLOT slot;
            for (int i = 0, count = m_slotList.Count; i < count; i++)
            {
                slot = m_slotList[i];
                slot.pos = m_brain.ComputePosition(ref slot.m_coordinates);
            }
        }

        /// <summary>
        /// Fills all empty slots in this cluster with.
        /// </summary>
        public override void Fill()
        {

#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("If you need to use Fill() on a SlotClusterFlex<V>, " +
                "using a SlotClusterFixed<V> instead will yield better performance & reduce memory fragmentation.");
#endif

            int
                sizeX = m_size.x,
                sizeY = m_size.y,
                sizeZ = m_size.z;

            ByteTrio coords;
            float3 o = pos, origin = pos, slotSize = m_slotModel.size;
            m_slotList.Capacity = m_size.Volume();

            for (int z = 0; z < sizeZ; z++) // volume
            {
                for (int y = 0; y < sizeY; y++) // plane
                {
                    for (int x = 0; x < sizeX; x++) // line
                    {
                        coords = new ByteTrio(x, y, z);
                        if (!TryGet(coords, out ISlot slot))
                            Add(coords);
                    }
                }
            }
        }

        /// <summary>
        /// Clear cluster & releases all slots.
        /// </summary>
        public override void Clear(bool release = false)
        {
            T_SLOT slot;
            for (int i = 0, count = m_slotList.Count; i < count; i++)
            {
                slot = m_slotList[i];
                OnSlotRemoved(slot);

                if (release)
                    slot.Release();
            }

            m_slots.Clear();
            m_slotList.Clear();
        }

        #region Nearest vertex in group

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override int GetNearestVertexIndex(IVertex v)
        {
            int index = -1, count = m_slotList.Count;
            float dist, sDist = float.MaxValue;
            float3 A = v.pos, B, C;
            IVertex oV;
            for (int i = 0; i < count; i++)
            {
                oV = m_slotList[i];

                if (oV == v) { continue; }

                B = oV.pos;
                C = float3(A.x - B.x, A.y - B.y, A.z - B.z);
                dist = C.x * C.x + C.y * C.y + C.z * C.z;

                if (dist > sDist)
                {
                    sDist = dist;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Return the the nearest IVertex in group to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override T_SLOT GetNearestVertex(IVertex v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_slotList[index];
        }

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override int GetNearestVertexIndex(float3 v)
        {
            int index = -1, count = m_slotList.Count;
            float dist, sDist = float.MaxValue;
            float3 B, C;
            for (int i = 0; i < count; i++)
            {
                B = m_slotList[i].pos;
                C = float3(v.x - B.x, v.y - B.y, v.z - B.z);
                dist = C.x * C.x + C.y * C.y + C.z * C.z;

                if (dist > sDist)
                {
                    sDist = dist;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Return the nearest IVertex in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override T_SLOT GetNearestVertex(float3 v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_slotList[index];
        }



        #endregion

    }
}
