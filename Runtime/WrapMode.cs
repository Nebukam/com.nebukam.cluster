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

namespace Nebukam.Cluster
{
    public enum WrapMode
    {
        NONE = 0,
        LOOP = 1,
        REPEAT = 2
    }

    public static class WrapExtensions
    {

        public static BytePair Clamp(ref this BytePair @this, ref BytePair bounds, ref WrapMode wrapX, ref WrapMode wrapY)
        {
            if (wrapX != WrapMode.NONE)
            {
                if (@this.x < 0)
                    @this.x = (byte)(wrapX == WrapMode.LOOP ? bounds.x + (@this.x % bounds.x) : 0);
                else if (@this.x >= bounds.x)
                    @this.x = (byte)(wrapX == WrapMode.LOOP ? @this.x % bounds.x : bounds.x - 1);
            }

            if (wrapY != WrapMode.NONE)
            {
                if (@this.y < 0)
                    @this.y = (byte)(wrapY == WrapMode.LOOP ? bounds.y + (@this.y % bounds.y) : 0);
                else if (@this.y >= bounds.y)
                    @this.y = (byte)(wrapY == WrapMode.LOOP ? @this.y % bounds.y : bounds.y - 1);
            }

            return @this;
        }

        public static ByteTrio Clamp(ref this ByteTrio @this, ref ByteTrio bounds, ref WrapMode wrapX, ref WrapMode wrapY, ref WrapMode wrapZ)
        {
            if (wrapX != WrapMode.NONE)
            {
                if (@this.x < 0)
                    @this.x = (byte)(wrapX == WrapMode.LOOP ? bounds.x + (@this.x % bounds.x) : 0);
                else if (@this.x >= bounds.x)
                    @this.x = (byte)(wrapX == WrapMode.LOOP ? @this.x % bounds.x : bounds.x - 1);
            }

            if (wrapY != WrapMode.NONE)
            {
                if (@this.y < 0)
                    @this.y = (byte)(wrapY == WrapMode.LOOP ? bounds.y + (@this.y % bounds.y) : 0);
                else if (@this.y >= bounds.y)
                    @this.y = (byte)(wrapY == WrapMode.LOOP ? @this.y % bounds.y : bounds.y - 1);
            }

            if (wrapZ != WrapMode.NONE)
            {
                if (@this.z < 0)
                    @this.z = (byte)(wrapZ == WrapMode.LOOP ? bounds.z + (@this.z % bounds.z) : 0);
                else if (@this.z >= bounds.y)
                    @this.z = (byte)(wrapZ == WrapMode.LOOP ? @this.z % bounds.z : bounds.z - 1);
            }

            return @this;
        }

    }

}
