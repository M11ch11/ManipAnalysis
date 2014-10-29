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
%   This function calculates the minimal perpendicular distance for each
%   point of a given set of points (trial trajectory) to the corresponding 
%   reference trajectory (straight line joining start and target points) 
%   using rotation of each point into x-direction and identification
%   of the magnitude of the z-value.
%
%   Arguments
%   - Input:
%       trajectory = Set of points of a trajectory in 2-dimensional space
%           given as a n x 2 matrix. Usually, for ManipAnalysis this is a 
%           time normalized 101 x 2 matix. 
%           Thereby, each row of this matrix represents the 2-dimensional 
%           coordinates of a single point.
%       targetNumber = number of target point (movement direction). 
%           Necessary to be able to identify straight line joining start 
%           and target point.
%   - Output:
%       distance = vector containing all computed perpendicular distances 
%           of given points to the baseline trajectory 
%
%========================================================================%

function fcp = forceCompensationFactor(forcePD, forceSign, velocity)

% Check for errors: number of input arguments
if (nargin < 2)
    error('distance2curve: ERROR - invalid input arguments - at least 2 input arguments needed!')
elseif (nargin <2)
    error('distance2curve: ERROR - invalid input arguments - too many input arguments.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of given points
[nTraj, pTraj] = size(forcePD);  % in general: nTraj=100, pTraj=2

if (pTraj ~= 2)
    error('distance2curve: ERROR - points must live in 2-dimensional space.')
end

if (nTraj == 1)
    error('distance2curve: ERROR - trajectory is only a single point.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of given points
[nTraj, pTraj] = size(forceSign);  % in general: nTraj=100, pTraj=2

if (pTraj ~= 2)
    error('distance2curve: ERROR - points must live in 2-dimensional space.')
end

if (nTraj == 1)
    error('distance2curve: ERROR - trajectory is only a single point.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of given points
[nTraj, pTraj] = size(velocity);  % in general: nTraj=101, pTraj=2

if (pTraj ~= 2)
    error('distance2curve: ERROR - points must live in 2-dimensional space.')
end

if (nTraj == 1)
    error('distance2curve: ERROR - trajectory is only a single point.')
end

%-------------------------------------------------------------------------%



%-------------------------------------------------------------------------%
fcp = 0;   % Return value

end