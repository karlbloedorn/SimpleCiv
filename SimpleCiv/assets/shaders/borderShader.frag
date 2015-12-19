#version 440
in vec3 pass_Position;
uniform vec3 gradientStart;
uniform vec3 gradientEnd;
uniform vec3 borderColor;
uniform vec3 cornerLocation;

out vec4 out_Color;

void main(void) {
	float pos = 0.0;
	
	if( gradientStart.x < gradientEnd.x){
		if(pass_Position.x > gradientStart.x && pass_Position.x < gradientEnd.x){
			pos = (gradientEnd.x - pass_Position.x) / (gradientEnd.x - gradientStart.x);
		}
	} else {
		if(pass_Position.x > gradientEnd.x && pass_Position.x <  gradientStart.x){
			pos = 1-(gradientStart.x - pass_Position.x) / (gradientStart.x - gradientEnd.x);
		}
	}

	if( gradientStart.z < gradientEnd.z){
		if(pass_Position.z > gradientStart.z && pass_Position.z < gradientEnd.z){
			pos = 1- (gradientStart.z - pass_Position.z) / (gradientStart.z - gradientEnd.z);  ///
		}
	} else {
		if(pass_Position.z > gradientEnd.z && pass_Position.z < gradientStart.z){
			pos = (gradientEnd.z - pass_Position.z) / (gradientEnd.z - gradientStart.z);
		}
	}

	if (cornerLocation.x < 1.1){
		float dist =  distance(pass_Position, cornerLocation);
		if(dist < 0.5){
			pos = 1- (dist / 0.5); 
		}
	}
	if(pos > 0.95){
		out_Color = vec4(0.0,0.0,0.0,0.0);
	} else if(pos > 0.90){
	   out_Color =  vec4(borderColor, 0.6);
	   //out_Color =  vec4(1.0,1.0,1.0, 0.6);

	} else if(pos > 0.2) {
		out_Color =  vec4(borderColor, pos/3.0);
	} else {
		out_Color =  vec4(borderColor, 0.0);
	}
}