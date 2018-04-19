using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace ManipAnalysis_v2
{
    internal class MatlabWrapper : IDisposable
    {
        public enum MatlabInstanceType
        {
            Shared,
            Single
        }
        private class MatlabException : Exception
        {
            public MatlabException(string message) : base(message)
            {
            }
        }

        private readonly MatlabInstanceType _instanceType;

        private readonly ManipAnalysisGui _manipAnalysisGui;

        private readonly object _matlab;

        private readonly Type _matlabType;

        private bool _showMatlabWindow = false;

        public MatlabWrapper(ManipAnalysisGui manipAnalysisGui, MatlabInstanceType instanceType)
        {
            try
            {


                _instanceType = instanceType;
                if (_instanceType == MatlabInstanceType.Shared)
                {
                    _matlabType = Type.GetTypeFromProgID("Matlab.Autoserver");
                }
                else if (_instanceType == MatlabInstanceType.Single)
                {
                    _matlabType = Type.GetTypeFromProgID("Matlab.Autoserver.Single");
                }

                if (_matlabType != null)
                {
                    _matlab = Activator.CreateInstance(_matlabType);
                }
                else
                {
                    /* rausgenommen, da evtl durch unsychronen Aufruf manipAnalysisGUI noch nicht gestartet ist zu diesem Zeitpunkt und dann alles abschmiert...

                    _manipAnalysisGui.WriteToLogBox("MATLAB-Interface could not be started! Please restart.");

                    */
                    throw new MatlabException("Matlab-Instance is could not be started and is instatiated with null");
                }
                _manipAnalysisGui = manipAnalysisGui;

                ClearWorkspace();
                NavigateToPath(Application.StartupPath + "\\MatlabFiles\\");
                ShowCommandWindow(false);
            }
            catch (MatlabException e)
            {
                MessageBox.Show("Matlab-Instance could not be started, please close the application, as it will not work properly! " + e.Source + "\n\n" + e.StackTrace);
            }

        }

        public void Dispose()
        {
            ClearWorkspace();

            if (_instanceType == MatlabInstanceType.Shared)
            {
                Execute("exit");
            }
            else if (_instanceType == MatlabInstanceType.Single && _matlabType != null)
            {
                _matlabType.InvokeMember("Quit", BindingFlags.InvokeMethod, null, _matlab, null);
            }
        }

        public void Execute(string command)
        {
            try
            {
                _matlabType.InvokeMember("Execute", BindingFlags.InvokeMethod, null, _matlab, new object[] { command });
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateFigure(string figureName, string xAxisLabel, string yAxisLabel)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("xlabel('" + xAxisLabel + "');");
                Execute("ylabel('" + yAxisLabel + "');");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateForceFigure(string figureName, string xAxisLabel, string yAxisLabel)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("xlabel('" + xAxisLabel + "');");
                Execute("ylabel('" + yAxisLabel + "');");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void AddLegend(string legend1, string legend2)
        {
            try
            {
                Execute("legend('" + legend1 + "','" + legend2 + "');");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void AddLegend(string legend1, string legend2, string legend3)
        {
            try
            {
                Execute("legend('" + legend1 + "','" + legend2 + "','" + legend3 + "');");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateStatisticFigure(string figureName, string dataVar, string fitVar, string stdVar,
            string xAxisLabel, string yAxisLabel, double xNegLimit, double xPosLimit, double yNegLimit, double yPosLimit,
            bool plotFit, bool plotErrorBars)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");

                if (stdVar != null && plotErrorBars)
                {
                    Execute("patch([[1:1:length(" + dataVar + ")], [length(" + dataVar + "):-1:1]],[" + dataVar + "(:)-" +
                            stdVar + "(:); flipud(" + dataVar + "(:)+" + stdVar +
                            "(:))],[0.8 0.8 0.8], 'EdgeColor',[0.8 0.8 0.8])");
                }

                Execute("plot(" + dataVar + ");");

                if (fitVar != null && plotFit)
                {
                    Execute("plot(" + fitVar + ");");
                }

                Execute("xlabel('" + xAxisLabel + "');");
                Execute("ylabel('" + yAxisLabel + "');");
                Execute("set(gca,'YLim',[" + yNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                        yPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) +
                        "],'YLimMode', 'manual','XLim',[" +
                        xNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                        xPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + "],'XLimMode', 'manual');");
                Execute("set(gca, 'Layer','top');");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateTrajectoryFigure(string figureName)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("axis([-0.13 0.13 -0.13 0.13]);");
                Execute("axis manual;");
                Execute("xlabel('Displacement [m]');");
                Execute("ylabel('Displacement [m]');");
                //Execute("set(gca,'YDir','rev'); "); // Old BioMotionBot Bug
                Execute("set(gca, 'XTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(gca, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(gca, 'YTickLabel', {'-0.1', '-0.05', '0', '0.05', '0.1'});");
                Execute("set(gca,'PlotBoxAspectRatio',[1 1 1]);");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateTrajectoryForceFigure(string figureName)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("axis1 = gca;");
                Execute("set(axis1,'XColor','k','YColor','k')");
                //Execute("set(axis1,'YDir','rev'); "); // Old BioMotionBot Bug
                Execute("set(axis1, 'XTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis1, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis1, 'YTickLabel', {'-0.1', '-0.05', '0', '0.05', '0.1'});");
                Execute("set(axis1,'PlotBoxAspectRatio',[1 1 1]);");
                Execute("axis([-0.13 0.13 -0.13 0.13]);");
                Execute("axis manual;");
                Execute("xlabel('Displacement [m]');");
                Execute("ylabel('Displacement [m]');");
                Execute("grid on");
                Execute(
                    "axis2 = axes('Position',get(axis1,'Position'),'XAxisLocation','top','YAxisLocation','right','Color','none','XColor','k','YColor','k');");
                Execute("set(axis2, 'XTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis2, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis2, 'XTickLabel', {'', '', '', '', ''});");
                Execute("set(axis2, 'YTickLabel', {'', '', '', '', ''});");
                Execute("set(axis2,'PlotBoxAspectRatio',[1 1 1]);");
                Execute("axis([-0.13 0.13 -0.13 0.13]);");
                Execute("axis manual;");
                Execute("xlabel('Force [100N]');");
                Execute("ylabel('Force [100N]');");
                Execute("axes(axis1);");
                Execute("hold all");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateVelocityFigure(string figureName, int sampleCount)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("axis([1 " + sampleCount + " 0 0.5]);");
                Execute("axis manual;");
                Execute("xlabel('[Samples]');");
                Execute("ylabel('Velocity [m/s]');");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }
        
        public void DrawTargets(System.Collections.Generic.List<MongoDb.TargetContainer> targets)
        {
            try {
                foreach (var target in targets)
                {
                    Execute("drawCircle(" + target.Radius.ToString(CultureInfo.InvariantCulture).Replace(',', '.')
                        + ", " + target.XPos.ToString(CultureInfo.InvariantCulture).Replace(',', '.')
                        + ", " + target.YPos.ToString(CultureInfo.InvariantCulture).Replace(',', '.')
                        + ");");
                }
            } catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }


        public void ClearWorkspace()
        {
            try
            {
                Execute("clear all");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        private void NavigateToPath(string path)
        {
            try
            {
                Execute("cd '" + path + "'");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void ShowCommandWindow(bool showWindow)
        {
            try
            {
                Execute(showWindow ? "showMatlabCommandWindow(1)" : "showMatlabCommandWindow(0)");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void ToggleCommandWindow()
        {
            try
            {
                if (_showMatlabWindow)
                {
                    Execute("showMatlabCommandWindow(0)");
                    _showMatlabWindow = false;
                }
                else
                {
                    Execute("showMatlabCommandWindow(1)");
                    _showMatlabWindow = true;
                }
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void ShowWorkspaceWindow()
        {
            try
            {
                Execute("workspace");
            }
            catch (Exception
                ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void SetWorkspaceData(string name, object variable)
        {
            try
            {
                _matlabType.InvokeMember("PutWorkspaceData", BindingFlags.InvokeMethod, null, _matlab,
                    new[] { name, "base", variable });
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public dynamic GetWorkspaceData(string name)
        {
            try
            {
                return _matlabType.InvokeMember("GetVariable", BindingFlags.InvokeMethod, null, _matlab,
                    new object[] { name, "base" }, null);
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
                return null;
            }
        }

        public void ClearWorkspaceData(string name)
        {
            try
            {
                Execute("clear " + name);
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void Plot(string xVar, string yVar, string color, int lineWidth)
        {
            try
            {
                Execute("plot(" + xVar + "," + yVar + ",'Color','" + color + "','LineWidth'," + lineWidth + ")");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void Plot(string xVar, string color, int lineWidth)
        {
            try
            {
                Execute("plot(" + xVar + ",'Color','" + color + "','LineWidth'," + lineWidth + ")");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }
    }
}