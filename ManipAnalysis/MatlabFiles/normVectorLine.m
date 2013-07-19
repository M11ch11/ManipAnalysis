%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   19.07.2013                                                          %
%=======================================================================%

%   normVectorLine.m for ManipAnalysis
%   
%   Description                                                     
%   This function receives a line segment and a force vector for this segment.
%   Then, the orthogonal force vector for this line segment ist calculated.
%
%   Arguments
%   - Input:
%       Vpos1 = Start vector array of the line segment (m by 2)
%       
%       Vpos2 = End vector array of the line segment (m by 2)
%       
%       Vforce = Force vector array for the given line segment (m by 2)
%
%   - Output:
%       VforcePD = Orthogonal force vector array for the line segment (m by 2)

function [VforcePD] = normVectorLine(Vpos1, Vpos2, Vforce)
[mVpos1, nVpos1] = size(Vpos1); 
[mVpos2, nVpos2] = size(Vpos2); 
[mVforce, nVforce] = size(Vforce); 

if (nVpos1 ~= 2 || nVpos1 ~= nVpos2 || nVpos1 ~=  nVforce || mVpos1 ~= mVpos2 || mVforce ~= mVpos1)
	disp('Dimension Error!')
    VforcePD = 0;
else
    VforcePD = zeros(mVpos1,nVpos1);
    
    % Calculate the direction vector ofd the line segment
    Vpos = (Vpos2 - Vpos1);
    
    for i = 1 : (mVpos1)
        % Calculate the absolute force of the force vector
        AbsForce = sqrt(Vforce(i,1)^2 + Vforce(i,2)^2);

        % Calculate the angle between the direction vector and the force vector
        AngleBetweenVectors = atan2(Vforce(i,2), Vforce(i,1)) - atan2(Vpos(i,2), Vpos(i,1));

        % Calculate the orthogonal force at the direction vector
        AbsForceOrtho = AbsForce * sin(AngleBetweenVectors);

        % Calculate the normalized orthogonal direction vector
        VposPDnorm = [-Vpos(i,2) Vpos(i,1)] / sqrt(Vpos(i,1)^2 + Vpos(i,2)^2);

        % Normalise orthogonal direction vector and resize it with the absolute orthogonal force
        VforcePD(i,:) = VposPDnorm * AbsForceOrtho;
    end
end
    