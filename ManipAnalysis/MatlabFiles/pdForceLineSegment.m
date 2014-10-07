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

%   pdForceLineSegment.m for ManipAnalysis
%   
%   Description                                                     
%   This function receives a force vector of movement at a certain point.
%   The orthogonal force vector ist calculated with respect to the straight 
%   line joining start and target point.
%
%   Arguments
%   - Input:
%       v_pos_1 = 2d position vector of the beginning of the line segment
%       v_pos_2 = 2d position vector of the end of the line segment
%       force_vector = 2d force vector for the given line segment
%
%   - Output:
%       pd_force = Orthogonal force vector for the line segment

function [ pd_force ] = pdForceLineSegment(force_vector, v_pos_1, v_pos_2)

position_vector = v_pos_2 - v_pos_1;
theta = 90;
R = [cosd(theta) -sind(theta); sind(theta) cosd(theta)];
pd_position_vector = position_vector * R;

pd_force = ( dot(force_vector, pd_position_vector) / norm(pd_position_vector)^2 ) * pd_position_vector;
end
