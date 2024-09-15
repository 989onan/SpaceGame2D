/*#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texCoord);
}
*/
#version 330 core
out vec4 FragColor;

//uniform vec4 ourColor; // we set this variable in the OpenGL code.

void main()
{
    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);//ourColor;
} 