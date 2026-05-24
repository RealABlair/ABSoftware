data={}
data[0]={800,120,0.005}
data[1]={1000,150,0.02}
data[2]={1000,300,0.01}
data[3]={900,600,0.005}
data[4]={800,1500,0.002}
data[5]={700,2400,0.001}
data[6]={600,2400,0.0005}

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
	wt=property.getNumber("Weapon Type")
	velocity=data[wt][1]
	lifetime=data[wt][2]
	coeff=data[wt][3]
	
	dx=tx-sx
	dy=ty-sy
	dz=tz-sz
	h=math.sqrt(dx*dx+dy*dy)
	
	a=0 b=0 c=0 e=0.01
	if ind then a=45 b=90
	else a=-10 b=45 end
	
	caz=(tx==0 and ty==0)
	
	if caz then
		output.setNumber(1, 0)
		output.setNumber(2, 0)
		output.setNumber(3, -1)
		return
	end
	ttf=0
	
	while b-a>e*2 do
		c=(a+b)*0.5
	
		era=f(a,h,dz)
		erc=f(c,h,dz)
	
		if era*erc < 0 then
			b=c
		else
			a=c
		end
	end
	
	a=(a+b)*0.5
	f(a,h,dz)
	yaw=(-math.atan(dy, dx) / (math.pi * 2)) + cmps + 0.25
	pitch=a/90
	
	output.setNumber(1, ((yaw + 0.5) % 1.0) - 0.5)
	output.setNumber(2, pitch)
	output.setNumber(3, ttf)
end
	
function f(ang,dst,dff)
	x=0
	y=0
	vx=math.cos(ang*d2r)*velocity
	vy=math.sin(ang*d2r)*velocity
	flag=false
	for i=1,lifetime do
		x=x+(vx/60)
		y=y+(vy/60)
		
		vx=vx*(1-coeff)
		vy=vy*(1-coeff)
		vy=vy-(g/60)
		
		if x>=dst then
			ttf=i
			flag=true
			break
		end
	end
	
	if flag then return y-dff else return -9999 end
end
