function drawCircle(radius,xpos,ypos)
[x,y,z] = cylinder(radius,200);
plot(x(1,:)+xpos,y(1,:)+ypos,'Color','red','LineWidth',2);