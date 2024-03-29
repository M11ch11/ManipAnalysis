%========================================================================%   
%   Karlsruhe Institute of Technology (KIT)                              %
%   Department of Sport and Sport Science                                %
%   BioMotion Center                                                     %
%   www.sport.kit.edu                                                    %
%                                                                        %
%   Matthias P�schl                                                      %
%   Christian Stockinger, christian.stockinger@kit.edu                   %
%                                                                        %
%   11.12.2015                                                           %
%========================================================================%
%
%   m-file for ManipAnalysis
%   distanceToCurve.m
%   
%   Description                                                     
%   This function calculates the minimal perpendicular distance for each
%   point of a given set of points (trial trajectory) to the corresponding 
%   reference trajectory (straight line joining start and target points).
%
%   Arguments
%   - Input:
%       trajectory = Set of points of a trajectory in 2-dimensional space
%           given as a n x 2 matrix. Usually, for ManipAnalysis this is a 
%           time normalized 101 x 2 matix. 
%           Thereby, each row of this matrix represents the 2-dimensional 
%           coordinates of a single point.
%       startPoint = coorrdinates of origin [1x2 matrix]
%       endPoint = coorrdinates of target [1x2 matrix]
%       forceFieldMatrix = 2x2 matrix describing the forceFieldMatrix for this trial.
%						   If the forceFieldMatrix is zero, a CCW forceFieldMatrix is assumed.
%   - Output:
%       distance = vector containing all absolute perpendicular distances 
%           of given points to the line connecting start and end point 
%
%		sign_pd = [1] when the point is located in anti clockwise to the movement direction,
%				  [-1] when clockwise
%
%		sign_ff = [-1] when the point is located in the direction of the forceFieldMatrix,
%				  [1] when in the opposite direction
%
%========================================================================%

function [distance, sign_pd, sign_ff] = distanceToCurve(trajectory, startPoint, endPoint, forceFieldMatrix)

% Check for errors: number of input arguments
if (nargin ~= 4)
    error('distanceToCurve: ERROR - invalid input arguments - 4 input arguments needed!')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of startPoint argument
[nTraj, pTraj] = size(startPoint);  % in general: nTraj=1, pTraj=2

if (pTraj ~= 2)
    error('distanceToCurve: ERROR - startPoint must live in 2-dimensional space.')
end

if (nTraj ~= 1)
    error('distanceToCurve: ERROR - startPoint must contain data points.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of endPoint argument
[nTraj, pTraj] = size(endPoint);  % in general: nTraj=1, pTraj=2

if (pTraj ~= 2)
    error('distanceToCurve: ERROR - endPoint must live in 2-dimensional space.')
end

if (nTraj ~= 1)
    error('distanceToCurve: ERROR - endPoint must contain data points.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of forceFieldMatrix argument
[nTraj, pTraj] = size(forceFieldMatrix);  % in general: nTraj=2, pTraj=2

if (pTraj ~= 2)
    error('distanceToCurve: ERROR - forceFieldMatrix must be a 2x2 matrix.')
end

if (nTraj ~= 2)
    error('distanceToCurve: ERROR - forceFieldMatrix must be a 2x2 matrix.')
end

%-------------------------------------------------------------------------%
% Identify and check dimension of trajectory argument
[nTraj, pTraj] = size(trajectory);  % in general: nTraj=101, pTraj=2

if (pTraj ~= 2)
    error('distanceToCurve: ERROR - trajectory must live in 2-dimensional space.')
end

if (nTraj < 1)
    error('distanceToCurve: ERROR - trajectory must contain data points.')
end

%-------------------------------------------------------------------------%
trajectory = double(trajectory);    % ensure trajectory entries are doubles
startPoint = double(startPoint);    % ensure startPoint entries are doubles
endPoint = double(endPoint);    % ensure endPoint entries are doubles
forceFieldMatrix = double(forceFieldMatrix);    % ensure forceFieldMatrix entries are doubles
%-------------------------------------------------------------------------%
if norm(forceFieldMatrix) == 0 % Null force field
	forceFieldMatrix = [0 -1; 1 0]; % Dummy CCW force field
end
%-------------------------------------------------------------------------%

dist = zeros(nTraj,1);
sign_pd = zeros(nTraj,1);
sign_ff = zeros(nTraj,1);

for i=1:nTraj
    absDistancePointToStart = abs(pdist2(trajectory(i,:), startPoint, 'euclidean'));
    absDistancePointToEnd = abs(pdist2(trajectory(i,:), endPoint, 'euclidean'));
	absDistanceStartToEnd = abs(pdist2(startPoint, endPoint, 'euclidean'));
	
	% Checking if current datapoint is behind the startPoint, endPoint or inbetween.
	% For this, the Pythagorean theorem for obtuse triangles is used.
	if((absDistanceStartToEnd^2 + absDistancePointToStart^2) < absDistancePointToEnd^2)   
		% The current datapoint is behind the startPoint
		dist(i) = absDistancePointToStart;	
	elseif ((absDistanceStartToEnd^2 + absDistancePointToEnd^2) < absDistancePointToStart^2) 
		% The current datapoint is behind the endPoint
		dist(i) = absDistancePointToEnd;
    else
        % The current datapoint is between the startPoint and endPoint
        startPoint3D = [startPoint 0];
        endPoint3D = [endPoint 0];
        currentPoint3D = [trajectory(i,:) 0];

        dist(i) = norm(cross(cross((startPoint3D - currentPoint3D), (endPoint3D - currentPoint3D)), (endPoint3D - startPoint3D))) / norm(endPoint3D - startPoint3D) ^ 2;
    end
    
    %-------------------------------------------------------------------------%
    sign_pd(i) = 0;
    pd_matrix = [0 -1; 1 0]; % CCW
    start_point_vector = startPoint - trajectory(i,:);
    start_end_vector = startPoint - endPoint;
    pd_position_vector = pd_matrix * transpose(start_end_vector);

    pd_angle = rad2deg(atan2(norm(cross([start_point_vector 0],[transpose(pd_position_vector) 0])),dot(start_point_vector, pd_position_vector)));

    if (pd_angle > 90)
        sign_pd(i) = -1;
    else
        sign_pd(i) = 1;
    end
    %-------------------------------------------------------------------------%
    sign_ff(i) = 0;
    start_point_vector = startPoint - trajectory(i,:);
    start_end_vector = startPoint - endPoint;
    ff_position_vector = forceFieldMatrix * transpose(start_end_vector);
    ff_angle = rad2deg(atan2(norm(cross([start_point_vector 0],[transpose(ff_position_vector) 0])),dot(start_point_vector, ff_position_vector)));

    if (ff_angle > 90)
        sign_ff(i) = 1;
    else
        sign_ff(i) = -1;
    end
    %-------------------------------------------------------------------------%
end

distance = dist;   % Return value

end