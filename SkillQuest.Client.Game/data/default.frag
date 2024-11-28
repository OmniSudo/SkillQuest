#version 330 core

in vec4 color;    // Input texture coordinates from the vertex shader
in vec2 uv;

out vec4 out_color;   // Output color of the fragment

uniform sampler2D textureSampler; // Uniform for the texture

void main()
{
    // Sample the texture using the provided texture coordinates
//    out_color = texture(textureSampler, uv);
    out_color = color;    
}
