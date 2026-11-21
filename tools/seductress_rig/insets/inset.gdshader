shader_type canvas_item;
// [Claude 2026-11-21] Hole inset with a straight canal (womb, ass).  One picture, no bones: the canal is stretched
// around the cock, the cock is drawn here, and the fill level and the cum spurt are drawn here too.
// The game sets only:  depth, fill, girth, spurt  (the scene's animations move depth and spurt).
// profile: per picture row, R = canal centre x - 512 + 128, G = canal half-width * 2   (made by make_profile.py)
// mask:    white where the picture is the inside of the canal / cavity
// cock_prof: per row of the cock picture, R = its half-width in its own px
uniform sampler2D profile : filter_linear, repeat_disable;
uniform sampler2D mask : filter_linear, repeat_disable;
uniform sampler2D cock_prof : filter_linear, repeat_disable;
uniform sampler2D cock_tex : filter_linear, repeat_disable;
// ---- set by the game / the animations ----
uniform float depth = -1.0;        // -1 = cock out of the picture, 0 = tip just inside, 1 = fully in
uniform float fill = 0.0;          // 0..1 = how full the hole is (proportional)
uniform float girth = 1.0;         // cock thickness: thin 0.7, normal 1.0, thick 1.35, massive 1.8
uniform float spurt = 0.0;         // 0..1 = cum jet from the tip (small spurt about 0.45, big one 1.0)
uniform float cock_alpha = 0.86;
// ---- fixed per picture (set in the scene) ----
uniform vec3 tip_at = vec3(990.0, 815.0, 690.0);   // picture y of the tip at depth -1, 0, 1
uniform vec2 fill_at = vec2(705.0, 505.0);         // picture y of the fill surface at fill 0 and 1
uniform float xc = 512.0;          // cock centre line
uniform float entrance_y = 832.0;  // the opening; outside it nothing stretches
uniform float cock_scale = 0.44;
uniform vec2 cock_size = vec2(333.0, 928.0);
uniform float wall = 5.0;          // px: the opened canal's wall is a band this thick, right on the cock's edge
uniform float soft = 14.0;         // px: how gently the walls start to part
uniform float jet_len = 120.0;     // px: length of a full spurt
uniform float jet_w = 6.0;         // px: its half-width at the tip
uniform vec3 inner_col : source_color = vec3(0.78, 0.28, 0.26);  // inside of the opened canal
uniform vec3 rim_col : source_color = vec3(0.60, 0.16, 0.16);    // its walls (a darker line)
uniform vec4 fill_col : source_color = vec4(0.98, 0.96, 0.9, 0.92);
float soft_pos(float v) { return 0.5 * (v + sqrt(v * v + soft * soft)); }
void fragment() {
	vec2 p = UV / TEXTURE_PIXEL_SIZE;
	float tip_y = depth < 0.0 ? mix(tip_at.y, tip_at.x, min(-depth, 1.0)) : mix(tip_at.y, tip_at.z, min(depth, 1.0));
	vec4 pr = texture(profile, vec2(0.5, UV.y));
	float c = 512.0 + pr.r * 255.0 - 128.0;
	float w0 = pr.g * 127.5;
	float t = (p.y - tip_y) / cock_scale;                       // row of the cock picture at this picture row
	float r = t > 0.0 ? texture(cock_prof, vec2(0.5, clamp(t / cock_size.y, 0.0, 1.0))).r * 255.0 * cock_scale * girth + wall : 0.0;
	r *= smoothstep(-10.0, 30.0, t * cock_scale);               // the walls part a little ahead of full width
	r *= 1.0 - smoothstep(entrance_y - 4.0, entrance_y + 40.0, p.y);
	float push = max(soft_pos(r - w0) - soft_pos(-w0), 0.0);    // how much wider than at rest; 0 with no cock
	float w = w0 + push;
	float cen = mix(c, xc, smoothstep(0.0, 10.0, push));        // open canal sits on the cock, closed one where drawn
	float d = p.x - cen, ad = abs(d), s = sign(d);
	float far = w * 2.4 + 130.0;
	float sx;
	if (ad <= w) sx = c + d * w0 / w;
	else {
		float u = clamp((ad - w) / (far - w), 0.0, 1.0);
		float shift = (cen + s * w) - (c + s * w0);
		sx = p.x - shift * (1.0 - u) * (1.0 - u) * (1.0 - u);    // eases to nothing: no visible edge to the push
	}
	vec2 q = vec2(sx, p.y) * TEXTURE_PIXEL_SIZE;
	vec4 col = texture(TEXTURE, q);
	// the opened canal is drawn as a clean tube (the stretched drawing alone smears and hides behind the cock)
	float k = smoothstep(2.0, 12.0, push) * (1.0 - smoothstep(w - 1.5, w + 0.5, ad));
	col.rgb = mix(col.rgb, mix(inner_col, rim_col, smoothstep(w - wall - 1.0, w - wall + 0.5, ad)), k);
	// fill level
	float m = texture(mask, q).r;
	float surf = mix(fill_at.x, fill_at.y, clamp(fill, 0.0, 1.0)) + sin(p.x * 0.09 + TIME * 5.0) * 2.0;
	float f = m * smoothstep(surf - 1.5, surf + 1.5, p.y) * (1.0 - smoothstep(fill_at.x - 2.0, fill_at.x + 6.0, p.y)) * smoothstep(0.0, 0.02, fill);
	col.rgb = mix(col.rgb, fill_col.rgb, f * fill_col.a);
	// the cock
	vec2 uv = vec2(0.5 + (p.x - xc) / (cock_scale * girth * cock_size.x), (t + 4.0) / cock_size.y);
	if (uv.x > 0.0 && uv.x < 1.0 && uv.y > 0.0 && uv.y < 1.0) {
		vec4 ck = texture(cock_tex, uv);
		col.rgb = mix(col.rgb, ck.rgb, ck.a * cock_alpha);
	}
	// cum jet ahead of the tip
	if (spurt > 0.001) {
		float a = tip_y - p.y;
		float len = jet_len * (0.35 + 0.65 * spurt);
		float jw = jet_w * sqrt(girth) * (0.7 + 0.9 * clamp(a / len, 0.0, 1.0)) * (1.0 + 0.3 * sin(a * 0.16 - TIME * 30.0));
		float off = abs(p.x - xc - sin(a * 0.07 + TIME * 18.0) * 3.0 * clamp(a / 40.0, 0.0, 1.0));
		float j = smoothstep(-3.0, 3.0, a) * (1.0 - smoothstep(len * 0.7, len, a)) * (1.0 - smoothstep(jw - 1.5, jw + 1.5, off));
		col.rgb = mix(col.rgb, fill_col.rgb, j * min(spurt * 2.5, 1.0) * fill_col.a);
	}
	COLOR = col;
}
