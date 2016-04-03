%========================================================================%   
%   Karlsruhe Institute of Technology (KIT)                              %
%   Department of Sport and Sport Science                                %
%   BioMotion Center                                                     %
%   www.sport.kit.edu                                                    %
%                                                                        %
%   Matthias Pöschl                                                      %
%   Christian Stockinger, christian.stockinger@kit.edu                   %
%                                                                        %
%   03.04.2016                                                           %
%========================================================================%
%
%   m-file for ManipAnalysis
%   predicitionAndFeedbackAngle.m
%   
%   Description                                                     
%   TODO
%
%   Arguments
%   - Input:
%       startPoint = coorrdinates of origin [1x2 matrix]
%       endPoint = coorrdinates of target [1x2 matrix]
%       point180ms = coorrdinates of subject at 180ms [1x2 matrix]
%       point400ms = coorrdinates of subject at 400ms [1x2 matrix]
%   - Output:
%       predictionAngle = angle between startPoint-endPoint and startPoint-point180ms
%       feedbackAngle = angle between point180ms-endPoint and point180ms-point400ms
%
%========================================================================%

function [predictionAngle, feedbackAngle] = predicitionAndFeedbackAngle(startPoint, endPoint, point180ms, point400ms)

% Check for errors: number of input arguments
if (nargin ~= 4)
    error('predicitionAndFeedbackAngle: ERROR - invalid input arguments - 4 input arguments needed!')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of startPoint argument
[nTraj, pTraj] = size(startPoint);  % in general: nTraj=1, pTraj=2

if (pTraj ~= 2)
    error('predicitionAndFeedbackAngle: ERROR - startPoint must live in 2-dimensional space.')
end

if (nTraj ~= 1)
    error('predicitionAndFeedbackAngle: ERROR - startPoint must contain data points.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of endPoint argument
[nTraj, pTraj] = size(endPoint);  % in general: nTraj=1, pTraj=2

if (pTraj ~= 2)
    error('predicitionAndFeedbackAngle: ERROR - endPoint must live in 2-dimensional space.')
end

if (nTraj ~= 1)
    error('predicitionAndFeedbackAngle: ERROR - endPoint must contain data points.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of point180ms argument
[nTraj, pTraj] = size(point180ms);  % in general: nTraj=1, pTraj=2

if (pTraj ~= 2)
    error('predicitionAndFeedbackAngle: ERROR - point180ms must live in 2-dimensional space.')
end

if (nTraj ~= 1)
    error('predicitionAndFeedbackAngle: ERROR - point180ms must contain data points.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of point400ms argument
[nTraj, pTraj] = size(point400ms);  % in general: nTraj=1, pTraj=2

if (pTraj ~= 2)
    error('predicitionAndFeedbackAngle: ERROR - point400ms must live in 2-dimensional space.')
end

if (nTraj ~= 1)
    error('predicitionAndFeedbackAngle: ERROR - point400ms must contain data points.')
end

%-------------------------------------------------------------------------%
startPoint = double(startPoint);    % ensure startPoint entries are doubles
endPoint = double(endPoint);        % ensure endPoint entries are doubles
point180ms = double(point180ms);    % ensure point180ms entries are doubles
point400ms = double(point400ms);    % ensure point400ms entries are doubles
%-------------------------------------------------------------------------%

start_end_vector = startPoint - endPoint;
start_point180ms_vector = startPoint - point180ms;
point180ms_end_vector = point180ms - endPoint;
point180ms_point400ms = point180ms - point400ms;

predictionAngle = rad2deg(atan2(norm(cross([start_point180ms_vector 0],[start_end_vector 0])),dot(start_point180ms_vector, start_end_vector)));
feedbackAngle = rad2deg(atan2(norm(cross([point180ms_point400ms 0],[point180ms_end_vector 0])),dot(point180ms_point400ms, point180ms_end_vector)));

end