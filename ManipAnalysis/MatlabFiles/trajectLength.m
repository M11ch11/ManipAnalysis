%Brauchen wir nicht mehr!
%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   12.05.2012                                                          %
%=======================================================================%

%   trajectLength.m for ManipAnalysis
%   
%   Description                                                     
%   This function calculates the length of a 2dim trajectory by
%   consicutively summing up the distance of each point to its neighbour
%   point. The distance is computed using the 2-Norm = Euclidean Norm.
%   This file is constructed for time normalized data (commonly N=101).
%
%   Arguments
%   - Input:
%       posX = Vector of dimension N (=101) representating x-coordinates
%       of trajectory points
%       posY = Vector of dimension N (=101) representating y-coordinates
%       of trajectory points
%   - Output:
%       length = length of the 2dim trajectory

function [length] = trajectLength(posX, posY)

[mX, nX] = size(posX);     % posx & posY are supposed to be column vectors
[mY, nY] = size(posY);     % in case of time normalization: [mX,nX]=[mZ,nZ]=[101,1]
counter = 0;

% check for valid dimensions
if (mX ~= mY)
    error('dimension error (invalid number of rows)')
elseif (nX ~= nY)
    error('dimension error (invalid number of columns)')
elseif (nX > 1)
    error('dimension error (invalid numeber of columns)')
end

% calculation of trajectory length 
% by summing up the distance of each point to the next point
for i = 1 : (mX-1)
    counter = counter + norm( [posX(i+1),posY(i+1)] - [posX(i),posY(i)] , 2 );
end

length = counter;