%raus!
%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias P�schl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   17.11.2014                                                          %
%=======================================================================%

%   fisherZTransform.m for ManipAnalysis
%   
%   Description                                                     
%   This function transforms values using Fisher Z-transform (necessarry for further analytical steps).
%
%   Arguments
%   - Input:
%       rvalues = (r-values)
%   - Output:
%       fisherZ = Fisher Z-transformed values (z-values)

function [fisherZ] = fisherZTransform(rvalues)

fisherZ = 0.5 * log((1+rvalues) ./ (1-rvalues));     % Fisher Z-transform of correlation coefficient