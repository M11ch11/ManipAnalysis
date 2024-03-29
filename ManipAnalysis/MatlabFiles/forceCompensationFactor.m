%========================================================================%   
%   Karlsruhe Institute of Technology (KIT)                              %
%   Department of Sport and Sport Science                                %
%   BioMotion Center                                                     %
%   www.sport.kit.edu                                                    %
%                                                                        %
%   Simon Renner                                                         %
%   Christian Stockinger, christian.stockinger@kit.edu                   %
%                                                                        %
%   29.10.2014                                                           %
%========================================================================%
%
%   m-file for ManipAnalysis
%   forceCompensationFactor.m
%   
%   Description                                                     
%   This function computes the force compensation factor by first computing
%   the ideal force (multiplication of velocity array and viscosity matrix)
%   and then making a linear regression fit through ideal and measured
%   force. The compensation factor is the first coefficient of the fitted
%   function.
%
%   Arguments
%   - Input:
%       velocityX and velocityY
%           velocities of data points in x and y direction (typically 1x101)
%       forcePD
%           measured perpendicular force of data points (typically 1x100)
%		forceFieldMatrix
%           a 2x2 matrix containing the forceField of the trial
%
%   - Output:
%       fcp
%			force compensation factor calculated through linear
%			regression between force ideal and force measured
%
%========================================================================%

function fcp = forceCompensationFactor(forcePD, velocityX, velocityY, forceFieldMatrix)

velocity = [velocityX; velocityY];

% Check for errors: number of input arguments
if (nargin < 4)
    error('forceCompensationFactor: ERROR - invalid input arguments - at least 4 input arguments needed!')
elseif (nargin > 4)
    error('forceCompensationFactor: ERROR - invalid input arguments - too many input arguments.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of given points
[nTraj, pTraj] = size(forcePD);  % in general: pTraj=100, nTraj=1

if (nTraj ~= 1)
    error('forceCompensationFactor: ERROR - points must live in 1-dimensional space.')
end

if (pTraj == 1)
    error('forceCompensationFactor: ERROR - forcePD is only a single point.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of given points
[nTraj, pTraj] = size(velocity);  % in general: pTraj=101, nTraj=2

if (nTraj ~= 2)
    error('forceCompensationFactor: ERROR - points must live in 2-dimensional space.')
end

if (pTraj == 1)
    error('forceCompensationFactor: ERROR - velocity is only a single point.')
end

%-------------------------------------------------------------------------%

forceIdealArray = forceFieldMatrix * velocity;

% Compute force values for Data Points from force Array
for i=1:length(forceIdealArray)
    forceIdealUncut(i)= sqrt(forceIdealArray(1, i)^2 + forceIdealArray(2, i)^2);
end

% Cut 1 datapoint to match dimension of measured and ideal force arrays
forceIdeal = forceIdealUncut (2:end);

% Linear regression fit force ideal and force measured
p = polyfit(forceIdeal, forcePD, 1);

%-------------------------------------------------------------------------%
fcp = p(1);   % The ForceFieldCompensationFactor

if isnan(fcp) % Could not calculate the ForceFieldCompensationFactor, so return zero
	fcp = 0; 	
end

end