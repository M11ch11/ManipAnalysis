%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   09.10.2014                                                          %
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

function [ pd_force, sign_pd ] = pdForceLineSegment(force_vector, v_pos_1, v_pos_2)

position_vector = v_pos_2 - v_pos_1;
R = [0 -1; 1 0];
pd_position_vector = R * transpose(position_vector);

pd_force = dot(force_vector, pd_position_vector) * (pd_position_vector ./ norm(pd_position_vector));

cross_product = cross([position_vector 0], [force_vector 0]);
sign_pd = sign(cross_product(3));

end
