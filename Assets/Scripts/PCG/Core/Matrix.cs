using System;

namespace PCG
{
    public class Matrix
    {
        public int size;
        public byte[] values;

        public Matrix(int size)
        {
            this.size = size;
            this.values = new byte[size * size];
        }

        public void Fill(byte value)
        {
            for (int i = 0; i < values.Length; i++)
                values [i] = value;
        }

        public byte GetValue(int x, int y)
        {
            return values[x + y * size];
        }

        public void SetValue(int x, int y, byte value)
        {
            values [x + y * size] = value;
        }

        public override string ToString()
        {
            return "Matrix " + size + "x" + size;
        }
    }
}

