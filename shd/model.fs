$input v_pos, v_view, v_normal, v_uv

#include "bgfx_shader.sh"

uniform vec4 u_color;

vec2 blinn(vec3 _lightDir, vec3 _normal, vec3 _viewDir) {
	float ndotl = dot(_normal, _lightDir);
	vec3 reflected = _lightDir - 2.0*ndotl*_normal; // reflect(_lightDir, _normal);
	float rdotv = dot(reflected, _viewDir);
	return vec2(ndotl, rdotv);
}

float fresnel(float _ndotl, float _bias, float _pow) {
	float facing = (1.0 - _ndotl);
	return max(_bias + (1.0 - _bias) * pow(facing, _pow), 0.0);
}

vec4 lit(float _ndotl, float _rdotv, float _m) {
	float diff = max(0.0, _ndotl);
	float spec = step(0.0, _ndotl) * max(0.0, _rdotv * _m);
	return vec4(1.0, diff, spec, 1.0);
}

void main() {
	vec3 lightDir = vec3(0.0, 0.0, 1.0);
	vec3 normal = normalize(v_normal);
	vec3 view = normalize(v_view);
	vec2 bln = blinn(lightDir, normal, view);
	vec4 lc = lit(bln.x, bln.y, 1.0);
	float fres = fresnel(bln.x, 0.2, 5.0);

	vec3 color = u_color.xyz;

	gl_FragColor.xyz = pow(color*lc.y + fres*pow(lc.z, 64.0), vec3_splat(1.0/2.2) );
	gl_FragColor.w = 1.0;
}
