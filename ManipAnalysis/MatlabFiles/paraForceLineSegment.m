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

%   normVectorLine_CS.m for ManipAnalysis
%   
%   Description                                                     
%   This function receives a force vector of movement at a certain point.
%   The parallel force vector ist calculated with respect to the straight 
%   line joining start and target point.
%
%   Arguments
%   - Input:
%       v_pos_1 = 2d position vector of the beginning of the line segment
%       v_pos_2 = 2d position vector of the end of the line segment
%       force_vector = 2d force vector for the given line segment      
%
%   - Output:
%       para_force = Parallel force vector for the line segment

function [ para_force ] = paraForceLineSegment(force_vector, v_pos_1, v_pos_2)

position_vector = v_pos_2 - v_pos_1;
theta = 0;
R = [cosd(theta) -sind(theta); sind(theta) cosd(theta)];
para_position_vector = position_vector * R;

para_force = ( dot(force_vector, para_position_vector) / norm(para_position_vector)^2 ) * para_position_vector;
end
