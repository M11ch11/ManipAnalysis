using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MLApp;
using System.Globalization;

namespace ManipAnalysis
{
    class MatlabWrapper
    {
        public void createFigure(MLApp.MLApp myMatlabInterface, string figureName, string xAxisLabel, string yAxisLabel)
        {
            myMatlabInterface.Execute("figure");
            myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
            myMatlabInterface.Execute("grid on");
            myMatlabInterface.Execute("hold all");
            myMatlabInterface.Execute("xlabel('" + xAxisLabel + "');");
            myMatlabInterface.Execute("ylabel('" + yAxisLabel + "');");
        }

        public void createMeanTimeFigure(MLApp.MLApp myMatlabInterface)
        {
            myMatlabInterface.Execute("figure");
            myMatlabInterface.Execute("set(gcf,'Name','Mean time plot','NumberTitle','off');");
            myMatlabInterface.Execute("grid on");
            myMatlabInterface.Execute("hold all");
            myMatlabInterface.Execute("xlabel('[Target]');");
            myMatlabInterface.Execute("ylabel('Movement time [s]');");
            myMatlabInterface.Execute("axis([0 18 0.2 1.4]);");
            myMatlabInterface.Execute("axis manual;");
            myMatlabInterface.Execute("set(gca,'YGrid','on','YTick',0.2:0.1:1.4,'XTick',1:1:17,'XTickLabel',{'1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', 'Mean'});");
        }

        public void createStatisticFigure(MLApp.MLApp myMatlabInterface, string figureName, string dataVar, string fitVar, string stdVar, string xAxisLabel, string yAxisLabel, double xNegLimit, double xPosLimit, double yNegLimit, double yPosLimit, bool plotFit, bool plotErrorBars)
        {
            myMatlabInterface.Execute("figure");
            myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
            myMatlabInterface.Execute("grid on");
            myMatlabInterface.Execute("hold all");
            
            if (stdVar != null && plotErrorBars)
            {
                myMatlabInterface.Execute("patch([[1:1:length(" + dataVar + ")], [length(" + dataVar + "):-1:1]],[" + dataVar + "(:)-" + stdVar + "(:); flipud(" + dataVar + "(:)+" + stdVar + "(:))],[0.8 0.8 0.8], 'EdgeColor',[0.8 0.8 0.8])");
            }

            myMatlabInterface.Execute("plot(" + dataVar + ");");

            if (fitVar != null && plotFit)
            {
                myMatlabInterface.Execute("plot(" + fitVar + ");");
            }

            myMatlabInterface.Execute("xlabel('" + xAxisLabel + "');");
            myMatlabInterface.Execute("ylabel('" + yAxisLabel + "');");
            myMatlabInterface.Execute("set(gca,'YLim',[" + yNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " + yPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + "],'YLimMode', 'manual','XLim',[" + xNegLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + " " + xPosLimit.ToString("R", CultureInfo.CreateSpecificCulture("en-US")) + "],'XLimMode', 'manual');");
            myMatlabInterface.Execute("set(gca, 'Layer','top');");
        }

        public void createTrajectoryFigure(MLApp.MLApp myMatlabInterface, string figureName)
        {
            myMatlabInterface.Execute("figure");
            myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
            myMatlabInterface.Execute("grid on");
            myMatlabInterface.Execute("hold all");
            myMatlabInterface.Execute("axis([-0.13 0.13 -0.13 0.13]);");
            myMatlabInterface.Execute("axis manual;");
            myMatlabInterface.Execute("xlabel('Displacement [m]');");
            myMatlabInterface.Execute("ylabel('Displacement [m]');");
            myMatlabInterface.Execute("set(gca,'YDir','rev'); ");
            myMatlabInterface.Execute("set(gca, 'YTick', [-0.1 -0.05 0 0.05 0.1]);");
            myMatlabInterface.Execute("set(gca, 'ZTick', [-0.1 -0.05 0 0.05 0.1]);");
            myMatlabInterface.Execute("set(gca, 'YTickLabel', {'0.1', '0.05', '0', '-0.05', '-0.1'});");
        }

        public void createVelocityFigure(MLApp.MLApp myMatlabInterface, string figureName, int sampleCount)
        {
            myMatlabInterface.Execute("figure");
            myMatlabInterface.Execute("set(gcf,'Name','" + figureName + "','NumberTitle','off');");
            myMatlabInterface.Execute("grid on");
            myMatlabInterface.Execute("hold all");
            myMatlabInterface.Execute("axis([1 " + sampleCount + " 0 0.5]);");
            myMatlabInterface.Execute("axis manual;");
            myMatlabInterface.Execute("xlabel('[Samples]');");
            myMatlabInterface.Execute("ylabel('Velocity [m/s]');");
        }

        public void drawTargets(MLApp.MLApp myMatlabInterface, double diameter, double radius, double centerX, double centerY)
        {
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(0)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(0)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(45)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(45)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(90)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(90)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(135)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(135)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(180)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(180)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(225)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(225)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(270)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(270)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", cos(degtorad(315)) * " + radius.ToString().Replace(',', '.') + ", sin(degtorad(315)) * " + radius.ToString().Replace(',', '.') + ");");
            myMatlabInterface.Execute("drawCircle(" + diameter.ToString().Replace(',', '.') + ", " + centerX.ToString().Replace(',', '.') + ", " + centerY.ToString().Replace(',', '.') + ");");
        }
    }
}
