velocity=800
lifetime=1500
coeff=0.002
g=30
d2r=0.0174533

function onTick()
	ind=input.getBool(1)
	sx=input.getNumber(1)
	sy=input.getNumber(2)
	sz=input.getNumber(6)
	tx=input.getNumber(3)
	ty=input.getNumber(4)
	tz=input.getNumber(7)
	cmps=input.getNumber(5)
	
	dx=tx-sx
	dy=ty-sy
	dz=tz-sz
	h=math.sqrt(dx*dx+dy*dy)
	
	a=0 b=0 c=0 e=0.01
	if ind then a=45 b=85
	else a=-10 b=45 end
	
	caz=(tx==0 and ty==0)
	
	if caz then
		output.setNumber(1, 0)
		output.setNumber(2, 0)
		return
	end
	
	while b-a>e*2 do
		c=(a+b)/2
	
		era=f(a,h,dz)
		erc=f(c,h,dz)
	
		if era*erc < 0 then
			b=c
		else
			a=c
		end
	end
	
	a=(a+b)*0.5
	
	yaw=(-math.atan(dy, dx) / (math.pi * 2)) + cmps + 0.25
	pitch=a/90
	output.setNumber(1, yaw)
	output.setNumber(2, pitch)
end
	
function f(ang,dst,dff)
	x=0
	y=0
	vx=math.cos(ang*d2r)*velocity
	vy=math.sin(ang*d2r)*velocity
	for i=0,lifetime-1 do
		x=x+(vx/60)
		y=y+(vy/60)
		
		vx=vx*(1-coeff)
		vy=vy*(1-coeff)
		vy=vy-(g/60)
		
		if x>=dst then
			break
		end
	end
	
	return y-dff
end
