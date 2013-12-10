%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   07.11.2013                                                          %
%=======================================================================%

%   vectorCorrelationFisherZTransform.m for ManipAnalysis
%   
%   Description                                                     
%   This function transforms the vector correlation coeffcient values 
%	using Fisher Z-transform (necessarry for further analytical steps).
%
%   Arguments
%   - Input:
%       corr = velocity vector correlation coefficient (r-values)
%   - Output:
%       fisherZ = Fisher Z-transformed velocity vector correlation
%       coefficient (z-values)

function [fisherZ] = vectorCorrelationFisherZTransform(corr)

fisherZ = 0.5 * log((1+corr) ./ (1-corr));     % Fisher Z-transform of correlation coefficient