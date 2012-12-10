%===================================================================%   
%   Karlsruhe Institute of Technology (KIT)                         %
%   Department of Sport and Sport Science                           %
%   BioMotion Center                                                %
%   www.sport.kit.edu                                               %
%                                                                   %
%   Matthias Pöschl                                                 %
%   Christian Stockinger, christian.stockinger@kit.edu              %
%                                                                   %
%   15.05.2012                                                      %
%===================================================================%

%   m-file for ManipAnalysis
%   timeNorm.m
%   
%   Description                                                     
%   This function time normalizes recorded data to a given number N of data
%   points by using cubic spline interpolation. 
%   Note that for analysis of BioMotionBot-data: data is recorded at a
%   frequency of approx. 100Hz = 1/100s = 0.01/s
%
%   Arguments
%   - Input:
%       inputTime = real time of recorded data 
%       inputData = vector/matrix contaoning recorded data (= one single
%       movement)
%       newSampleCount = the new sample count, (e.g normalization into 101
%       data points the input has to be N=100)
%   - Output:
%       normalizedTime = vector [0,1,...,N]
%       normalizedData = vector/matrix containing normalized data
%       errorvar = Contains error-information or 'No Errors!' if everything
%       is fine. 

function [errorvar, normalizedData, normalizedTime] = timeNorm(inputData, inputTime, newSampleCount)

errorvar = '';
localNewSampleCount = newSampleCount + 1 ;

[mData, nData] = size(inputData);           % dimension of input data matrix
normalizedData = zeros(localNewSampleCount,nData);  % allocation for normalized data
[mTime, nTime] = size(inputTime);           % dimension of input time matrix

if nTime ~= nData % Check for dimension errors
	errorvar = [errorvar 'Dimension Error!'];
	disp('Dimension Error!')

end
if (max(diff(inputTime))/10000) > 20   % Checks if the maximum difference between time increments is smaller than 20ms
										% Note: BioMotionBot data is recorded at a frequency of approx. 200Hz
	errorvar = [errorvar ['Difference between time increments too high! ( ' num2str(max(diff(inputTime))/10000) ' ms )']];
	disp (errorvar)   
end

dt = mean(abs(diff(inputTime))); % Gets the mean difference between time ticks = mean frequency

t0 = inputTime(1);
tEnd = inputTime(nTime);

deltaInputTime = nTime/localNewSampleCount;
tp = (t0:(dt*deltaInputTime):tEnd)';

while(length(tp) ~= newSampleCount)	% Checks if the normalized time array 'tp' has the required length. If it doesn't, it will be recalculated.
    if length(tp) < newSampleCount	
        localNewSampleCount = localNewSampleCount + 1;
    else
        localNewSampleCount = localNewSampleCount - 1;
    end
	
	deltaInputTime = nTime/localNewSampleCount;
	tp = (t0:(dt*deltaInputTime):tEnd)';
end

normalizedTime = tp;

% Time Normalization
normalizedData = interp1(inputTime, inputData, tp, 'spline');
