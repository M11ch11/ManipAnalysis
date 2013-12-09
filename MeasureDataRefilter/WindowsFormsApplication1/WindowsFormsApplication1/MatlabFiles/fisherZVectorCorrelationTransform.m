%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias P�schl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   07.11.2013                                                          %
%=======================================================================%

%   fisherZVectorCorrelationTransform.m for ManipAnalysis
%   
%   Description                                                     
%   This function transforms Fisher-Z-Values to vector correlation coeffcient
%	r-Values using backward Fisher Z-transformation.
%
%   Arguments
%   - Input:
%       fisherZ = Fisher Z-transformed velocity vector correlation
%       coefficient (z-values)
%   - Output:
%       corr = velocity vector correlation coefficient (r-values)

function [corr] = fisherZVectorCorrelationTransform(fisherZ)

corr = (exp(2*fisherZ) - 1) ./ (exp(2*fisherZ) + 1); % Backward transformation of Fisher Z-transform to r-values