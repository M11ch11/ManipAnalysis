using System;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using ManipAnalysis.Properties;

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

            try
            {
                string newestVersion =
                    new WebClient().DownloadString("http://eoptam.github.io/ManipAnalysis/release-version");
                if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(Version.Parse(newestVersion)) < 0)
                {
                    if (MessageBox.Show(Resources.VersionCheckerNewVersionAvailableMessageBox, @"New Version available!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("http://eoptam.github.io/ManipAnalysis/");
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.VersionCheckerCatchBlockMessageBox);
            }

            var manipAnalysisGui = new ManipAnalysisGui();
            var sqlWrapper = new SqlWrapper(manipAnalysisGui);
            var matlabWrapper = new MatlabWrapper(manipAnalysisGui);
            var manipAnalysisModel = new ManipAnalysisModel(manipAnalysisGui, matlabWrapper, sqlWrapper);
            manipAnalysisGui.SetManipAnalysisModel(manipAnalysisModel);

            splash.Close();
            splash.Dispose();

            Application.Run(manipAnalysisGui);

            matlabWrapper.Dispose();
        }
    }
}