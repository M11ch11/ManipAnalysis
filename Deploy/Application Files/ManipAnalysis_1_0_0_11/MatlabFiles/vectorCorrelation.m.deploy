function [corr] = vectorCorrelation(trialData,baselineData)

mean_trialData = mean(trialData);
mean_baselineData = mean(baselineData);
cov = mean(dot(trialData,baselineData,2)) - dot(mean_trialData,mean_baselineData);
var_trialData = mean(dot(trialData,trialData,2)) - dot(mean_trialData,mean_trialData);
var_baselineData = mean(dot(baselineData,baselineData,2)) - dot(mean_baselineData,mean_baselineData);
corr = cov / sqrt(var_trialData * var_baselineData);