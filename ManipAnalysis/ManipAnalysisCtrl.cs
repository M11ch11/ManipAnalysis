using System;
using System.Windows.Forms;

namespace ManipAnalysis
{
    internal static class ManipAnalysisCtrl
    {
        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var splash = new ManipAnalysisSplash();
            splash.Show();

            var manipAnalysisGui = new ManipAnalysisGui();
            var sqlWrapper = new SqlWrapper(manipAnalysisGui);
            var matlabWrapper = new MatlabWrapper(manipAnalysisGui);
            var manipAnalysisModel = new ManipAnalysisModel(manipAnalysisGui, matlabWrapper, sqlWrapper);
            manipAnalysisGui.SetManipAnalysisModel(manipAnalysisModel);

            splash.Close();
            splash.Dispose();

            Application.Run(manipAnalysisGui);
        }
    }
}