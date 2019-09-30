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

namespace Nebukam.Cluster
{

    public interface ISlotInfos<T_SLOT> : IVertexInfos
        where T_SLOT : ISlot
    {
        int index { get; set; }
        ByteTrio coord { get; set; }
        void Capture(T_SLOT slot);
    }

    public struct SlotInfos : ISlotInfos<ISlot>
    {

        #region IVertexInfos

        public float3 m_pos;

        public float3 pos
        {
            get { return m_pos; }
            set { m_pos = value; }
        }

        #endregion

        #region ISlotInfos

        public int i;
        public ByteTrio c;

        public int index
        {
            get { return i; }
            set { i = value; }
        }

        public ByteTrio coord
        {
            get { return c; }
            set { c = value; }
        }

        public void Capture(ISlot slot)
        {

        }

        #endregion

    }

}
