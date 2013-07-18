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

%   normVectorLine.m for ManipAnalysis
%   
%   Description                                                     
%   This function rotates a force vektor so it alignes with a normal vektor
%   of a line segment.
%
%   Arguments
%   - Input:
%       Vpos1 = Vector of dimension N (=2) representating x and z-coordinates
%       of trajectory point 1 (first)
%       Vpos2 = Vector of dimension N (=2) representating x and z-coordinates
%       of trajectory point 2 (second)
%       Vforce = Vector of dimension N (=2) representating z-coordinates
%       of trajectory points
%   - Output:
%       VforcePD = length of the 2dim trajectory

function [VforcePD] = normVectorLine(Vpos1, Vpos2, Vforce)
%
%RotationMatrix90Degrees = [0 -1; 1 0];
%
%VposNorm = (Vpos2 - Vpos1) * RotationMatrix90Degrees ;

AbsForce = sqrt(Vforce(1)^2 + Vforce(2)^2);

Vpos = (Vpos2 - Vpos1);

AngleBetweenVectors = atan2(Vforce(2), Vforce(1)) - atan2(Vpos(2), Vpos(1));

AbsForceOrtho = AbsForce * sin(AngleBetweenVectors);

VforcePD = ([-Vpos(2) Vpos(1)] / sqrt(Vpos(1)^2 + Vpos(2)^2)) * AbsForceOrtho;