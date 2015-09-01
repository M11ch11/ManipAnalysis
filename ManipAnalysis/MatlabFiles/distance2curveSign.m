%========================================================================%   
%   Karlsruhe Institute of Technology (KIT)                              %
%   Department of Sport and Sport Science                                %
%   BioMotion Center                                                     %
%   www.sport.kit.edu                                                    %
%                                                                        %
%   Matthias Pöschl                                                      %
%   Christian Stockinger, christian.stockinger@kit.edu                   %
%                                                                        %
%   23.10.2012                                                           %
%========================================================================%
%
%   m-file for ManipAnalysis
%   distance2curveSign.m
%   
%   Description                                                     
%   This function calculates the minimal perpendicular distance for each
%   point of a given set of points (trial trajectory) to the corresponding 
%   reference trajectory (straight line joining start and target points) 
%   using rotation of each point into x-direction and identification
%   of the magnitude of the z-value.
%   The distance measure generates positive (negative) values if 
%   displacement of straight line joining start & target point is in 
%   CW direction (CCW direction) from the subject's point of view. 
%   This definition is based on outward-directed movements. 
%   For inward-directed movements, the definition of positive/negative sign 
%   is the other way round.
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

function distance = distance2curveSign(trajectory, targetNumber)

% Check for errors: number of input arguments
if (nargin < 2)
    error('distance2curve: ERROR - invalid input arguments - at least 2 input arguments needed!')
