using System.Collections;

//Original from: http://scrawkblog.com/2013/03/05/perlin-noise-pulgin-for-unity/

//Usage:

//- Instantiate perlin noise generator with seed
//  var perlin : PerlinNoise = new PerlinNoise(0);
//- Then evaluate in 1D / 2D / 3D
//  var noise1D : fint = perlin.FractalNoise1D(x, octaves, frq, amp);
//  var noise2D : fint = perlin.FractalNoise2D(x, y,  octaves, frq, amp);
//  var noise3D : fint = perlin.FractalNoise3D(x, y, z, octaves, frq, amp);

namespace PCG
{
    public class PerlinNoiseFixed
    {
        const int B = 256;
        int[] m_perm = new int[B + B];

        public PerlinNoiseFixed(int seed)
        {
            RandomGeneratorXorShift rnd = new RandomGeneratorXorShift(seed);

            int i, j, k;
            for (i = 0; i < B; i++)
            {
                m_perm[i] = i;
            }

            while (--i != 0)
            {
                k = m_perm[i];
                j = rnd.Range(0, B);
                m_perm[i] = m_perm[j];
                m_perm[j] = k;
            }

            for (i = 0; i < B; i++)
            {
                m_perm[B + i] = m_perm[i];
            }

        }

        static fint FADE(fint t) 
        { 
            return t * t * t * (t * (t * fint.CreateFromInt(6) - fint.CreateFromInt(15)) + fint.CreateFromInt(10)); 
        }

        static fint LERP(fint t, fint a, fint b) 
        { 
            return (a) + (t) * ((b) - (a)); 
        }

        /*
        fint GRAD1(int hash, fint x)
        {
            //This method uses the mod operator which is slower 
            //than bitwise operations but is included out of interest
            //		int h = hash % 16;
            //		fint grad = 1.0f + (h % 8);
            //		if((h%8) < 4) grad = -grad;
            //		return ( grad * x );

            int h = hash & 15;
            fint grad = 1.0f + (h & 7);
            if ((h & 8) != 0) grad = -grad;
            return (grad * x);
        }
        */

        fint GRAD2(int hash, fint x, fint y)
        {
            //This method uses the mod operator which is slower 
            //than bitwise operations but is included out of interest
            //		int h = hash % 16;
            //    	fint u = h<4 ? x : y;
            //    	fint v = h<4 ? y : x;
            //		int hn = h%2;
            //		int hm = (h/2)%2;
            //    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -2.0f*v : 2.0f*v);

            int h = hash & 7;
            fint u = h < 4 ? x : y;
            fint v = h < 4 ? y : x;
            return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -fint.two * v : fint.two * v);
        }

        /*
        fint GRAD3(int hash, fint x, fint y, fint z)
        {
            //This method uses the mod operator which is slower 
            //than bitwise operations but is included out of interest
            //    	int h = hash % 16;
            //    	fint u = (h<8) ? x : y;
            //    	fint v = (h<4) ? y : (h==12||h==14) ? x : z;
            //		int hn = h%2;
            //		int hm = (h/2)%2;
            //    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -v : v);

            int h = hash & 15;
            fint u = h < 8 ? x : y;
            fint v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
            return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -v : v);
        }
        */

        /*
        fint Noise1D(fint x)
        {
            //returns a noise value between -0.5 and 0.5
            int ix0, ix1;
            fint fx0, fx1;
            fint s, n0, n1;

            ix0 = (int)Mathf.Floor(x); 	// Integer part of x
            fx0 = x - ix0;       	// Fractional part of x
            fx1 = fx0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff;
            ix0 = ix0 & 0xff;    	// Wrap to 0..255

            s = FADE(fx0);

            n0 = GRAD1(m_perm[ix0], fx0);
            n1 = GRAD1(m_perm[ix1], fx1);
            return 0.188f * LERP(s, n0, n1);
        }
        */

        static private fint f0507 = fint.CreateFromInt(507) / fint.CreateFromInt(1000);

