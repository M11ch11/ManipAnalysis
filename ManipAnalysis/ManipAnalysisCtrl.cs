using System;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using ManipAnalysisLib;

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
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            var splash = new ManipAnalysisSplash();
            splash.Show();

            try
            {
                string newestVersion =
                    new WebClient().DownloadString("http://eoptam.github.io/ManipAnalysis/release-version");
                
                if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(Version.Parse(newestVersion)) < 0)
                {
                    if (MessageBox.Show(@"There is a new version of ManipAnalysis available. Do you want to download it?", @"New Version available!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("http://eoptam.github.io/ManipAnalysis/");
                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Could not check for a new version of ManipAnalysis. Are you connected to the Internet?");
            }

            //MessageBox.Show(Assembly.LoadFrom("ManipAnalysisLib.dll").GetName().Version.ToString());

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