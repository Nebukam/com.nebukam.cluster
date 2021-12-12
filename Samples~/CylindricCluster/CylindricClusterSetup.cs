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
#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

using Nebukam;
using Nebukam.Cluster;
using Nebukam.Common.Editor;

public class CylindricClusterSetup : MonoBehaviour
{

    public int3 size = int3(1, 1, 1);
    public float debugRadius = 0.01f;
    public float3 slotSize = float3(1f);
    public float3 slotAnchor = float3(0.5f);

    public Transform positionTester;

    protected SlotCluster<Slot, CylinderBrain> m_cluster;
    protected SlotModel m_model = new SlotModel();

    private void Awake()
    {
        m_model.size = slotSize;
        m_model.anchor = slotAnchor;

        m_cluster = Pool.Rent<SlotClusterFixed<Slot, CylinderBrain>>();
        m_cluster.Init(m_model, size, true);
    }

    private void Update()
    {
        ISlot slot;
        //debugRadius = length(slotSize) * 0.5f;

        for (int i = 0, count = m_cluster.Count; i < count; i++)
        {
            slot = m_cluster[i];
            Draw.Cube(slot.pos, debugRadius, Color.red);
        }

        if (m_cluster.TryGet(positionTester.position, out slot))
        {
            Draw.Cube(slot.pos, debugRadius + 0.1f, Color.green);
        }
    }

    private void OnDrawGizmos()
    {
        if (m_cluster == null)
            return;

        Bounds b = m_cluster.bounds;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}
#endif