elseif (nargin <2)
    error('distance2curve: ERROR - invalid input arguments - too many input arguments.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of given points
[nTraj, pTraj] = size(trajectory);  % in general: nTraj=101, pTraj=2

if (pTraj ~= 2)
    error('distance2curve: ERROR - points must live in 2-dimensional space.')
end

if (nTraj == 1)
    error('distance2curve: ERROR - trajectory is only a single point.')
end

trajectory = double(trajectory);    % ensure trajectory entries are doubles

%-------------------------------------------------------------------------%
% Defintitions
baseLength = 0.1;   % length of straight line (typ. 0.1m) for center-out tasks
R = [0,0;0,0];      % allocation for rotation matrix
                    % R = [cos(alpha), -sin(alpha); sin(alpha), cos(alpha)]

%-------------------------------------------------------------------------%
% 3-dimensional rotation of trajectory point and straight line joinig 
% start & target point to the x-axis (ankle = alpha)

%targetDefinition = 'Study1-6_BioMotionBot';
%targetDefinition = 'Study1-6_KINARM';
%targetDefinition = 'Study7_KINARM';
targetDefinition = 'Study10_KINARM';

if(strcmp(targetDefinition, 'Study1-6_BioMotionBot'))
    if(targetNumber == 1)
        % alpha = 2*pi/4;
        R = [0, -1; 1, 0];

    elseif(targetNumber == 2)  
        % alpha = pi/4;
        R = [cos(pi/4), -sin(pi/4); sin(pi/4), cos(pi/4)];

    elseif(targetNumber == 3)
        % alpha = 0 = 8*pi/4;   % no rotation neccessary
        R = [1, 0; 0, 1];       % Identity

    elseif(targetNumber == 4)
        % alpha = 7*pi/4;
        R = [cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];

    elseif (targetNumber == 5)    
        % alpha = 6*pi/4;
        R = [0, 1; -1, 0];

    elseif(targetNumber == 6) 
        % alpha = 5*pi/4;
        R = [cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];

    elseif(targetNumber == 7)
        % alpha = 4*pi/4 = pi;
        R = [-1, 0; 0, -1];

    elseif(targetNumber == 8) 
        % alpha = 3*pi/4;
        R = [cos(3*pi/4), -sin(3*pi/4); sin(3*pi/4), cos(3*pi/4)];


    % Inward-directed movements are rotated and the z-value is multiplicated
    % with (-1) in order to be able to compare inward- and outward-directed 
    % movements because direction of force field is acting in opposing direction

    elseif(targetNumber == 9)
        % alpha = 2*pi/4;
        R = [1, 0; 0, -1]*[0, -1; 1, 0];

    elseif(targetNumber == 10)
        % alpha = pi/4;
        R = [1, 0; 0, -1]*[cos(pi/4), -sin(pi/4); sin(pi/4), cos(pi/4)];

    elseif(targetNumber == 11)
        % alpha = 0 = 8*pi/4;   % no rotation neccessary
        R = [1, 0; 0, -1]*[1, 0; 0, 1];       % Identity

    elseif(targetNumber == 12)   
        % alpha = 7*pi/4;
        R = [1, 0; 0, -1]*[cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];

    elseif (targetNumber == 13)
        % alpha = 6*pi/4;
        R = [1, 0; 0, -1]*[0, 1; -1, 0];

    elseif(targetNumber == 14)
        % alpha = 5*pi/4;
        R = [1, 0; 0, -1]*[cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];

    elseif(targetNumber == 15)
        % alpha = 4*pi/4 = pi;
        R = [1, 0; 0, -1]*[-1, 0; 0, -1];

    elseif(targetNumber == 16)
        % alpha = 3*pi/4;
        R = [1, 0; 0, -1]*[cos(3*pi/4), -sin(3*pi/4); sin(3*pi/4), cos(3*pi/4)];

    end
    
elseif(strcmp(targetDefinition, 'Study1-6_KINARM'))
    if(targetNumber == 5)
        % alpha = 2*pi/4;
        R = [0, -1; 1, 0];

    elseif(targetNumber == 4)
        % alpha = pi/4;
        R = [cos(pi/4), -sin(pi/4); sin(pi/4), cos(pi/4)];

    elseif(targetNumber == 3)
        % alpha = 0 = 8*pi/4;   % no rotation neccessary
        R = [1, 0; 0, 1];       % Identity

    elseif(targetNumber == 2)
        % alpha = 7*pi/4;
        R = [cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];

    elseif (targetNumber == 1)
        % alpha = 6*pi/4;
        R = [0, 1; -1, 0];

    elseif(targetNumber == 8)
        % alpha = 5*pi/4;
        R = [cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];

    elseif(targetNumber == 7)
        % alpha = 4*pi/4 = pi;
        R = [-1, 0; 0, -1];

    elseif(targetNumber == 6)
        % alpha = 3*pi/4;
        R = [cos(3*pi/4), -sin(3*pi/4); sin(3*pi/4), cos(3*pi/4)];

    % Inward-directed movements are rotated and the z-value is multiplicated
    % with (-1) in order to be able to compare inward- and outward-directed 
    % movements because direction of force field is acting in opposing direction

    elseif(targetNumber == 13)
        % alpha = 2*pi/4;
        R = [1, 0; 0, -1]*[0, -1; 1, 0];

    elseif(targetNumber == 12)
        % alpha = pi/4;
        R = [1, 0; 0, -1]*[cos(pi/4), -sin(pi/4); sin(pi/4), cos(pi/4)];

    elseif(targetNumber == 11)
        % alpha = 0 = 8*pi/4;   % no rotation neccessary
        R = [1, 0; 0, -1]*[1, 0; 0, 1];       % Identity

    elseif(targetNumber == 10)
        % alpha = 7*pi/4;
        R = [1, 0; 0, -1]*[cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];

    elseif (targetNumber == 9)
        % alpha = 6*pi/4;
        R = [1, 0; 0, -1]*[0, 1; -1, 0];

    elseif(targetNumber == 16)
        % alpha = 5*pi/4;
        R = [1, 0; 0, -1]*[cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];

    elseif(targetNumber == 15)
        % alpha = 4*pi/4 = pi;
        R = [1, 0; 0, -1]*[-1, 0; 0, -1];

    elseif(targetNumber == 14)
        % alpha = 3*pi/4;
        R = [1, 0; 0, -1]*[cos(3*pi/4), -sin(3*pi/4); sin(3*pi/4), cos(3*pi/4)];

    end
    
elseif(strcmp(targetDefinition, 'Study7_KINARM'))
       
    if(targetNumber == 3)
        % alpha = 7*pi/4;
        R = [cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];
    
    elseif (targetNumber == 2)
        % alpha = 6*pi/4;
        R = [0, 1; -1, 0];
       
    elseif(targetNumber == 1)
        % alpha = 5*pi/4;
        R = [cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];


    % Inward-directed movements are rotated and the z-value is multiplicated
    % with (-1) in order to be able to compare inward- and outward-directed 
    % movements because direction of force field is acting in opposing direction

    elseif(targetNumber == 13)
        % alpha = 7*pi/4;
        R = [1, 0; 0, -1]*[cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];
  
    elseif (targetNumber == 12)
        % alpha = 6*pi/4;
        R = [1, 0; 0, -1]*[0, 1; -1, 0];
       
    elseif(targetNumber == 11)
        % alpha = 5*pi/4;
        R = [1, 0; 0, -1]*[cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];

    end

	elseif(strcmp(targetDefinition, 'Study10_KINARM'))
    if(targetNumber == 5)
        % alpha = 2*pi/4;
        R = [0, -1; 1, 0];

    elseif(targetNumber == 4)
        % alpha = pi/4;
        R = [cos(pi/4), -sin(pi/4); sin(pi/4), cos(pi/4)];

    elseif(targetNumber == 3)
        % alpha = 0 = 8*pi/4;   % no rotation neccessary
        R = [1, 0; 0, 1];       % Identity

    elseif(targetNumber == 2)
        % alpha = 7*pi/4;
        R = [cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];

    elseif (targetNumber == 1)
        % alpha = 6*pi/4;
        R = [0, 1; -1, 0];

    elseif(targetNumber == 8)
        % alpha = 5*pi/4;
        R = [cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];

    elseif(targetNumber == 7)
        % alpha = 4*pi/4 = pi;
        R = [-1, 0; 0, -1];

    elseif(targetNumber == 6)
        % alpha = 3*pi/4;
        R = [cos(3*pi/4), -sin(3*pi/4); sin(3*pi/4), cos(3*pi/4)];
	end
end

% Allocation
rotXvalues = zeros(1,nTraj);
rotZvalues = zeros(1,nTraj);
rotation = [0;0];
dist = zeros(nTraj,1);

for i=1:nTraj
    rotation = R * transpose(trajectory(i,:));  % Rotation of trial point number i with the ankle alpha (to x-axis)
    rotXvalues(i) = rotation(1);
    rotZvalues(i) = rotation(2);
    
    if(rotXvalues(i) < 0)               % if trial point is out of range (behind origin) AND in positive z-direction
        dist(i) = sign(rotZvalues(i)) * sqrt((rotXvalues(i))^2 + (rotZvalues(i))^2);              % distance to origin using pythagorean law
    elseif(rotXvalues(i) > baseLength)  % if trial point is out of range (behind target)
        dist(i) = sign(rotZvalues(i)) * sqrt((rotXvalues(i)-baseLength)^2 + (rotZvalues(i))^2);   % distance to target point using pythagorean law after translation in direction of origin of 0.1m
    else
        dist(i) = rotZvalues(i);
    end

end

distance = dist;   % Return value

end