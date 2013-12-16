%========================================================================%   
%   Karlsruhe Institute of Technology (KIT)                              %
%   Department of Sport and Sport Science                                %
%   BioMotion Center                                                     %
%   www.sport.kit.edu                                                    %
%                                                                        %
%   Matthias P�schl                                                      %
%   Christian Stockinger, christian.stockinger@kit.edu                   %
%                                                                        %
%   12.11.2013                                                           %
%========================================================================%
%
%   m-file for ManipAnalysis
%   forcefieldCompensationFactor.m
%   
%   Description                                                     
%   Calculated the forcefield compensation factor.
%
%   Arguments
%   - Input:
%       position = X,Z position vector
%		velocity = X,Z velocity vector
%		forceIst = X,Z actual force vector
%		forceSoll = X,Z nominal force vector 
%		index = the index at wich the forcefield compensation factor should be calculated
%   - Output:
%       Forcefield compensation factor in percent
%
%========================================================================%
function [ffcf] = forcefieldCompensationFactor(position,velocity,forceIst,forceSoll,index)

fIstPD = pdForceLineSegment(forceIst(index,:), position(index,:), position(index + 1,:));
fSollPD = pdForceLineSegment(forceSoll(index,:), position(index,:), position(index + 1,:));

ffcf = sqrt(fIstPD(1)^2 + fIstPD(2)^2) / sqrt(fSollPD(1)^2 + fSollPD(2)^2) * 100.0;