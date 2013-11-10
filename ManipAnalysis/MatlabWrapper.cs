using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace ManipAnalysis
{
    public class MatlabWrapper : IDisposable
    {
        private static readonly Type MatlabType = Type.GetTypeFromProgID("Matlab.Application");
        private readonly ManipAnalysisGui _manipAnalysisGui;
        private readonly object _matlab;
        private bool _showMatlabWindow;

        public MatlabWrapper(ManipAnalysisGui manipAnalysisGui)
        {
            _matlab = Activator.CreateInstance(MatlabType);
            _manipAnalysisGui = manipAnalysisGui;
            _showMatlabWindow = false;

            try
            {
                ClearWorkspace();
                NavigateToPath(Application.StartupPath + "\\MatlabFiles\\");
                ShowCommandWindow(false);
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("MATLAB-Interface could not be started.\n" + ex);
            }
        }

        public void Dispose()
        {
            ClearWorkspace();
            Execute("exit");
        }

        public void Execute(string command)
        {
            try
            {
                MatlabType.InvokeMember("Execute", BindingFlags.InvokeMethod, null, _matlab, new object[] {command});
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

        public void CreateMeanTimeFigure()
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','Mean time plot','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("xlabel('[Target]');");
                Execute("ylabel('Movement time [s]');");
                Execute("axis([0 18 0.2 1.4]);");
                Execute("axis manual;");
                Execute(
                    "set(gca,'YGrid','on','YTick',0.2:0.1:1.4,'XTick',1:1:17,'XTickLabel',{'1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', 'Mean'});");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateErrorclampForceFigure(int trials)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','Errorclamp forces','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("xlabel('[Errorclamp trial]');");
                Execute("ylabel('Force [N]');");
                Execute("axis([0 " + (trials + 1) + " 0 10]);");
                Execute("axis manual;");
                Execute("set(gca,'YGrid','on','YTick',0:1:10,'XTick',1:1:" + trials + ");");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateLearningIndexFigure(int setCount)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','Learning index plot','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");
                Execute("xlabel('[Set]');");
                Execute("ylabel('Learning index');");
                Execute("axis([0 " + (setCount + 1) + " -1.0 1.0]);");
                Execute("axis manual;");
                Execute("set(gca,'YGrid','on','YTick',-1.0:0.2:1.0,'XTick',1:1:" + setCount + ");");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void CreateStatisticFigure(string figureName, string dataVar, string fitVar, string stdVar,
            string xAxisLabel, string yAxisLabel, double xNegLimit, double xPosLimit,
            double yNegLimit, double yPosLimit, bool plotFit, bool plotErrorBars)
        {
            try
            {
                Execute("figure");
                Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                Execute("grid on");
                Execute("hold all");

                if (stdVar != null && plotErrorBars)
                {
                    Execute("patch([[1:1:length(" + dataVar + ")], [length(" + dataVar + "):-1:1]],[" +
                            dataVar + "(:)-" + stdVar + "(:); flipud(" + dataVar + "(:)+" + stdVar +
                            "(:))],[0.8 0.8 0.8], 'EdgeColor',[0.8 0.8 0.8])");
                }

                Execute("plot(" + dataVar + ");");

                if (fitVar != null && plotFit)
                {
                    Execute("plot(" + fitVar + ");");
                }

                Execute("xlabel('" + xAxisLabel + "');");
                Execute("ylabel('" + yAxisLabel + "');");
                Execute("set(gca,'YLim',[" +
                        yNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                        yPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) +
                        "],'YLimMode', 'manual','XLim',[" +
                        xNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                        xPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) +
                        "],'XLimMode', 'manual');");
                Execute("set(gca, 'Layer','top');");
            }
            catch (Exception ex)
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
                Execute("set(gca,'YDir','rev'); ");
                Execute("set(gca, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(gca, 'ZTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(gca, 'YTickLabel', {'0.1', '0.05', '0', '-0.05', '-0.1'});");
                Execute("set(gca,'PlotBoxAspectRatio',[1 1 1]);");
            }
            catch (Exception ex)
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
                Execute("set(axis1,'YDir','rev'); ");
                Execute("set(axis1, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis1, 'ZTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis1, 'YTickLabel', {'0.1', '0.05', '0', '-0.05', '-0.1'});");
                Execute("set(axis1,'PlotBoxAspectRatio',[1 1 1]);");
                Execute("axis([-0.13 0.13 -0.13 0.13]);");
                Execute("axis manual;");
                Execute("xlabel('Displacement [m]');");
                Execute("ylabel('Displacement [m]');");
                Execute("grid on");
                Execute(
                    "axis2 = axes('Position',get(axis1,'Position'),'XAxisLocation','top','YAxisLocation','right','Color','none','XColor','k','YColor','k');");
                Execute("set(axis2, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                Execute("set(axis2, 'ZTick', [-0.1 -0.05 0 0.05 0.1]);");
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void DrawTargets(double diameter, double radius, double centerX, double centerY)
        {
            string diameterString = diameter.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
            string radiusString = radius.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

            try
            {
                Execute("drawCircle(" + diameterString + ", cos(degtorad(0)) * " + radiusString +
                        ", sin(degtorad(0)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(45)) * " + radiusString +
                        ", sin(degtorad(45)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(90)) * " + radiusString +
                        ", sin(degtorad(90)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(135)) * " + radiusString +
                        ", sin(degtorad(135)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(180)) * " + radiusString +
                        ", sin(degtorad(180)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(225)) * " + radiusString +
                        ", sin(degtorad(225)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(270)) * " + radiusString +
                        ", sin(degtorad(270)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", cos(degtorad(315)) * " + radiusString +
                        ", sin(degtorad(315)) * " + radiusString + ");");
                Execute("drawCircle(" + diameterString + ", " +
                        centerX.ToString(CultureInfo.InvariantCulture).Replace(',', '.') + ", " +
                        centerY.ToString(CultureInfo.InvariantCulture).Replace(',', '.') + ");");
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void ShowCommandWindow(bool showWindow)
        {
            try
            {
                if (showWindow)
                {
                    Execute("showMatlabCommandWindow(1)");
                }
                else
                {
                    Execute("showMatlabCommandWindow(0)");
                }
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void SetWorkspaceData(string name, object variable)
        {
            try
            {
                MatlabType.InvokeMember("PutWorkspaceData", BindingFlags.InvokeMethod, null, _matlab,
                    new[] {name, "base", variable});
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
                return MatlabType.InvokeMember("GetVariable", BindingFlags.InvokeMethod, null, _matlab,
                    new Object[] {name, "base"}, null);
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

        public void PlotMeanTimeErrorBar(string xVar, string yVar, string stdVar)
        {
            try
            {
                Execute("errorbar(" + xVar + ", " + yVar + ", " + stdVar +
                        ", 'Marker', 'x', 'MarkerSize', 10, 'Color', [0.4 0.4 0.4], 'LineWidth', 2, 'LineStyle', 'none');");
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
                Execute("plot(" + xVar + "," + yVar + ",'Color','" + color + "','LineWidth'," +
                        lineWidth +
                        ")");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void Plot(string xVar, string yVar, string color, int lineWidth, string axis)
        {
            try
            {
                Execute("plot(" + axis + "," + xVar + "," + yVar + ",'Color','" + color + "','LineWidth'," +
                        lineWidth +
                        ")");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void Plot(string xVar, int lineWidth)
        {
            try
            {
                Execute("plot(" + xVar + ",'LineWidth'," + lineWidth + ")");
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