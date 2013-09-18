%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   22.07.2013                                                          %
%=======================================================================%

%   normVectorLine_CS.m for ManipAnalysis
%   
%   Description                                                     
%   This function receives a force vector of movement at a certain point.
%   The orthogonal force vector ist calculated with respect to the straight 
%   line joining start and target point.
%
%   Arguments
%   - Input:
%       TargetNumber = Number of appearing target (1,2,...,16)
%       
%       Vforce = 2d force vector for the given line segment 
%       (x-z-Plane, Maipulandum Coordinate System)
%
%   - Output:
%       VforcePD = Orthogonal force vector for the line segment

function [VforcePD] = normVectorLine_CS(targetNumber, Vforce)

% Determine angle between center point and target point
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
    R = (-1)*[0, -1; 1, 0];

elseif(targetNumber == 10)
    % alpha = pi/4;
    R = (-1)*[cos(pi/4), -sin(pi/4); sin(pi/4), cos(pi/4)];
 
elseif(targetNumber == 11)
    % alpha = 0 = 8*pi/4;   % no rotation neccessary
    R = (-1)*[1, 0; 0, 1];  % Identity

elseif(targetNumber == 12)
    % alpha = 7*pi/4;
    R = (-1)*[cos(7*pi/4), -sin(7*pi/4); sin(7*pi/4), cos(7*pi/4)];
    
elseif (targetNumber == 13)
    % alpha = 6*pi/4;
    R = (-1)*[0, 1; -1, 0];
        
elseif(targetNumber == 14)
    % alpha = 5*pi/4;
    R = (-1)*[cos(5*pi/4), -sin(5*pi/4); sin(5*pi/4), cos(5*pi/4)];
    
elseif(targetNumber == 15)
    % alpha = 4*pi/4 = pi;
    R = (-1)*[-1, 0; 0, -1];
    
elseif(targetNumber == 16)
    % alpha = 3*pi/4;
    R = (-1)*[cos(3*pi/4), -sin(3*pi/4); sin(3*pi/4), cos(3*pi/4)];    
end

rotVforce = [];

rotVforce = R * Vforce;  % Rotation of force point with the ankle alpha (to x-axis)

VforcePD = rotVforce(2);
