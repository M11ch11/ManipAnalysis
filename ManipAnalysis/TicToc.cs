using System;

namespace ManipAnalysis_v2
{
    internal static class TicToc
    {
        private static long _tic, _toc;

        /// <summary>
        ///     Starts TicToc-Logic
        /// </summary>
        public static void Tic()
        {
            _tic = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        ///     Ends TicToc-Logic.
        /// </summary>
        /// <returns>Time between Tic() and Toc() in milliseconds.</returns>
        public static long Toc()
        {
            _toc = DateTime.UtcNow.Ticks;
            return (_toc - _tic)/10000;
        }
    }
}