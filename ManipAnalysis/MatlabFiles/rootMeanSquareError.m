%Nicht mehr ben�tigt!
%===================================================================%   
%   Karlsruhe Institute of Technology (KIT)                         %
%   Department of Sport and Sport Science                           %
%   BioMotion Center                                                %
%   www.sport.kit.edu                                               %
%                                                                   %
%   Matthias P�schl                                                 %
%   Christian Stockinger, christian.stockinger@kit.edu              %
%                                                                   %
%   23.10.2012                                                      %
%===================================================================%

%   m-file for ManipAnalysis
%   rootMeanSquareError.m
%   
%   Description                                                     
%   This function calculates the root mean square error/deviation (RMSE)
%   between a given trial trajectory and its corresponding baseline
%   trajectory.
%
%   Arguments
%   - Input: Trial trajectory and its corresproding baseline trajectory)
%       
%   - Output: Root mean sqaure error/deviation 
%       

function RMSE = rootMeanSquareError(trajectory, baseline)

% Identify and check dimension of given points
[nTraj, pTraj] = size(trajectory);  
[nBase, pBase] = size(baseline);                    % Normally: n=101, p=2

if (nTraj ~= nBase)
    error('rootMeanSqaureError: ERROR - trial and baseline trajectory differ in dimension')
end

% trajectory(:,1) = measureDataX
% trajectory(:,2) = measureDataZ
% baseline(:,1) = baselineDataX
% baseline(:,2) = baselineDataZ


% Calculation of RMSE
RMSE = sqrt( mean( (trajectory(:,1) - baseline(:,1)).^2 + (trajectory(:,2) - baseline(:,2)).^2 ) );

end