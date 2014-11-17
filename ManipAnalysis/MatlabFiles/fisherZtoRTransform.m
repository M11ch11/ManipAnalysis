%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   17.11.2014                                                          %
%=======================================================================%

%   fisherZtoRTransform.m for ManipAnalysis
%   
%   Description                                                     
%   This function transforms Fisher-Z-Values to r-Values using backward Fisher Z-transformation.
%
%   Arguments
%   - Input:
%       fisherZ = Fisher Z-transformed values (z-values)
%   - Output:
%       rvalues = r-values

function [rvalues] = fisherZtoRTransform(fisherZ)

rvalues = (exp(2*fisherZ) - 1) ./ (exp(2*fisherZ) + 1); % Backward transformation of Fisher Z-transform to r-values