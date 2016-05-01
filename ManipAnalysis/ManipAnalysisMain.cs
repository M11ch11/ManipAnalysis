using System;
using System.Windows.Forms;

namespace ManipAnalysis_v2
{
    internal static class ManipAnalysisMain
    {
        /// <summary>
        ///     Main-Method, start of program
        ///     (1) Show Splashscreen
        ///     (2) Declare and instantiate all objects
        ///     (3) Close SplashScreen
        ///     (4) Start GUI-Process
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var splash = new ManipAnalysisSplash();
            splash.Show();
            try
            {
                var manipAnalysisGui = new ManipAnalysisGui();
                var matlabWrapper = new MatlabWrapper(manipAnalysisGui, MatlabWrapper.MatlabInstanceType.Shared);
                var mongoDbWrapper = new MongoDbWrapper(manipAnalysisGui);
                var manipAnalysisModel = new ManipAnalysisFunctions(manipAnalysisGui, matlabWrapper, mongoDbWrapper);
                manipAnalysisGui.SetManipAnalysisModel(manipAnalysisModel);

                splash.Close();

                Application.EnableVisualStyles();
                Application.Run(manipAnalysisGui);

                if (MessageBox.Show(@"Close the MATLAB-Instance as well?", @"Close MATLAB?", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    matlabWrapper.Dispose();
                }
            }
            catch (Exception ex)
            {
                splash.Close();
                MessageBox.Show("Error in ManipAnalysis! Please send a screenshot to a responsible person!\nChristian Stockinger [christian.stockinger@kit.edu]\n\n" + ex.Source + "\n\n" + ex.StackTrace);
            }
        }
    }
}