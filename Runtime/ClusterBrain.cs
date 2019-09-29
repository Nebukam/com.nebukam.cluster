using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace Nebukam.Cluster
{

    /// <summary>
    /// An IClusterBrain is reponsible for handling abstract positioning in a cluster space.
    /// This make it easy to manipulate elements as if they were in a cluster, and makes
    /// job-related computation easy to handle.
    /// </summary>
    public interface IClusterBrain : IVertexInfos
    {

        #region IClusterBrain properties

        /// <summary>
        /// Model used for formatting slots
        /// </summary>
        SlotModel slotModel { set; }

        /// <summary>
        /// Slot size
        /// </summary>
        float3 slotSize { get; set; }

        /// <summary>
        /// Slot size
        /// </summary>
        float3 slotAnchor { get; set; }

        /// <summary>
        /// Slot size
        /// </summary>
        float3 slotOffset { get; }

        /// <summary>
        /// Cluster's slot size
        /// </summary>
        ByteTrio clusterSize { get; set; }

        /// <summary>
        /// Cluster's bounds
        /// </summary>
        Bounds bounds { get; }

        /// <summary>
        /// Clusters max capacity
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Wrap mode over X axis
        /// </summary>
        WrapMode wrapX { get; set; }

        /// <summary>
        /// Wrap mode over Y axis
        /// </summary>
        WrapMode wrapY { get; set; }

        /// <summary>
        /// Wrap mode over Z axis
        /// </summary>
        WrapMode wrapZ { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cluster"></param>
        void ExtractProperties(ISlotCluster cluster);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brain"></param>
        /// 
        void ExtractProperties(IClusterBrain brain);

        #endregion

        /// <summary>
        /// Retrieve the coordinates that contain the given location,
        /// based on cluster location & slot's size
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool TryGetCoordOf(float3 location, out ByteTrio coord);

        /// <summary>
        /// Return the world-space projection of the given coordinates, as projected by this cluster.
        /// </summary>
        /// <param name="coords"></param>
        float3 ComputePosition(ref ByteTrio coords);

    }

    public partial struct ClusterBrain : IClusterBrain
    {

        #region IVertexInfos

        public float3 m_pos;

        public float3 pos
        {
            get { return m_pos; }
            set
            {
                m_pos = value;
                UpdateBounds();
            }
        }

        #endregion

        #region IClusterBrain properties

        public WrapMode
            m_wrapX,
            m_wrapY,
            m_wrapZ;

        public ByteTrio m_clusterSize;

        public float3
            m_slotSize,
            m_slotAnchor,
            m_slotOffset;

        public Bounds m_bounds;

        public float3 slotSize
        {
            get { return m_slotSize; }
            set
            {
                m_slotSize = value;
                m_slotOffset = m_slotSize * m_slotAnchor;
                UpdateBounds();
            }
        }

        public float3 slotAnchor
        {
            get { return m_slotAnchor; }
            set
            {
                m_slotAnchor = value;
                m_slotOffset = m_slotSize * m_slotAnchor;
            }
        }

        public float3 slotOffset
        {
            get { return m_slotOffset; }
        }

        public ByteTrio clusterSize
        {
            get { return m_clusterSize; }
            set
            {
                m_clusterSize = value;
                UpdateBounds();
            }
        }

        public int Capacity
        {
            get { return m_clusterSize.Volume(); }
        }

        public Bounds bounds
        {
            get { return m_bounds; }
            set { m_bounds = value; }
        }

        public WrapMode wrapX
        {
            get { return m_wrapX; }
            set { m_wrapX = value; }
        }

        public WrapMode wrapY
        {
            get { return m_wrapY; }
            set { m_wrapY = value; }
        }

        public WrapMode wrapZ
        {
            get { return m_wrapZ; }
            set { m_wrapZ = value; }
        }

        public SlotModel slotModel
        {
            set
            {
                m_slotSize = value.m_slotSize;
                m_slotAnchor = value.m_anchor;
                m_slotOffset = value.m_offset;
                UpdateBounds();
            }
        }

        private void UpdateBounds()
        {
            float3 s = m_clusterSize * m_slotSize;
            m_bounds = new Bounds(m_pos + s * 0.5f, s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cluster"></param>
        public void ExtractProperties(ISlotCluster cluster)
        {

            m_pos = cluster.pos;

            m_wrapX = cluster.wrapX;
            m_wrapY = cluster.wrapY;
            m_wrapZ = cluster.wrapZ;

            m_clusterSize = cluster.size;
            
            slotModel = cluster.slotModel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brain"></param>
        public void ExtractProperties(IClusterBrain brain)
        {

            m_pos = brain.pos;

            m_wrapX = brain.wrapX;
            m_wrapY = brain.wrapY;
            m_wrapZ = brain.wrapZ;

            m_clusterSize = brain.clusterSize;

            m_slotSize = brain.slotSize;
            m_slotOffset = brain.slotOffset;
            m_slotAnchor = brain.slotAnchor;

            UpdateBounds();

        }

        #endregion

        #region ClusterBrain methods

        /// <summary>
        /// Retrieve the coordinates that contain the given location,
        /// based on cluster location & slot's size
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool TryGetCoordOf(float3 location, out ByteTrio coord)
        {

            if (!bounds.Contains(location))
            {
                coord = ByteTrio.zero;
                return false;
            }

            float3
                loc = location - pos;

            float
                modX = loc.x % m_slotSize.x,
                modY = loc.y % m_slotSize.y,
                modZ = loc.z % m_slotSize.z;

            int
                posX = (int)((loc.x - modX) / m_slotSize.x),
                posY = (int)((loc.y - modY) / m_slotSize.y),
                posZ = (int)((loc.z - modZ) / m_slotSize.z);

            coord = new ByteTrio(posX, posY, posZ);
            coord.Clamp(ref m_clusterSize, ref m_wrapX, ref m_wrapY, ref m_wrapZ);
            return true;
        }

        /// <summary>
        /// Return the world-space projection of the given coordinates, as projected by this cluster.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="position"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3 ComputePosition(ref ByteTrio coords)
        {
            return m_pos + coords * m_slotSize + m_slotOffset;
        }

        #endregion

    }

}
