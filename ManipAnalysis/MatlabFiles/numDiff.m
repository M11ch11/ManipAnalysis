%=======================================================================%   
%   Karlsruhe Institute of Technology (KIT)                             %
%   Department of Sport and Sport Science                               %
%   BioMotion Center                                                    %
%   www.sport.kit.edu                                                   %
%                                                                       %
%   Matthias Pöschl                                                     %
%   Christian Stockinger, christian.stockinger@kit.edu                  %
%                                                                       %
%   18.01.2014                                                          %
%=======================================================================%

%   numDiff.m for ManipAnalysis
%   
%   Description                                                     
%   This function calculates the numeric differentiation (the result is the velocity).
%
%   Arguments
%   - Input:
%       position_data = 1xN vector containing the position-data
%       sampleRate = The number of samples per second
%   - Output:
%       position_data_diff = velocity vector

function [ position_data_diff ] = numDiff(position_data, sampleRate)

%Annahme: 
%- position_data (Positionsdaten) ist Zeilenvektor also 1xN (falls nicht, dann entsprechend , durch ; ersetzen, etc.)
%- die samplingRate ist konstant (äquidistante Schrittweiten)

sampleTime = double(1.0) / double(sampleRate);
N = length(position_data);

position_data_help1 = [position_data(1), position_data];	% = [position_data(1), position_data(1), position_data(2), position_data(3), ..., position_data(N-1), position_data(N)],  1x(N+1)-Vektor, vorne ersten Einrag doppeln
position_data_help2 = [position_data, position_data(N)];	% = [position_data(1), position_data(2), position_data(3), ..., position_data(N-1), position_data(N), position_data(N)],  1x(N+1)-Vektor, hinten letzten Einrag doppeln

% damit ist diff(x_help1) + diff(x_help2) = [x(2)-x(1), x(3)-x(1), x(4)-x(2),...,x(N-1)-X(N-3), x(N)-X(N-2), X(N)-x(N-1)];  1xN-Vektor
position_data_diff = ( diff(position_data_help1) + diff(position_data_help2) ) ./ ( sampleTime .* [1,2 .* ones(1,N-2),1] );	% 1xN-Vektor, erster und letzter Wert werden durch samplingRate geteilt, alle anderen durch doppelte Schrittweite.

end