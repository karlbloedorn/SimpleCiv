#version 440
out vec4 out_Color;
in vec3 pass_Normal;
in vec3 vertPos;

//uniform vec3 lightPos;
uniform vec3 ambientColor;
uniform vec3 diffuseColor;
uniform vec3 specColor;
uniform float shininess;

const float screenGamma = 2.2; 
const vec3 lightPos = vec3(1.0,0.0,0.0);

void main(void) {

  vec3 normal = normalize(pass_Normal);
  vec3 lightDir = normalize(lightPos - vertPos);

  float lambertian = max(dot(lightDir,normal), 0.0);
  float specular = 0.0;

  if(lambertian > 0.0) {
    vec3 viewDir = normalize(-vertPos);
    vec3 halfDir = normalize(lightDir + viewDir);
    float specAngle = max(dot(halfDir, normal), 0.0);
    specular = pow(specAngle, shininess);
  }
  vec3 colorLinear = ambientColor +
                     lambertian * diffuseColor +
                     specular * specColor;
  vec3 colorGammaCorrected = pow(colorLinear, vec3(1.0/screenGamma));
  out_Color = vec4(colorGammaCorrected, 1.0);
}

/*
	vec3 ct,cf;
    vec4 texel;
    float intensity,at,af;
    intensity = max(dot(lightPos,normalize(pass_Normal)),0.0);
    cf = intensity * diffuseColor.rgb +  ambientColor.rgb;
    af = 1.0;//diffuseColor.a;
    ct = vec3(0.5,0.5,0.5);
    at = 1.0;
    out_Color = vec4(ct * cf, 1.0);	*/