using System;
using System.Globalization;
using System.Windows.Forms;

namespace ManipAnalysis
{
    public class MatlabWrapper
    {
        private readonly ManipAnalysisGui _manipAnalysisGui;
        private readonly MLApp.MLApp _myMatlabInterface;

        public MatlabWrapper(ManipAnalysisGui manipAnalysisGui)
        {
            _manipAnalysisGui = manipAnalysisGui;

            try
            {
                _myMatlabInterface = new MLApp.MLApp {Visible = 0};
                ClearWorkspace();
                NavigateToPath(Application.StartupPath + "\\MatlabFiles\\");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab could not be started. Is Matlab 2012b installed?\n" + ex);
            }
        }

        public void Execute(string command)
        {
            try
            {
                _myMatlabInterface.Execute(command);
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
                _myMatlabInterface.Execute("figure");
                _myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                _myMatlabInterface.Execute("grid on");
                _myMatlabInterface.Execute("hold all");
                _myMatlabInterface.Execute("xlabel('" + xAxisLabel + "');");
                _myMatlabInterface.Execute("ylabel('" + yAxisLabel + "');");
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
                _myMatlabInterface.Execute("figure");
                _myMatlabInterface.Execute("set(gcf,'Name','Mean time plot','NumberTitle','off');");
                _myMatlabInterface.Execute("grid on");
                _myMatlabInterface.Execute("hold all");
                _myMatlabInterface.Execute("xlabel('[Target]');");
                _myMatlabInterface.Execute("ylabel('Movement time [s]');");
                _myMatlabInterface.Execute("axis([0 18 0.2 1.4]);");
                _myMatlabInterface.Execute("axis manual;");
                _myMatlabInterface.Execute(
                    "set(gca,'YGrid','on','YTick',0.2:0.1:1.4,'XTick',1:1:17,'XTickLabel',{'1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', 'Mean'});");
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
                _myMatlabInterface.Execute("figure");
                _myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                _myMatlabInterface.Execute("grid on");
                _myMatlabInterface.Execute("hold all");

                if (stdVar != null && plotErrorBars)
                {
                    _myMatlabInterface.Execute("patch([[1:1:length(" + dataVar + ")], [length(" + dataVar + "):-1:1]],[" +
                                               dataVar + "(:)-" + stdVar + "(:); flipud(" + dataVar + "(:)+" + stdVar +
                                               "(:))],[0.8 0.8 0.8], 'EdgeColor',[0.8 0.8 0.8])");
                }

                _myMatlabInterface.Execute("plot(" + dataVar + ");");

                if (fitVar != null && plotFit)
                {
                    _myMatlabInterface.Execute("plot(" + fitVar + ");");
                }

                _myMatlabInterface.Execute("xlabel('" + xAxisLabel + "');");
                _myMatlabInterface.Execute("ylabel('" + yAxisLabel + "');");
                _myMatlabInterface.Execute("set(gca,'YLim',[" +
                                           yNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                                           yPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) +
                                           "],'YLimMode', 'manual','XLim',[" +
                                           xNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                                           xPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) +
                                           "],'XLimMode', 'manual');");
                _myMatlabInterface.Execute("set(gca, 'Layer','top');");
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
                _myMatlabInterface.Execute("figure");
                _myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                _myMatlabInterface.Execute("grid on");
                _myMatlabInterface.Execute("hold all");
                _myMatlabInterface.Execute("axis([-0.13 0.13 -0.13 0.13]);");
                _myMatlabInterface.Execute("axis manual;");
                _myMatlabInterface.Execute("xlabel('Displacement [m]');");
                _myMatlabInterface.Execute("ylabel('Displacement [m]');");
                _myMatlabInterface.Execute("set(gca,'YDir','rev'); ");
                _myMatlabInterface.Execute("set(gca, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
                _myMatlabInterface.Execute("set(gca, 'ZTick', [-0.1 -0.05 0 0.05 0.1]);");
                _myMatlabInterface.Execute("set(gca, 'YTickLabel', {'0.1', '0.05', '0', '-0.05', '-0.1'});");
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
                _myMatlabInterface.Execute("figure");
                _myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
                _myMatlabInterface.Execute("grid on");
                _myMatlabInterface.Execute("hold all");
                _myMatlabInterface.Execute("axis([1 " + sampleCount + " 0 0.5]);");
                _myMatlabInterface.Execute("axis manual;");
                _myMatlabInterface.Execute("xlabel('[Samples]');");
                _myMatlabInterface.Execute("ylabel('Velocity [m/s]');");
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
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(0)) * " + radiusString +
                                           ", sin(degtorad(0)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(45)) * " + radiusString +
                                           ", sin(degtorad(45)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(90)) * " + radiusString +
                                           ", sin(degtorad(90)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(135)) * " + radiusString +
                                           ", sin(degtorad(135)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(180)) * " + radiusString +
                                           ", sin(degtorad(180)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(225)) * " + radiusString +
                                           ", sin(degtorad(225)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(270)) * " + radiusString +
                                           ", sin(degtorad(270)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", cos(degtorad(315)) * " + radiusString +
                                           ", sin(degtorad(315)) * " + radiusString + ");");
                _myMatlabInterface.Execute("drawCircle(" + diameterString + ", " +
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
                _myMatlabInterface.Execute("clear all");
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
                _myMatlabInterface.Execute("cd '" + path + "'");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }

        public void ToggleShowCommandWindow()
        {
            try
            {
                if (Convert.ToBoolean(_myMatlabInterface.Visible))
                {
                    _myMatlabInterface.Visible = 0;
                }
                else
                {
                    _myMatlabInterface.Visible = 1;
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
                _myMatlabInterface.Execute("workspace");
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
                _myMatlabInterface.PutWorkspaceData(name, "base", variable);
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
                return _myMatlabInterface.GetVariable(name, "base");
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
                _myMatlabInterface.Execute("clear " + name);
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
                _myMatlabInterface.Execute("errorbar(" + xVar + ", " + yVar + ", " + stdVar +
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
                _myMatlabInterface.Execute("plot(" + xVar + "," + yVar + ",'Color','" + color + "','LineWidth'," +
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
                _myMatlabInterface.Execute("plot(" + xVar + ",'LineWidth'," + lineWidth + ")");
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
                _myMatlabInterface.Execute("plot(" + xVar + ",'Color','" + color + "','LineWidth'," + lineWidth + ")");
            }
            catch (Exception ex)
            {
                _manipAnalysisGui.WriteToLogBox("Matlab error: " + ex);
            }
        }
    }
}