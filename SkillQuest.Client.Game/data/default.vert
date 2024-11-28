#version 330 core

in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv;

out vec3 color;
out vec2 uv;

uniform vec3 g_cameraPosision;
uniform vec3 g_cameraDirection;
uniform mat4 g_model;
uniform mat4 g_view;
uniform mat4 g_projection;

void main ( void ) {
    gl_Position = vec4( in_position, 1.0 );
    uv = in_uv;
    color = normalize( vec3( in_position ) );
}