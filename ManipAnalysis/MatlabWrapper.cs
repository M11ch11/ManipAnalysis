using System;
using System.Globalization;
using System.Windows.Forms;

namespace ManipAnalysis
{
    public class MatlabWrapper
    {
        private readonly MLApp.MLApp _myMatlabInterface;

        public MatlabWrapper()
        {
            _myMatlabInterface = new MLApp.MLApp {Visible = 0};
            ClearWorkspace();
            NavigateToPath(Application.StartupPath + "\\MatlabFiles\\");
        }

        public void Execute(string command)
        {
            _myMatlabInterface.Execute(command);
        }

        public void CreateFigure(string figureName, string xAxisLabel, string yAxisLabel)
        {
            _myMatlabInterface.Execute("figure");
            _myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
            _myMatlabInterface.Execute("grid on");
            _myMatlabInterface.Execute("hold all");
            _myMatlabInterface.Execute("xlabel('" + xAxisLabel + "');");
            _myMatlabInterface.Execute("ylabel('" + yAxisLabel + "');");
        }

        public void CreateMeanTimeFigure()
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

        public void CreateStatisticFigure(string figureName, string dataVar, string fitVar, string stdVar,
                                          string xAxisLabel, string yAxisLabel, double xNegLimit, double xPosLimit,
                                          double yNegLimit, double yPosLimit, bool plotFit, bool plotErrorBars)
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

        public void CreateTrajectoryFigure(string figureName)
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

        public void CreateVelocityFigure(string figureName, int sampleCount)
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

        public void DrawTargets(double diameter, double radius, double centerX, double centerY)
        {
            string diameterString = diameter.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
            string radiusString = radius.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

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

        public void ClearWorkspace()
        {
            _myMatlabInterface.Execute("clear all");
        }

        private void NavigateToPath(string path)
        {
            _myMatlabInterface.Execute("cd '" + path + "'");
        }

        public void ToggleShowCommandWindow()
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

        public void ShowWorkspaceWindow()
        {
            _myMatlabInterface.Execute("workspace");
        }

        public void SetWorkspaceData(string name, object variable)
        {
            _myMatlabInterface.PutWorkspaceData(name, "base", variable);
        }

        public dynamic GetWorkspaceData(string name)
        {
            return _myMatlabInterface.GetVariable(name, "base");
        }

        public void ClearWorkspaceData(string name)
        {
            _myMatlabInterface.Execute("clear " + name);
        }

        public void PlotMeanTimeErrorBar(string xVar, string yVar, string stdVar)
        {
            _myMatlabInterface.Execute("errorbar(" + xVar + ", " + yVar + ", " + stdVar +
                                       ", 'Marker', 'x', 'MarkerSize', 10, 'Color', [0.4 0.4 0.4], 'LineWidth', 2, 'LineStyle', 'none');");
        }

        public void Plot(string xVar, string yVar, string color, int lineWidth)
        {
            _myMatlabInterface.Execute("plot(" + xVar + "," + yVar + ",'Color','" + color + "','LineWidth'," + lineWidth +
                                       ")");
        }

        public void Plot(string xVar, int lineWidth)
        {
            _myMatlabInterface.Execute("plot(" + xVar + ",'LineWidth'," + lineWidth + ")");
        }

        public void Plot(string xVar, string color, int lineWidth)
        {
            _myMatlabInterface.Execute("plot(" + xVar + ",'Color','" + color + "','LineWidth'," + lineWidth + ")");
        }
    }
}