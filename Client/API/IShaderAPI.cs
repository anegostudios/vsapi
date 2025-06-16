
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// API for configuring and loading GLSL Shaders
    /// </summary>
    public interface IShaderAPI
    {
        /// <summary>
        /// Returns an empty instance of an IShaderProgram for you to configure. Once configured, call RegisterShaderProgram
        /// </summary>
        /// <returns></returns>
        IShaderProgram NewShaderProgram();

        /// <summary>
        /// Returns an empty instance of an IShader for you to configure as vertex, fragment or geometry shader
        /// </summary>
        /// <returns></returns>
        IShader NewShader(EnumShaderType shaderType);

        /// <summary>
        /// Registers a configured IShaderProgram. The name must correspond to the .vsh and .fsh filenames (without ending).
        /// Returns a program number to be used in UseShaderProgram()
        /// </summary>
        /// <param name="name"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        int RegisterFileShaderProgram(string name, IShaderProgram program);

        /// <summary>
        /// Registers a configured IShaderProgram. Will not load anything from the shaders folder.
        /// Returns a program number to be used in UseShaderProgram()
        /// </summary>
        /// <param name="name"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        int RegisterMemoryShaderProgram(string name, IShaderProgram program);

        /// <summary>
        /// Returns the loaded shader for use in rendering
        /// </summary>
        /// <param name="renderPass"></param>
        /// <returns></returns>
        IShaderProgram GetProgram(int renderPass);

        /// <summary>
        /// Returns the loaded shader for use in rendering
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IShaderProgram GetProgramByName(string name);

        /// <summary>
        /// Discards all currently compiled shaders and recompiles them. Returns true if all shaders compiled without errors.
        /// </summary>
        /// <returns></returns>
        bool ReloadShaders();

        /// <summary>
        /// Returns true if given GLSL Version is available on this machine
        /// </summary>
        /// <param name="minVersion"></param>
        /// <returns></returns>
        bool IsGLSLVersionSupported(string minVersion);
    }
}
