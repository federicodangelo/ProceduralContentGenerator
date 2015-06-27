using System;

/* Simple plasma fractal implementation. 
 *
 * Justin Seyster, 2002
 *
 * Permission is granted by the author to use the source code provided
 * in this file for any purpose, on its own or as part of a derivative
 * work, and with no restrictions.  Enjoy!
 */

//Origianl source code: https://github.com/jseyster/plasmafractal/blob/master/Plasma.java
namespace PCG
{
    public class PlasmaGenerator
    {
        //This is something of a "helper function" to create an initial grid
        //before the recursive function is called.  
        static public void DrawPlasma(Matrix matrix, int seed, int min, int max)
        {
            int c1, c2, c3, c4;

            RandomGeneratorXorShift rnd = new RandomGeneratorXorShift(seed);
            
            //Assign the four corners of the intial grid random color values
            //These will end up being the colors of the four corners of the applet.     
            c1 = rnd.Range(min, max);
            c2 = rnd.Range(min, max);
            c3 = rnd.Range(min, max);
            c4 = rnd.Range(min, max);
            
            DivideGrid(matrix, rnd, 0, 0, matrix.size, c1, c2, c3, c4, min, max);
        }

        //Randomly displaces color value for midpoint depending on size
        //of grid piece.
        static int Displace(RandomGeneratorXorShift rnd, int num)
        {
            return rnd.Range(-num/2, num/2 + 1);
        }

        //This is the recursive function that implements the random midpoint
        //displacement algorithm.  It will call itself until the grid pieces
        //become smaller than one pixel.    
        static private void DivideGrid(Matrix matrix, RandomGeneratorXorShift rnd, int x, int y, int size, int c1, int c2, int c3, int c4, int min, int max)
        {
            int Edge1, Edge2, Edge3, Edge4, Middle;

            if (size > 1)
            {   
                int newSize = size / 2;

                Middle = (c1 + c2 + c3 + c4) / 4 + Displace(rnd, newSize);  //Randomly displace the midpoint!
                Edge1 = (c1 + c2) / 2;
                Edge2 = (c2 + c3) / 2;
                Edge3 = (c3 + c4) / 2;
                Edge4 = (c4 + c1) / 2;
                
                //Make sure that the midpoint doesn't accidentally "randomly displaced" past the boundaries!
                if (Middle < min)
                    Middle = min;
                else if (Middle > max)
                    Middle = max;

                //Do the operation over again for each of the four new grids.           
                DivideGrid(matrix, rnd, x, y, newSize, c1, Edge1, Middle, Edge4, min, max);
                DivideGrid(matrix, rnd, x + newSize, y, newSize, Edge1, c2, Edge2, Middle, min, max);
                DivideGrid(matrix, rnd, x + newSize, y + newSize, newSize, Middle, Edge2, c3, Edge3, min, max);
                DivideGrid(matrix, rnd, x, y + newSize, newSize, Edge4, Middle, Edge3, c4, min, max);
            }
            else    //This is the "base case," where each grid piece is less than the size of a pixel.
            {
                //The four corners of the grid piece will be averaged and drawn as a single pixel.
                int c = (c1 + c2 + c3 + c4) / 4;
                matrix.SetValue(x, y, (byte) c);
            }
        }
    }
}

