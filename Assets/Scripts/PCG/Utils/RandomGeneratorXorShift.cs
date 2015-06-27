/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using System;

namespace PCG
{
    public class RandomGeneratorXorShift
    {
        private uint state;
        
        public RandomGeneratorXorShift (int seed) 
        {
            state = (uint)seed;
        }

        private uint Next () 
        {
            // Xorshift algorithm from George Marsaglia's paper.
            state ^= (state << 13);
            state ^= (state >> 17);
            state ^= (state << 5);
            return state;
        }

        public int Value () 
        {
            return (int) Next();
        }
        
        public int Range (int min, int max) 
        {
            return min + (int)(Next () % (max - min));
        }
    }
}

