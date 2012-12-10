using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MLApp;

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
