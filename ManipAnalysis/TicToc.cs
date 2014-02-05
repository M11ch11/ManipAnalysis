﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace ManipAnalysis_v2
{
    static class TicToc
    {
        private static long tic, toc;

        /// <summary>
        /// Starts TicToc-Logic
        /// </summary>
        public static void Tic()
        {
            tic = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Ends TicToc-Logic.
        /// </summary>
        /// <returns>Time between Tic() and Toc() in milliseconds.</returns>
        public static long Toc()
        {
            toc = DateTime.UtcNow.Ticks;
            return (toc - tic) / 10000;
        }

    }
}