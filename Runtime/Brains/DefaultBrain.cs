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

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Nebukam.Cluster
{

    [BurstCompile]
    public struct DefaultBrain : IClusterBrain
    {

        #region Brain core

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
                m_slotSize = value.size;
                m_slotAnchor = value.anchor;
                m_slotOffset = value.offset;
                UpdateBounds();
            }
        }

        private void UpdateBounds()
        {
            float3 s = m_clusterSize * m_slotSize;
            m_bounds = new Bounds(m_pos + s * 0.5f, s);
        }

        #endregion

        #region Wrapping

        public void Clamp(ref ByteTrio coord)
        {
            if (m_wrapX != WrapMode.NONE)
            {
                if (coord.x < 0)
                    coord.x = (byte)(m_wrapX == WrapMode.LOOP ? m_clusterSize.x + (coord.x % m_clusterSize.x) : 0);
                else if (coord.x >= m_clusterSize.x)
                    coord.x = (byte)(m_wrapX == WrapMode.LOOP ? coord.x % m_clusterSize.x : m_clusterSize.x - 1);
            }

            if (m_wrapY != WrapMode.NONE)
            {
                if (coord.y < 0)
                    coord.y = (byte)(m_wrapY == WrapMode.LOOP ? m_clusterSize.y + (coord.y % m_clusterSize.y) : 0);
                else if (coord.y >= m_clusterSize.y)
                    coord.y = (byte)(m_wrapY == WrapMode.LOOP ? coord.y % m_clusterSize.y : m_clusterSize.y - 1);
            }

            if (m_wrapZ != WrapMode.NONE)
            {
                if (coord.z < 0)
                    coord.z = (byte)(m_wrapZ == WrapMode.LOOP ? m_clusterSize.z + (coord.z % m_clusterSize.z) : 0);
                else if (coord.z >= m_clusterSize.y)
                    coord.z = (byte)(m_wrapZ == WrapMode.LOOP ? coord.z % m_clusterSize.z : m_clusterSize.z - 1);
            }
        }

        public ByteTrio Clamp(ByteTrio coord)
        {
            if (m_wrapX != WrapMode.NONE)
            {
                if (coord.x < 0)
                    coord.x = (byte)(m_wrapX == WrapMode.LOOP ? m_clusterSize.x + (coord.x % m_clusterSize.x) : 0);
                else if (coord.x >= m_clusterSize.x)
                    coord.x = (byte)(m_wrapX == WrapMode.LOOP ? coord.x % m_clusterSize.x : m_clusterSize.x - 1);
            }

            if (m_wrapY != WrapMode.NONE)
            {
                if (coord.y < 0)
                    coord.y = (byte)(m_wrapY == WrapMode.LOOP ? m_clusterSize.y + (coord.y % m_clusterSize.y) : 0);
                else if (coord.y >= m_clusterSize.y)
                    coord.y = (byte)(m_wrapY == WrapMode.LOOP ? coord.y % m_clusterSize.y : m_clusterSize.y - 1);
            }

            if (m_wrapZ != WrapMode.NONE)
            {
                if (coord.z < 0)
                    coord.z = (byte)(m_wrapZ == WrapMode.LOOP ? m_clusterSize.z + (coord.z % m_clusterSize.z) : 0);
                else if (coord.z >= m_clusterSize.y)
                    coord.z = (byte)(m_wrapZ == WrapMode.LOOP ? coord.z % m_clusterSize.z : m_clusterSize.z - 1);
            }

            return coord;
        }

        public ByteTrio Clamp(int x, int y, int z)
        {

            if (m_wrapX != WrapMode.NONE)
            {
                if (x < 0)
                    x = (byte)(m_wrapX == WrapMode.LOOP ? m_clusterSize.x + (x % m_clusterSize.x) : 0);
                else if (x >= m_clusterSize.x)
                    x = (byte)(m_wrapX == WrapMode.LOOP ? x % m_clusterSize.x : m_clusterSize.x - 1);
            }

            if (m_wrapY != WrapMode.NONE)
            {
                if (y < 0)
                    y = (byte)(m_wrapY == WrapMode.LOOP ? m_clusterSize.y + (y % m_clusterSize.y) : 0);
                else if (y >= m_clusterSize.y)
                    y = (byte)(m_wrapY == WrapMode.LOOP ? y % m_clusterSize.y : m_clusterSize.y - 1);
            }

            if (m_wrapZ != WrapMode.NONE)
            {
                if (z < 0)
                    z = (byte)(m_wrapZ == WrapMode.LOOP ? m_clusterSize.z + (z % m_clusterSize.z) : 0);
                else if (z >= m_clusterSize.y)
                    z = (byte)(m_wrapZ == WrapMode.LOOP ? z % m_clusterSize.z : m_clusterSize.z - 1);
            }

            return new ByteTrio(x, y, z);
        }

        public void Clamp(ref int x, ref int y, ref int z)
        {

            if (m_wrapX != WrapMode.NONE)
            {
                if (x < 0)
                    x = m_wrapX == WrapMode.LOOP ? m_clusterSize.x + (x % m_clusterSize.x) : 0;
                else if (x >= m_clusterSize.x)
                    x = m_wrapX == WrapMode.LOOP ? x % m_clusterSize.x : m_clusterSize.x - 1;
            }

            if (m_wrapY != WrapMode.NONE)
            {
                if (y < 0)
                    y = m_wrapY == WrapMode.LOOP ? m_clusterSize.y + (y % m_clusterSize.y) : 0;
                else if (y >= m_clusterSize.y)
                    y = m_wrapY == WrapMode.LOOP ? y % m_clusterSize.y : m_clusterSize.y - 1;
            }

            if (m_wrapZ != WrapMode.NONE)
            {
                if (z < 0)
                    z = m_wrapZ == WrapMode.LOOP ? m_clusterSize.z + (z % m_clusterSize.z) : 0;
                else if (z >= m_clusterSize.y)
                    z = m_wrapZ == WrapMode.LOOP ? z % m_clusterSize.z : m_clusterSize.z - 1;
            }

        }

        public void Clamp(ref int3 coord)
        {

            if (m_wrapX != WrapMode.NONE)
            {
                if (coord.x < 0)
                    coord.x = m_wrapX == WrapMode.LOOP ? m_clusterSize.x + (coord.x % m_clusterSize.x) : 0;
                else if (coord.x >= m_clusterSize.x)
                    coord.x = m_wrapX == WrapMode.LOOP ? coord.x % m_clusterSize.x : m_clusterSize.x - 1;
            }

            if (m_wrapY != WrapMode.NONE)
            {
                if (coord.y < 0)
                    coord.y = m_wrapY == WrapMode.LOOP ? m_clusterSize.y + (coord.y % m_clusterSize.y) : 0;
                else if (coord.y >= m_clusterSize.y)
                    coord.y = m_wrapY == WrapMode.LOOP ? coord.y % m_clusterSize.y : m_clusterSize.y - 1;
            }

            if (m_wrapZ != WrapMode.NONE)
            {
                if (coord.z < 0)
                    coord.z = m_wrapZ == WrapMode.LOOP ? m_clusterSize.z + (coord.z % m_clusterSize.z) : 0;
                else if (coord.z >= m_clusterSize.y)
                    coord.z = m_wrapZ == WrapMode.LOOP ? coord.z % m_clusterSize.z : m_clusterSize.z - 1;
            }

        }

        public bool Contains(ref ByteTrio coord)
        {
            if (m_wrapX == WrapMode.NONE && (coord.x < 0 || coord.x >= m_clusterSize.x))
                return false;
            else if (m_wrapY == WrapMode.NONE && (coord.y < 0 || coord.y >= m_clusterSize.y))
                return false;
            else if (m_wrapZ == WrapMode.NONE && (coord.z < 0 || coord.z >= m_clusterSize.z))
                return false;

            return true;
        }

        public bool Contains(ref float3 position)
        {
            return m_bounds.Contains(position);
        }

        #endregion

        #endregion

        ///
        /// Base logic
        ///

        /// <summary>
        /// Retrieve the coordinates that contain the given location,
        /// based on cluster location & slot's size
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [BurstCompile]
        public bool TryGetCoordOf(float3 location, out ByteTrio coord)
        {

            coord = ByteTrio.zero;

            if (!bounds.Contains(location))
                return false;

            float3
                loc = location - pos;

            float
                lx = location.x - pos.x,
                ly = location.y - pos.y,
                lz = location.z - pos.z,
                ssx = m_slotSize.x,
                ssy = m_slotSize.y,
                ssz = m_slotSize.z;

            coord = Clamp(
                (int)((lx - (lx % ssx)) / ssx),
                (int)((ly - (ly % ssy)) / ssy),
                (int)((lz - (lz % ssz)) / ssz));

            return true;
        }

        /// <summary>
        /// Return the world-space projection of the given coordinates, as projected by this cluster.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="position"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public float3 ComputePosition(ref ByteTrio coords)
        {
            return m_pos + coords * m_slotSize + m_slotOffset;
        }

    }

}
