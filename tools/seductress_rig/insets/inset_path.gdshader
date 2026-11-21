shader_type canvas_item;
// [Claude 2026-11-21] Hole inset whose canal is curved (mouth / throat).  Same idea and same controls as
// inset.gdshader, but everything is measured along a centre path: the throat bulges around the cock, and the cock
// is drawn bent along the path.
// The game sets only:  depth, fill, girth, spurt  (the scene's animations move depth and spurt).
uniform vec2 pts[360];             // centre path, points 4 px apart (made by make_path.py)
uniform int n = 2;
uniform float step_len = 4.0;
uniform sampler2D profile : filter_linear, repeat_disable;   // per path point: R, G = inside half-width * 2 on either side of the path; B = 1 where there is a throat
uniform sampler2D mask : filter_linear, repeat_disable;      // white = inside of the throat (for the fill)
uniform sampler2D cock_prof : filter_linear, repeat_disable;
uniform sampler2D cock_tex : filter_linear, repeat_disable;
// ---- set by the game / the animations ----
uniform float depth = -1.0;        // -1 = cock out of the picture, 0 = tip at the lips, 1 = fully down the throat
uniform float fill = 0.0;          // 0..1 = how full (proportional)
uniform float girth = 1.0;         // cock thickness: thin 0.7, normal 1.0, thick 1.35, massive 1.8
uniform float spurt = 0.0;         // 0..1 = cum jet from the tip
uniform float cock_alpha = 0.86;
// ---- fixed per picture (set in the scene) ----
uniform vec3 tip_at = vec3(150.0, 640.0, 1165.0);  // distance along the path of the tip at depth -1, 0, 1
uniform vec2 fill_at = vec2(1015.0, 420.0);        // picture y of the fill surface at fill 0 and 1
uniform float cock_scale = 0.9;
uniform vec2 cock_size = vec2(333.0, 928.0);
uniform float margin = 1.5;       // px between the cock's edge and the wall: the flesh touches it
uniform float hug_len = 50.0;     // behind the tip, a throat wider than the cock closes onto it over this length
uniform float soft = 8.0;
uniform float reach = 150.0;       // how far outside the wall the bulge fades out
uniform float jet_len = 150.0;
uniform float jet_w = 9.0;
uniform vec4 fill_col : source_color = vec4(0.98, 0.96, 0.9, 0.92);
uniform vec2 shaft_rows = vec2(180.0, 600.0);   // rows of the cock picture that are plain shaft
uniform float shaft_stretch = 1.8;              // the shaft is drawn this much longer, so it can reach down the throat
float cock_row(float t) {                       // distance from the tip (in picture px) -> row of the cock picture
	float a = shaft_rows.x, b = shaft_rows.y, len = (b - a) * shaft_stretch;
	return t < a ? t : (t < a + len ? a + (t - a) / shaft_stretch : b + (t - a - len));
}
float soft_pos(float v) { return 0.5 * (v + sqrt(v * v + soft * soft)); }
void fragment() {
	vec2 p = UV / TEXTURE_PIXEL_SIZE;
	float tip_s = depth < 0.0 ? mix(tip_at.y, tip_at.x, min(-depth, 1.0)) : mix(tip_at.y, tip_at.z, min(depth, 1.0));
	float best = 1e9, s = 0.0; vec2 cp = p, tan = vec2(0.0, 1.0);
	for (int i = 0; i < n - 1; i++) {
		vec2 a = pts[i], ab = pts[i + 1] - a;
		float h = clamp(dot(p - a, ab) / dot(ab, ab), 0.0, 1.0);
		vec2 c = a + ab * h; float dd = distance(p, c);
		if (dd < best) { best = dd; s = (float(i) + h) * step_len; cp = c; tan = normalize(ab); }
	}
	float total = float(n - 1) * step_len;
	float ad = best;
	vec2 dir = ad > 0.01 ? (p - cp) / ad : vec2(0.0);
	float side = sign(tan.x * (p.y - cp.y) - tan.y * (p.x - cp.x));
	vec4 pr = texture(profile, vec2(0.5, (s / step_len + 0.5) / float(n)));
	float w0 = (side > 0.0 ? pr.r : pr.g) * 127.5;              // where the inside of the wall is drawn, this side
	float behind = tip_s - s;                                    // px behind the tip (negative = ahead of it)
	float t = cock_row(behind / cock_scale);                     // row of the cock picture here (tip = row 0)
	bool on = behind > 0.0 && t < cock_size.y;
	float r = on ? texture(cock_prof, vec2(0.5, clamp(t / cock_size.y, 0.0, 1.0))).r * 255.0 * cock_scale * girth + margin : 0.0;
	float push = max(soft_pos(r - w0) - soft_pos(-w0), 0.0);    // cock wider than the throat: the wall is pushed out to its edge
	float w = w0 + push;
	if (on && r < w0) w = mix(w, r, pr.b * smoothstep(0.0, hug_len, behind));   // narrower: the wall closes onto it
	float sh = w - w0;
	float ad2;
	if (ad <= w) ad2 = ad * w0 / w;
	else { float u = clamp((ad - w) / (reach + 3.5 * abs(sh)), 0.0, 1.0); ad2 = ad - sh * (1.0 - u) * (1.0 - u) * (1.0 - u); }
	vec2 q = (cp + dir * ad2) * TEXTURE_PIXEL_SIZE;
	vec4 col = texture(TEXTURE, q);
	// fill level
	float m = texture(mask, q).r;
	float surf = mix(fill_at.x, fill_at.y, clamp(fill, 0.0, 1.0)) + sin(p.x * 0.09 + TIME * 5.0) * 2.0;
	float f = m * smoothstep(surf - 1.5, surf + 1.5, p.y) * (1.0 - smoothstep(fill_at.x - 2.0, fill_at.x + 6.0, p.y)) * smoothstep(0.0, 0.02, fill);
	col.rgb = mix(col.rgb, fill_col.rgb, f * fill_col.a);
	bool inpath = s > 0.5 && s < total - 0.5;
	// the cock, bent along the path
	if (inpath && on) {
		vec2 uv = vec2(0.5 + side * ad / (cock_scale * girth * cock_size.x), t / cock_size.y);
		if (uv.x > 0.0 && uv.x < 1.0) { vec4 ck = texture(cock_tex, uv); col.rgb = mix(col.rgb, ck.rgb, ck.a * cock_alpha); col.a = max(col.a, ck.a * cock_alpha); }
	}
	// cum jet ahead of the tip, along the path
	if (spurt > 0.001 && inpath) {
		float a = -behind;
		float len = jet_len * (0.35 + 0.65 * spurt);
		float jw = jet_w * sqrt(girth) * (0.7 + 0.9 * clamp(a / len, 0.0, 1.0)) * (1.0 + 0.3 * sin(a * 0.16 - TIME * 30.0));
		float off = abs(side * ad - sin(a * 0.07 + TIME * 18.0) * 3.0 * clamp(a / 40.0, 0.0, 1.0));
		float j = smoothstep(-3.0, 3.0, a) * (1.0 - smoothstep(len * 0.7, len, a)) * (1.0 - smoothstep(jw - 1.5, jw + 1.5, off));
		col.rgb = mix(col.rgb, fill_col.rgb, j * min(spurt * 2.5, 1.0) * fill_col.a);
	}
	COLOR = col;
}
