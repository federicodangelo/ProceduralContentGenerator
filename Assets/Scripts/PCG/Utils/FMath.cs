#define USE_OPTIMIZATIONS

using System;

namespace PCG
{
    //Fixed point math auxiliary functions
    public class FMath
    {
        #region Constants

        public static fint PI = fint.CreateRaw(12868); //PI x 2^12
        public static fint TwoPI = fint.CreateRaw(12868 * 2); //radian equivalent of 360 degrees
        public static fint HalfPI = fint.CreateRaw((12868 / 2)); //PI x 2^12 / 2

        public static fint Deg2Rad = PI / fint.CreateFromInt(180); //PI / 180
        public static fint Rad2Deg = fint.CreateFromInt(180) / PI;

        #endregion

        #region Sqrt

        public static fint Sqrt(fint f, int NumberOfIterations)
        {
            if (f.raw < 0) //NaN in Math.Sqrt
                throw new ArithmeticException("Input Error");

            if (f.raw == 0)
                return fint.zero;

#if USE_OPTIMIZATIONS
            long fraw = f.raw;
            long frawshift = (fraw << fint.SHIFT_AMOUNT);
            long k = fraw + fint.one.raw >> 1;

            for (int i = 0; i < NumberOfIterations; i++)
                k = (k + (frawshift / k)) >> 1;

            if (k < 0)
                throw new ArithmeticException("Overflow");

            return fint.CreateRaw((int)k);
#else
			fint k = f + fint.one >> 1;
			for ( int i = 0; i < NumberOfIterations; i++ )
				k = ( k + ( f / k ) ) >> 1;
			
			if ( k.raw < 0 )
				throw new ArithmeticException( "Overflow" );

			return k;
#endif
        }

        public static fint Sqrt(fint f)
        {
            if (f.raw > 0x3e8000)
                return Sqrt(f, 16 * 3 / 4);
            else if (f.raw > 0x64000)
                return Sqrt(f, 12 * 2 / 3);
            else
                return Sqrt(f, 8 * 2 / 3);
        }

        #endregion

        #region Sin

        public static fint Sin(fint i)
        {
            fint j = fint.zero;
            for (; i.raw < 0; i += fint.CreateRaw(25736)) ;
            if (i > fint.CreateRaw(25736))
                i %= fint.CreateRaw(25736);
            fint k = (i * fint.CreateRaw(10)) / fint.CreateRaw(714);
            if (i.raw != 0 && i != fint.CreateRaw(6434) && i != fint.CreateRaw(12868) &&
                i != fint.CreateRaw(19302) && i != fint.CreateRaw(25736))
                j = (i * fint.CreateRaw(100)) / fint.CreateRaw(714) - k * fint.CreateRaw(10);
            if (k <= fint.CreateRaw(90))
                return sin_lookup(k, j);
            if (k <= fint.CreateRaw(180))
                return sin_lookup(fint.CreateRaw(180) - k, j);
            if (k <= fint.CreateRaw(270))
                return -sin_lookup(k - fint.CreateRaw(180), j);
            else
                return -sin_lookup(fint.CreateRaw(360) - k, j);
        }

        private static fint sin_lookup(fint i, fint j)
        {
            if (j.raw > 0 && j < fint.CreateRaw(10) && i < fint.CreateRaw(90))
                return fint.CreateRaw(SIN_TABLE[i.raw]) +
                    ((fint.CreateRaw(SIN_TABLE[i.raw + 1]) - fint.CreateRaw(SIN_TABLE[i.raw])) /
                     fint.CreateRaw(10)) * j;
            else
                return fint.CreateRaw(SIN_TABLE[i.raw]);
        }

        private static int[] SIN_TABLE = {
			0, 71, 142, 214, 285, 357, 428, 499, 570, 641, 
			711, 781, 851, 921, 990, 1060, 1128, 1197, 1265, 1333, 
			1400, 1468, 1534, 1600, 1665, 1730, 1795, 1859, 1922, 1985, 
			2048, 2109, 2170, 2230, 2290, 2349, 2407, 2464, 2521, 2577, 
			2632, 2686, 2740, 2793, 2845, 2896, 2946, 2995, 3043, 3091, 
			3137, 3183, 3227, 3271, 3313, 3355, 3395, 3434, 3473, 3510, 
			3547, 3582, 3616, 3649, 3681, 3712, 3741, 3770, 3797, 3823, 
			3849, 3872, 3895, 3917, 3937, 3956, 3974, 3991, 4006, 4020, 
			4033, 4045, 4056, 4065, 4073, 4080, 4086, 4090, 4093, 4095, 
			4096
		};

        #endregion

        private static fint mul(fint F1, fint F2)
        {
            return F1 * F2;
        }

        #region Cos, Tan, Asin

        public static fint Cos(fint i)
        {
            return Sin(i + fint.CreateRaw(6435));
        }

        public static fint Tan(fint i)
        {
            return Sin(i) / Cos(i);
        }

        public static fint Asin(fint F)
        {
            bool isNegative = F.raw < 0;
            F = Abs(F);

            if (F > fint.one)
                throw new ArithmeticException("Bad Asin Input:" + F.ToFloat());

            fint f1 = mul(mul(mul(mul(
                fint.CreateRaw(145103 >> fint.SHIFT_AMOUNT), F) -
                fint.CreateRaw(599880 >> fint.SHIFT_AMOUNT), F) +
                fint.CreateRaw(1420468 >> fint.SHIFT_AMOUNT), F) -
                fint.CreateRaw(3592413 >> fint.SHIFT_AMOUNT), F) +
                fint.CreateRaw(26353447 >> fint.SHIFT_AMOUNT);

            fint f2 = HalfPI - (Sqrt(fint.one - F) * f1);

            return isNegative ? -f2 : f2;
        }

        public static fint Acos(fint F)
        {
            return HalfPI - Asin(F);
        }

        #endregion

        #region ATan, ATan2

        public static fint Atan(fint F)
        {
            return Asin(F / Sqrt(fint.one + (F * F)));
        }

        public static fint Atan2(fint F1, fint F2)
        {
            if (F2.raw == 0 && F1.raw == 0)
                return fint.zero;

            fint result = fint.zero;
            if (F2 > fint.zero)
            {
                result = Atan(F1 / F2);
            }
            else if (F2 < fint.zero)
            {
                if (F1 >= fint.zero)
                    result = (PI - Atan(Abs(F1 / F2)));
                else
                    result = -(PI - Atan(Abs(F1 / F2)));
            }
            else
                result = (F1 >= fint.zero ? PI : -PI) / fint.CreateFromInt(2);

            return result;
        }

        #endregion

        #region Abs

        public static fint Abs(fint F)
        {
            if (F.raw < 0)
                return -F;
            else
                return F;
        }

        #endregion

        #region Clamp

        public static fint Clamp(fint value, fint min, fint max)
        {
            if (value < min)
            {
                value = min;
            }
            else
            {
                if (value > max)
                {
                    value = max;
                }
            }
            return value;
        }

        public static fint Clamp01(fint value)
        {
            if (value < fint.zero)
            {
                return fint.zero;
            }
            if (value > fint.one)
            {
                return fint.one;
            }
            return value;
        }

        #endregion

        #region Max / Min

        public static fint Max(params fint[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return fint.zero;
            }
            fint num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }
            return num2;
        }

        public static fint Max(fint a, fint b)
        {
            return (a <= b) ? b : a;
        }

        public static fint Min(params fint[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return fint.zero;
            }
            fint num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }
            return num2;
        }

        public static fint Min(fint a, fint b)
        {
            return (a >= b) ? b : a;
        }

        #endregion
    }
}