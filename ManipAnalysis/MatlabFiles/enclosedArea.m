function [area] = enclosedArea(posX,posY)

X = [transpose(posX),posX(1)];
Y = [transpose(posY),posY(1)];

area = polyarea(X,Y);