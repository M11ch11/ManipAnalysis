%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   17.07.2013                                                          %
%=======================================================================%

%   normVectorLine.m for ManipAnalysis
%   
%   Description                                                     
%   This function receives a line segment and a force vector for this segment.
%   Then, the orthogonal force vector for this line segment ist calculated.
%
%   Arguments
%   - Input:
%       Vpos1 = Start vector of the line segment
%       
%       Vpos2 = End vector of the line segment
%       
%       Vforce = Force vector for the given line segment
%
%   - Output:
%       VforcePD = Orthogonal force vector for the line segment

function [VforcePD] = normVectorLine(Vpos1, Vpos2, Vforce)

% Calculate the absolute force of the force vector
AbsForce = sqrt(Vforce(1)^2 + Vforce(2)^2);

% Calculate the direction vector ofd the line segment
Vpos = (Vpos2 - Vpos1);

% Calculate the angle between the direction vector and the force vector
AngleBetweenVectors = atan2(Vforce(2), Vforce(1)) - atan2(Vpos(2), Vpos(1));

% Calculate the orthogonal force at the direction vector
AbsForceOrtho = AbsForce * sin(AngleBetweenVectors);

% Calculate the orthogonal direction vector
VposPD = [-Vpos(2) Vpos(1)];

% Normalise orthogonal direction vector and resize it with the absolute orthogonal force
VforcePD = (VposPD / sqrt(Vpos(1)^2 + Vpos(2)^2)) * AbsForceOrtho;