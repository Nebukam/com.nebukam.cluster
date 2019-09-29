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

using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Nebukam.Cluster
{

    public interface ISlotCluster : IVertex
    {
        
        /// <summary>
        /// Model used for formatting slots
        /// </summary>
        SlotModel slotModel { get; set; }

        /// <summary>
        /// Cluster's slot size
        /// </summary>
        ByteTrio size { get; set; }

        /// <summary>
        /// Cluster's bounds
        /// </summary>
        Bounds bounds { get; }

        /// <summary>
        /// Clusters finite capacity
        /// </summary>
        int Capacity { get; }
        
        /// <summary>
        /// Return the slot index of the given coordinates
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        int IndexOf(ByteTrio coord);

        /// <summary>
        /// Retrieve the coordinates that contain the given location,
        /// based on cluster location & slot's size
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool TryGetCoordOf(float3 location, out ByteTrio coord);
        
        /// <summary>
        /// Gets the slot associated with the specified coordinates.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <remarks>Input coordinates are wrapped per-axis using each of the cluster's individual wrap mode.</remarks>
        bool TryGet(ByteTrio coord, out ISlot slot);

        /// <summary>
        /// Gets the slot associated with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <remarks>Input coordinates are wrapped per-axis using each of the cluster's individual wrap mode.</remarks>
        bool TryGet(int x, int y, int z, out ISlot slot);

        /// <summary>
        /// Gets the slot containing the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        bool TryGet(float3 location, out ISlot slot);

        /// <summary>
        /// Return the world-space projection of the given coordinates, as projected by this cluster.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        float3 ComputePosition(ref ByteTrio coords);

    }

    public interface ISlotCluster<out V> : IClearableVertexGroup<V>, ISlotCluster
        where V : ISlot
    {

        /// <summary>
        /// Set the slot occupation at a given coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <param name="releaseExisting"></param>
        /// <returns></returns>
        V Set(ByteTrio coord, ISlot slot, bool releaseExisting = false);

        /// <summary>
        /// Create a slot at the given coordinates and return it.
        /// If a slot already exists at the given location, that slot is returned instead.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        V Add(ByteTrio coord);

        /// <summary>
        /// Remove the slot at the given coordinates and returns it.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        V Remove(ByteTrio coord);

        /// <summary>
        /// Fills all empty slots in this cluster with.
        /// </summary>
        void Fill();

    }

    public interface ISlotCluster<out V, B> : ISlotCluster<V>
        where V : ISlot
        where B : struct, IClusterBrain
    {

        /// <summary>
        /// A Brain image of this SlotCluster
        /// </summary>
        B brain { get; set; }

        void Init(SlotModel clusterSlotModel, B clusterBrain, bool fillCluster);
        void Init(SlotModel clusterSlotModel, ByteTrio clusterSize, bool fillCluster);
        void Init(SlotModel clusterSlotModel, ByteTrio clusterSize, WrapMode wrapX, WrapMode wrapY, WrapMode wrapZ, bool fillCluster);

    }

    /// <summary>
    /// A GridChunk represent a 3D abstract grid with finite slot capacity.
    /// Each slot is stored at a given ByteTrio (x, y, z) location
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public abstract class SlotCluster<V, B> : Vertex, ISlotCluster<V, B>, Pooling.IRequireCleanUp
        where V : Slot, ISlot, new()
        where B : struct, IClusterBrain
    {

        protected internal B m_brain = default;
        protected internal SlotModel m_slotModel = null;
        protected internal ByteTrio m_size = ByteTrio.zero;

        /// <summary>
        /// Model used for formatting slots
        /// </summary>
        public SlotModel slotModel
        {
            get { return m_slotModel; }
            set
            {
                m_slotModel = value;
                m_brain.slotModel = value;
                if (m_slotModel != null)
                {
                    OnModelChanged();
                }
            }
        }

        public B brain
        {
            get { return m_brain; }
            set { SetBrain( value ); }
        }

        protected virtual void SetBrain(B value)
        {
            m_brain = value;

            m_brain.pos = m_pos;
            m_brain.slotModel = m_slotModel;

            size = m_brain.clusterSize;
        }

        /// <summary>
        /// Return size difference on each individual X, Y & Z axis, compared
        /// to the previous size before it was updated
        /// </summary>
        /// <returns></returns>
        protected virtual void OnModelChanged()
        {
            UpdatePositions();
        }

        /// <summary>
        /// Cluster's slot size
        /// </summary>
        public ByteTrio size
        {
            get { return m_size; }
            set
            {
                if (m_size == value) { return; }
                ByteTrio oldSize = m_size;
                m_size = value;
                m_brain.clusterSize = value;
                OnSizeChanged(oldSize);
            }
        }

        protected virtual int3 OnSizeChanged(ByteTrio oldSize)
        {
            return int3(
                m_size.x - oldSize.x,
                m_size.y - oldSize.y,
                m_size.z - oldSize.z);
        }

        public virtual Bounds bounds
        {
            get { return m_brain.bounds; }
        }

        /// <summary>
        /// Cluster;s finite capacity
        /// </summary>
        public int Capacity { get { return m_size.Volume(); } }

        /// <summary>
        /// Number of slots in the cluster
        /// </summary>
        public abstract int Count { get; }
        public abstract V this[int index] { get; }
        public abstract int this[IVertex v] { get; }
        
        /// <summary>
        /// Return the slot index of the given coordinates
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public abstract int IndexOf(ByteTrio coord);

        /// <summary>
        /// Retrieve the coordinates that contain the given location,
        /// based on cluster location & slot's size
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual bool TryGetCoordOf(float3 location, out ByteTrio coord)
        {
            return m_brain.TryGetCoordOf(location, out coord);
        }

        public virtual void Init(
            SlotModel clusterSlotModel, 
            B clusterBrain, 
            bool fillCluster)
        {
            Clear(true);

            m_slotModel = clusterSlotModel;

            SetBrain( clusterBrain );

            if (fillCluster)
                Fill();
        }

        public virtual void Init(
            SlotModel clusterSlotModel, 
            ByteTrio clusterSize, 
            bool fillCluster)
        {
            Clear(true);

            m_slotModel = clusterSlotModel;

            m_brain.clusterSize = clusterSize;
            SetBrain(m_brain);

            if (fillCluster)
                Fill();
        }

        public virtual void Init(
            SlotModel clusterSlotModel, 
            ByteTrio clusterSize, 
            WrapMode wrapX, 
            WrapMode wrapY, 
            WrapMode wrapZ, 
            bool fillCluster)
        {
            Clear(true);

            m_slotModel = clusterSlotModel;

            B b = default;
            b.wrapX = wrapX;
            b.wrapY = wrapY;
            b.wrapZ = wrapZ;
            b.clusterSize = clusterSize;

            SetBrain(m_brain);

            if (fillCluster)
                Fill();
        }


        /// <summary>
        /// Set the slot occupation at a given coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <param name="releaseExisting"></param>
        /// <returns></returns>
        public abstract V Set(ByteTrio coord, ISlot slot, bool releaseExisting = false);

        /// <summary>
        /// Create a slot at the given coordinates and return it.
        /// If a slot already exists at the given location, that slot is returned instead.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public abstract V Add(ByteTrio coord);

        /// <summary>
        /// Remove the slot at the given coordinates and returns it.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public abstract V Remove(ByteTrio coord);

        /// <summary>
        /// Gets the slot associated with the specified coordinates.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <remarks>Input coordinates are wrapped per-axis using each of the cluster's individual wrap mode.</remarks>
        public abstract bool TryGet(ByteTrio coord, out ISlot slot);

        /// <summary>
        /// Gets the slot associated with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <remarks>Input coordinates are wrapped per-axis using each of the cluster's individual wrap mode.</remarks>
        public abstract bool TryGet(int x, int y, int z, out ISlot slot);

        /// <summary>
        /// Gets the slot containing the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool TryGet(float3 location, out ISlot slot)
        {

            if (m_brain.TryGetCoordOf(location, out ByteTrio coord) && TryGet(coord, out slot))
                return true;

            slot = null;
            return false;

        }

        /// <summary>
        /// Return the world-space projection of the given coordinates, as projected by this cluster.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="position"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3 ComputePosition(ref ByteTrio coords)
        {
            return m_brain.ComputePosition(ref coords);
        }

        /// <summary>
        /// Callback when a slot is added to the cluster.
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void OnSlotAdded(V slot)
        {
            slot.cluster = this;
            slot.pos = m_brain.ComputePosition(ref slot.m_coordinates);
        }

        /// <summary>
        /// Callback when a slot is removed from the cluster.
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void OnSlotRemoved(V slot)
        {
            if (slot.cluster == this)
                slot.cluster = null;
        }

        /// <summary>
        /// Update all slot positions.
        /// </summary>
        protected abstract void UpdatePositions();

        /// <summary>
        /// Fills all empty slots in this cluster with.
        /// </summary>
        public abstract void Fill();

        /// <summary>
        /// Clear cluster & releases all slots.
        /// </summary>
        public abstract void Clear(bool release = false);

        #region PoolItem

        public virtual void CleanUp()
        {
            Clear();
            m_slotModel = null;
            m_size = int3(0);
        }

        #endregion

        #region Nearest vertex in group

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public abstract int GetNearestVertexIndex(IVertex v);

        /// <summary>
        /// Return the the nearest IVertex in group to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public abstract V GetNearestVertex(IVertex v);

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public abstract int GetNearestVertexIndex(float3 v);

        /// <summary>
        /// Return the nearest IVertex in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public abstract V GetNearestVertex(float3 v);



        #endregion

    }
}
