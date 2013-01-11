%========================================================================%   
%   Karlsruhe Institute of Technology (KIT)                              %
%   Department of Sport and Sport Science                                %
%   BioMotion Center                                                     %
%   www.sport.kit.edu                                                    %
%                                                                        %
%   Matthias Pöschl                                                      %
%   Christian Stockinger, christian.stockinger@kit.edu                   %
%                                                                        %
%   11.01.2013                                                           %
%========================================================================%
%
%   m-file for ManipAnalysis
%   drawCircle.m
%   
%   Description                                                     
%   This function draws a red circle with a specified radius around a
%	specified point
%
%   Arguments
%   - Input:
%       radius = radius of circle
%       xpos / ypos = Position of circle-center
%   - Output:
%       none
%
%========================================================================%
function drawCircle(radius,xpos,ypos)
[x,y,z] = cylinder(radius,200);
plot(x(1,:)+xpos,y(1,:)+ypos,'Color','red','LineWidth',2);