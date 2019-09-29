// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com.
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
using static Unity.Mathematics.math;

namespace Nebukam.Cluster
{

    /// <summary>
    /// A GridChunk represent a 3D abstract grid with finite slot capacity.
    /// Each slot is stored at a given ByteTrio (x, y, z) location
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class SlotClusterFixed<V, B> : SlotCluster<V, B>, IPlanarOrder
        where V : Slot, ISlot, new()
        where B : struct, IClusterBrain
    {

        protected internal delegate int ComputeIndexOf(ByteTrio coord);

        protected internal V[] m_slotList = null;
        protected internal AxisOrder m_planeOrder = AxisOrder.XYZ;
        protected internal ComputeIndexOf
            m_indexOf,
            m_indexOfXYZ,
            m_indexOfXZY,
            m_indexOfYXZ,
            m_indexOfYZX,
            m_indexOfZXY,
            m_indexOfZYX;
        protected internal int
            m_lineLength = 0,
            m_planeLength = 0;

        /// <summary>
        /// Number of slots in the cluster
        /// </summary>
        public override int Count { get { return m_slotList.Length; } }
        public override V this[int index] { get { return m_slotList[index]; } }
        public override int this[IVertex v] { get { return IndexOf((v as V).m_coordinates); } }

        /// <summary>
        /// Plane order defined the order in which slots are ordered.
        /// If you intend to shrink and/or expand the cluster size at runtime,
        /// using the right plane order will greatly improve performance.
        /// </summary>
        public AxisOrder planeOrder
        {
            get { return m_planeOrder; }
            set
            {
                m_planeOrder = value;
                if (value == AxisOrder.XZY) // (x * z) * y
                {
                    if (m_indexOfXZY == null) { m_indexOfXZY = d_indexOfXZY; }
                    m_indexOf = m_indexOfXZY;
                    m_lineLength = m_size.x;
                    m_planeLength = m_size.z * m_lineLength;
                }
                else if (value == AxisOrder.YXZ) // (y * x) * z
                {
                    if (m_indexOfYXZ == null) { m_indexOfYXZ = d_indexOfYXZ; }
                    m_indexOf = m_indexOfYXZ;
                    m_lineLength = m_size.y;
                    m_planeLength = m_size.x * m_lineLength;
                }
                else if (value == AxisOrder.YZX) // (y * z) * x
                {
                    if (m_indexOfYZX == null) { m_indexOfYZX = d_indexOfYZX; }
                    m_indexOf = m_indexOfYZX;
                    m_lineLength = m_size.y;
                    m_planeLength = m_size.z * m_lineLength;
                }
                else if (value == AxisOrder.ZXY) // (y * z) * x
                {
                    if (m_indexOfZXY == null) { m_indexOfZXY = d_indexOfZXY; }
                    m_indexOf = m_indexOfZXY;
                    m_lineLength = m_size.z;
                    m_planeLength = m_size.x * m_lineLength;
                }
                else if (value == AxisOrder.ZYX) // (z * y) * x
                {
                    if (m_indexOfZYX == null) { m_indexOfZYX = d_indexOfZYX; }
                    m_indexOf = m_indexOfZYX;
                    m_lineLength = m_size.z;
                    m_planeLength = m_size.y * m_lineLength;
                }
                else // AxisOrder.XYZ is default. (x * y) * z
                {
                    if (m_indexOfXYZ == null) { m_indexOfXYZ = d_indexOfXYZ; }
                    m_indexOf = m_indexOfXYZ;
                    m_lineLength = m_size.x;
                    m_planeLength = m_size.y * m_lineLength;
                }
            }
        }

        public override int IndexOf(ByteTrio coord)
        {
            //Compute index based on X Y Z values, according to current plane order.
            m_brain.Clamp(ref coord);

            if (!m_brain.Contains(ref coord))
                return -1;

            return coord.x + m_lineLength * coord.y + m_planeLength * coord.z;
        }

        #region IndexOf delegates

        protected internal int d_indexOfXYZ(ByteTrio coord)
        {
            return coord.x + m_lineLength * coord.y + m_planeLength * coord.z;
        }

        protected internal int d_indexOfXZY(ByteTrio coord)
        {
            return coord.x + m_lineLength * coord.z + m_planeLength * coord.y;
        }

        protected internal int d_indexOfYXZ(ByteTrio coord)
        {
            return coord.y + m_lineLength * coord.x + m_planeLength * coord.z;
        }

        protected internal int d_indexOfYZX(ByteTrio coord)
        {
            return coord.y + m_lineLength * coord.z + m_planeLength * coord.x;
        }

        protected internal int d_indexOfZXY(ByteTrio coord)
        {
            return coord.z + m_lineLength * coord.x + m_planeLength * coord.y;
        }

        protected internal int d_indexOfZYX(ByteTrio coord)
        {
            return coord.z + m_lineLength * coord.y + m_planeLength * coord.x;
        }

        #endregion

        protected override int3 OnSizeChanged(ByteTrio oldSize)
        {
            planeOrder = m_planeOrder;

            int3 diff = base.OnSizeChanged(oldSize);
            int oldVolume = oldSize.Volume(), newVolume = m_size.Volume();

            if (m_slotList == null)
            {
                m_slotList = new V[newVolume];
                return diff;
            }

            if (diff.x == 0 && diff.y == 0 && diff.z == 0)
                return diff;

            V oldSlot;
            V[] oldList = m_slotList;
            m_slotList = new V[newVolume];

            int 
                sizeX = oldSize.x,
                sizeY = oldSize.y,
                sizeZ = oldSize.z,
                oldLineLength = sizeX,
                oldPlaneLength = sizeY * oldLineLength,
                x = 0, y = 0, z = 0,
                index;

            ByteTrio coord;
            
            for (z = 0; z < sizeZ; z++) // volume
            {
                for (y = 0; y < sizeY; y++) // plane
                {
                    for (x = 0; x < sizeX; x++) // line
                    {

                        oldSlot = oldList[x + oldLineLength * y + oldPlaneLength * z];

                        if (oldSlot == null)
                            continue;

                        coord = new ByteTrio(x,y,z);
                        index = IndexOf(coord); // Index in resized context

                        if (index == -1)
                        {
                            // Old slot pops out.
                            OnSlotRemoved(oldSlot);
                            oldSlot.Release();
                        }
                        else
                        {
                            // Move old slot
                            m_slotList[index] = oldSlot;
                        }

                    }
                }
            }

            return diff;
        }

        #region init methods

        public override void Init(
            SlotModel clusterSlotModel, 
            ByteTrio clusterSize, 
            bool fillCluster)
        {
            Init(clusterSlotModel, clusterSize, fillCluster, AxisOrder.XYZ);
        }

        public virtual void Init(
            SlotModel clusterSlotModel, 
            ByteTrio clusterSize, 
            bool fillCluster, 
            AxisOrder clusterPlaneOrder)
        {
            planeOrder = clusterPlaneOrder;
            base.Init(clusterSlotModel, clusterSize, fillCluster);
        }

        public override void Init(
            SlotModel clusterSlotModel, 
            B clusterBrain, 
            bool fillCluster)
        {
            Init(clusterSlotModel, clusterBrain, fillCluster, AxisOrder.XYZ);
        }

        public virtual void Init(
            SlotModel clusterSlotModel,
            B clusterBrain,
            bool fillCluster,
            AxisOrder clusterPlaneOrder)
        {
            planeOrder = clusterPlaneOrder;
            base.Init(clusterSlotModel, clusterBrain, fillCluster);
        }

        public override void Init(
            SlotModel clusterSlotModel, 
            ByteTrio clusterSize, 
            WrapMode wrapX, 
            WrapMode wrapY, 
            WrapMode wrapZ, 
            bool fillCluster)
        {
            Init(clusterSlotModel, clusterSize, wrapX, wrapY, wrapZ, fillCluster, AxisOrder.XYZ);
        }

        public virtual void Init(
            SlotModel clusterSlotModel,
            ByteTrio clusterSize,
            WrapMode wrapX,
            WrapMode wrapY,
            WrapMode wrapZ,
            bool fillCluster,
            AxisOrder clusterPlaneOrder)
        {
            planeOrder = clusterPlaneOrder;
            base.Init(clusterSlotModel, clusterSize, wrapX, wrapY, wrapZ, fillCluster);
        }

        #endregion

        /// <summary>
        /// Set the slot occupation at a given coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <param name="releaseExisting"></param>
        /// <returns></returns>
        public override V Set(ByteTrio coord, ISlot slot, bool releaseExisting = false)
        {

            int index = IndexOf(coord);

            if (index == -1)
                return null;

            V vSlot = slot as V;

#if UNITY_EDITOR
            if (vSlot == null)
                throw new System.ArgumentException("slot cannot be null.");
#endif

            if (vSlot.cluster == this)
            {
                if (vSlot.m_coordinates == coord)
                    return vSlot;
                else
                    Remove(vSlot.m_coordinates); //a slot can only exists at a single coordinate
            }

            V existingSlot = m_slotList[index];

            if (existingSlot != null)
            {
                if (existingSlot == vSlot)
                    return vSlot;

                m_slotList[index] = null;
                OnSlotRemoved(existingSlot);

                if (releaseExisting)
                    existingSlot.Release();

            }

            vSlot.m_coordinates = coord;
            m_slotList[index] = vSlot;
            OnSlotAdded(vSlot);

            return vSlot;
        }

        /// <summary>
        /// Create a slot at the given coordinates and return it.
        /// If a slot already exists at the given location, that slot is returned instead.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override V Add(ByteTrio coord)
        {

            int index = IndexOf(coord);

            if (index == -1)
                return null;

            V slot = m_slotList[index];
            if (slot != null)
                return slot;

            slot = Pooling.Pool.Rent<V>();
            slot.m_coordinates = coord;
            m_slotList[index] = slot;
            OnSlotAdded(slot);

            return slot;
        }

        /// <summary>
        /// Remove the slot at the given coordinates and returns it.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override V Remove(ByteTrio coord)
        {
            int index = IndexOf(coord);

            if (index == -1)
                return null;

            V slot = m_slotList[index];
            m_slotList[index] = null;
            return slot;
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
            int index = IndexOf(coord);

            if (index == -1 || index >= m_slotList.Length)
            {
                slot = null;
                return false;
            }

            slot = m_slotList[index];
            return slot == null ? false : true;
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
            int index = IndexOf(new ByteTrio(x, y, z));

            if (index == -1)
            {
                slot = null;
                return false;
            }

            slot = m_slotList[index];
            return slot == null ? false : true;
        }

        /// <summary>
        /// Update all slot positions.
        /// </summary>
        protected override void UpdatePositions()
        {
            V slot;
            for (int i = 0, count = m_slotList.Length; i < count; i++)
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

            //TODO : Implement fill methods accounting for planeOrder.

            int
                sizeX = m_size.x,
                sizeY = m_size.y,
                sizeZ = m_size.z;

            ByteTrio coords;
            float3 o = pos, origin = pos, slotSize = m_slotModel.size;
            int index;
            V slot;

            for (int z = 0; z < sizeZ; z++) // volume
            {
                for (int y = 0; y < sizeY; y++) // plane
                {
                    for (int x = 0; x < sizeX; x++) // line
                    {
                        coords = new ByteTrio(x, y, z);
                        index = x + m_lineLength * y + m_planeLength * z;
                        slot = m_slotList[index];

                        if (slot == null)
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

            if (m_slotList == null)
                return;

            V slot;
            for (int i = 0, count = m_slotList.Length; i < count; i++)
            {
                slot = m_slotList[i];
                if (slot == null)
                    continue;

                m_slotList[i] = null;
                OnSlotRemoved(slot);

                if (release)
                    slot.Release();
            }

            //m_slotList = null;
        }

        #region Nearest vertex in group

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override int GetNearestVertexIndex(IVertex v)
        {
            int index = -1, count = m_slotList.Length;
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
        public override V GetNearestVertex(IVertex v)
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
            int index = -1, count = m_slotList.Length;
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
        public override V GetNearestVertex(float3 v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_slotList[index];
        }



        #endregion

    }
}
