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
%   enclosedArea.m
%   
%   Description                                                     
%   Calculated the area enclosed by a curve.
%	Uses a copy of first point to form a closed curve
%
%   Arguments
%   - Input:
%       posX/posY = the x&y coordinates of the curve
%   - Output:
%       the calculated enclosed Area
%
%========================================================================%
function [area] = enclosedArea(posX,posY)

X = [posX,posX(1)];
Y = [posY,posY(1)];

area = polyarea(X,Y);