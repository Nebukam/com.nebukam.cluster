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
using UnityEngine;

namespace Nebukam.Cluster
{
    public class SlotModelDrawer : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool draw = true;
        public SlotModelData model;
        public Color color = Color.white;
        public bool wire = false;

        private void OnDrawGizmos()
        {
            SlotModel m = model?.model;

            if (!draw || m == null)
                return;

            Gizmos.color = color;

            if (wire)
                Gizmos.DrawWireCube(m.m_offset + (float3)transform.position, m.size);
            else
                Gizmos.DrawCube(m.m_offset + (float3)transform.position, m.size);

        }
#endif
    }
}
