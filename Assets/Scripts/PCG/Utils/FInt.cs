#define USE_INT

using System;

namespace PCG
{
    //Fixed point math library taken from:
    //http://stackoverflow.com/questions/605124/fixed-point-math-in-c
    //Removed some explicit conversion and operations that allowed "int" parameters instead of fint (better control over involuntary conversions)
    public struct fint
    {
#if USE_INT
        public int raw;
#else
		public long raw;
#endif

        public const int SHIFT_AMOUNT = 12; //12 is 4096

        private const long rawOne = 1 << SHIFT_AMOUNT;

        public static fint zero = fint.CreateFromInt(0);

        public static fint one = fint.CreateFromInt(1);
        public static fint two = fint.CreateFromInt(2);

        public static fint half = fint.CreateFromInt(1) / fint.CreateFromInt(2);
        public static fint quarter = fint.CreateFromInt(1) / fint.CreateFromInt(4);

        public const int decimalPartMask = (1 << SHIFT_AMOUNT) - 1;

        #region Constructors

#if USE_INT
        public static fint CreateRaw(int StartingRawValue)
#else
		public static fint CreateRaw(long StartingRawValue)
#endif
        {
            fint fInt;
            fInt.raw = StartingRawValue;
            return fInt;
        }

        public static fint CreateFromInt(int IntValue)
        {
            fint fInt;
            fInt.raw = IntValue;
            fInt.raw = fInt.raw << SHIFT_AMOUNT;
            return fInt;
        }

        public static fint CreateFromFloat(float FloatValue)
        {
            fint fInt;
            FloatValue *= (float)rawOne;
            fInt.raw = (int)UnityEngine.Mathf.Round(FloatValue);
            return fInt;
        }

        #endregion

        public int ToInt()
        {
            return (int)(this.raw >> SHIFT_AMOUNT);
        }

        public float ToFloat()
        {
            return (float)this.raw / (float)rawOne;
        }

        #region FromParts
        /// <summary>
        /// Create a fixed-int number from parts.  For example, to create 1.5 pass in 1 and 500.
        /// </summary>
        /// <param name="PreDecimal">The number above the decimal.  For 1.5, this would be 1.</param>
        /// <param name="PostDecimal">The number below the decimal, to three digits.  
        /// For 1.5, this would be 500. For 1.005, this would be 5.</param>
        /// <returns>A fixed-int representation of the number parts</returns>
        public static fint FromParts(int PreDecimal, int PostDecimal)
        {
            fint f = fint.CreateFromInt(PreDecimal);
            if (PostDecimal != 0)
                f += (fint.CreateFromInt(PostDecimal) / fint.CreateFromInt(1000));
            return f;
        }
        #endregion

        #region *
        public static fint operator *(fint one, fint other)
        {
            fint fInt;
#if USE_INT
            fInt.raw = (int)(((long)one.raw * (long)other.raw) >> SHIFT_AMOUNT);
#else
			fInt.raw = ( one.raw * other.raw ) >> SHIFT_AMOUNT;
#endif
            return fInt;
        }
        #endregion

        #region /
        public static fint operator /(fint one, fint other)
        {
            fint fInt;
#if USE_INT
            fInt.raw = (int)((((long)one.raw) << SHIFT_AMOUNT) / ((long)other.raw));
#else
			fInt.raw = ( one.raw << SHIFT_AMOUNT ) / ( other.raw );
#endif
            return fInt;
        }
        #endregion

        #region %
        public static fint operator %(fint one, fint divisor)
        {
            fint fInt;
            fInt.raw = (one.raw) % (divisor.raw);
            return fInt;
        }
        #endregion

        #region +
        public static fint operator +(fint one, fint other)
        {
            fint fInt;
            fInt.raw = one.raw + other.raw;
            return fInt;
        }
        #endregion

        #region -
        public static fint operator -(fint one, fint other)
        {
            fint fInt;
            fInt.raw = one.raw - other.raw;
            return fInt;
        }
        #endregion

        #region ==
        public static bool operator ==(fint one, fint other)
        {
            return one.raw == other.raw;
        }
        #endregion

        #region !=
        public static bool operator !=(fint one, fint other)
        {
            return one.raw != other.raw;
        }
        #endregion

        #region >=
        public static bool operator >=(fint one, fint other)
        {
            return one.raw >= other.raw;
        }
        #endregion

        #region <=
        public static bool operator <=(fint one, fint other)
        {
            return one.raw <= other.raw;
        }
        #endregion

        #region >
        public static bool operator >(fint one, fint other)
        {
            return one.raw > other.raw;
        }
        #endregion

        #region <
        public static bool operator <(fint one, fint other)
        {
            return one.raw < other.raw;
        }
        #endregion

        public static fint operator <<(fint one, int Amount)
        {
            return fint.CreateRaw(one.raw << Amount);
        }

        public static fint operator >>(fint one, int Amount)
        {
            return fint.CreateRaw(one.raw >> Amount);
        }

        public static fint operator -(fint a)
        {
            a.raw = -a.raw;

            return a;
        }


        public override bool Equals(object obj)
        {
            if (obj is fint)
                return ((fint)obj).raw == this.raw;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return raw.GetHashCode();
        }

        public override string ToString()
        {
            return this.raw.ToString();
        }
    }
}