        fint Noise2D(fint x, fint y)
        {
            //returns a noise value between -0.75 and 0.75
            int ix0, iy0, ix1, iy1;
            fint fx0, fy0, fx1, fy1, s, t, nx0, nx1, n0, n1;

            ix0 = (int)x.ToInt(); 	// Integer part of x
            iy0 = (int)y.ToInt(); 	// Integer part of y
            fx0 = fint.CreateRaw(x.raw & fint.decimalPartMask); //x - ix0;        	// Fractional part of x
            fy0 = fint.CreateRaw(y.raw & fint.decimalPartMask); //y - iy0;        	// Fractional part of y
            fx1 = fx0 - fint.one;
            fy1 = fy0 - fint.one;
            ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;

            t = FADE(fy0);
            s = FADE(fx0);

            nx0 = GRAD2(m_perm[ix0 + m_perm[iy0]], fx0, fy0);
            nx1 = GRAD2(m_perm[ix0 + m_perm[iy1]], fx0, fy1);

            n0 = LERP(t, nx0, nx1);

            nx0 = GRAD2(m_perm[ix1 + m_perm[iy0]], fx1, fy0);
            nx1 = GRAD2(m_perm[ix1 + m_perm[iy1]], fx1, fy1);

            n1 = LERP(t, nx0, nx1);

            return f0507 * LERP(s, n0, n1); // 0.507f * LERP(s, n0, n1);
        }

        /*
        fint Noise3D(fint x, fint y, fint z)
        {
            //returns a noise value between -1.5 and 1.5
            int ix0, iy0, ix1, iy1, iz0, iz1;
            fint fx0, fy0, fz0, fx1, fy1, fz1;
            fint s, t, r;
            fint nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = (int)Mathf.Floor(x); // Integer part of x
            iy0 = (int)Mathf.Floor(y); // Integer part of y
            iz0 = (int)Mathf.Floor(z); // Integer part of z
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fz0 = z - iz0;        // Fractional part of z
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            fz1 = fz0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            iz1 = (iz0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;
            iz0 = iz0 & 0xff;

            r = FADE(fz0);
            t = FADE(fy0);
            s = FADE(fx0);

            nxy0 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz0]]], fx0, fy0, fz0);
            nxy1 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz1]]], fx0, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz0]]], fx0, fy1, fz0);
            nxy1 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz1]]], fx0, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n0 = LERP(t, nx0, nx1);

            nxy0 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz0]]], fx1, fy0, fz0);
            nxy1 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz1]]], fx1, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz0]]], fx1, fy1, fz0);
            nxy1 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz1]]], fx1, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n1 = LERP(t, nx0, nx1);

            return 0.936f * LERP(s, n0, n1);
        }
        */

        /*
        public fint FractalNoise1D(fint x, int octNum, fint frq, fint amp)
        {
            fint gain = 1.0f;
            fint sum = 0.0f;

            for (int i = 0; i < octNum; i++)
            {
                sum += Noise1D(x * gain / frq) * amp / gain;
                gain *= 2.0f;
            }
            return sum;
        }
        */

        public fint FractalNoise2D(int x, int y, int octNum, fint frq, fint amp)
        {
            fint gain = fint.one;
            fint sum = fint.zero;

            fint fx = fint.CreateFromInt(x);
            fint fy = fint.CreateFromInt(y);

            for (int i = 0; i < octNum; i++)
            {
                sum += Noise2D(fx * gain / frq, fy * gain / frq) * amp / gain;
                gain *= fint.two;
            }
            return sum;
        }

        /*
        public fint FractalNoise3D(fint x, fint y, fint z, int octNum, fint frq, fint amp)
        {
            fint gain = 1.0f;
            fint sum = 0.0f;

            for (int i = 0; i < octNum; i++)
            {
                sum += Noise3D(x * gain / frq, y * gain / frq, z * gain / frq) * amp / gain;
                gain *= 2.0f;
            }
            return sum;
        }
        */
    }
}
