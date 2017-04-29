using System;
using System.Collections.Generic;
using System.Text;

namespace StegoCore.Statics
{
    public class JPEG
    {
        public static int[][] JpegLuminQuantTable = {
            new int[]{16,  11,  10,  16,  24,  40,  51,  61},
            new int[]{12,  12,  14,  19,  26,  58,  60,  55},
            new int[]{14,  13,  16,  24,  40,  57,  69,  56},
            new int[]{14,  17,  22,  29,  51,  87,  80,  62},
            new int[]{18,  22,  37,  56,  68, 109, 103,  77},
            new int[]{24,  35,  55,  64,  81, 104, 113,  92},
            new int[]{49,  64,  78,  87, 103, 121, 120, 101},
            new int[]{72,  92,  95,  98, 112, 100, 103,  99}
        };
    }
}
