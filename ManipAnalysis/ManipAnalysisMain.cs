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

            var manipAnalysisGui = new ManipAnalysisGui();
            var matlabWrapper = new MatlabWrapper(manipAnalysisGui);
            var mongoDbWrapper = new MongoDbWrapper(manipAnalysisGui);
            var manipAnalysisModel = new ManipAnalysisFunctions(manipAnalysisGui, matlabWrapper, mongoDbWrapper);
            manipAnalysisGui.SetManipAnalysisModel(manipAnalysisModel);

            splash.Close();

            Application.EnableVisualStyles();
            Application.Run(manipAnalysisGui);

            matlabWrapper.Dispose();
        }
    }
}