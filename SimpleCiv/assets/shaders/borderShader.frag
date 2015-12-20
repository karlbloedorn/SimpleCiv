#version 440
in vec3 pass_Position;
uniform vec3 borderColor;
uniform vec3 S0;
uniform vec3 S1;

out vec4 out_Color;

float mydist(){
	vec3 v = S1 - S0;
    vec3 w = pass_Position - S0;

    float c1 = dot(w,v);
    if ( c1 <= 0 )
        return distance(pass_Position,S0);

    float c2 = dot(v,v);
    if ( c2 <= c1 )
        return distance(pass_Position, S1);

    float b = c1 / c2;
    vec3 Pb = S0 + b * v;

	return  distance(pass_Position, Pb);
}

void main(void) {
	 //distance(pass_Position, vec3(0,0,0));
	float pos = 1-mydist();

	if(pos > 0.97){
		out_Color = vec4(0.0,0.0,0.0,0.0);
	} else if(pos > 0.93){
	   out_Color =  vec4(borderColor, 0.6);
	   //out_Color =  vec4(1.0,1.0,1.0, 0.6);

	} else if(pos > 0.6) {
		out_Color =  vec4(borderColor, pos/6.0);
	} else {
		out_Color =  vec4(borderColor, 0.0);
	}
}