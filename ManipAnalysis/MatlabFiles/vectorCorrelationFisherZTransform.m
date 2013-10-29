%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias P�schl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   28.10.2013                                                          %
%=======================================================================%

%   vectorCorrelationFisherZTransform.m for ManipAnalysis
%   
%   Description                                                     
%   This function calculates the velocity vector correlation coefficient
%   between baseline and movement trial. Afterwards the correlation
%   coeffcient values are transformed using Fisher Z-transform (necessarry 
%   for further analytical steps).
%   This file is constructed for time normalized data (commonly N=101).
%
%   Arguments
%   - Input:
%       trialData = Vector of dimension N (=101) representating velocity 
%       profile of trajectory points
%       baselineData = Vector of dimension N (=101) representating 
%       velocity profile of baseline
%   - Output:
%       corrFisherZ = Fisher Z-transformed velocity vector correlation
%       coefficient

function [corrFisherZ] = vectorCorrelationFisherZTransform(trialData,baselineData)

mean_trialData = mean(trialData);
mean_baselineData = mean(baselineData);

cov = mean(dot(trialData,baselineData,2)) - dot(mean_trialData,mean_baselineData);

var_trialData = mean(dot(trialData,trialData,2)) - dot(mean_trialData,mean_trialData);
var_baselineData = mean(dot(baselineData,baselineData,2)) - dot(mean_baselineData,mean_baselineData);

corr = cov / sqrt(var_trialData * var_baselineData);

corrFisherZ = 0.5 * log((1+corr)/(1-corr));     % Fisher Z-transform of correlation coefficient