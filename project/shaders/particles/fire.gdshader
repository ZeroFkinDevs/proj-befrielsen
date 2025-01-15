shader_type particles;

float rand_from_seed(inout uint seed) {
	int k;
	int s = int(seed);
	if (s == 0)
	s = 305420679;
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0)
		s += 2147483647;
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}

void start() {
	uint alt_seed = NUMBER;
	vec3 pos = vec3(rand_from_seed(alt_seed) * 2.0 - 1.0, 0.0, rand_from_seed(alt_seed) * 2.0 - 1.0) * 1.0;
	pos = normalize(pos);
	pos *= 0.2;
	TRANSFORM[3].xyz = pos;

	float rotation = rand_from_seed(alt_seed) * PI;
	float scale = 0.7;
	TRANSFORM[0].xyz = vec3(sin(rotation), 0.0, cos(rotation)) * scale;
	TRANSFORM[1].xyz = vec3(0.0, 1.0, 0.0) * scale;
	TRANSFORM[2].xyz = vec3(cos(rotation), 0.0, sin(rotation)) * scale;
}

void process() {
	// Called every frame on existing particles (according to the Fixed FPS property).
}
