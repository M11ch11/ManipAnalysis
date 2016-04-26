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
            try {
                var splash = new ManipAnalysisSplash();
                splash.Show();

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
            catch(Exception ex)
            {
                MessageBox.Show("Error! Please send a screenshot to a responsible person!\n\n" + ex.ToString());
            }
        }
    }
}