using System;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public enum EnumShaderType
    {
        FragmentShader = 35632,
        VertexShader = 35633,
        GeometryShader = 36313,
        GeometryShaderExt = 36313,
        TessEvaluationShader = 36487,
        TessControlShader = 36488,
        ComputeShader = 37305
    }


    public interface IShader
    {
        EnumShaderType Type { get; }

        /// <summary>
        /// If set, the shader registry will attach this bit of code to the beginning of the fragment shader file. Useful for setting stuff at runtime when using file shaders, e.g. via #define
        /// </summary>
        string PrefixCode { get; set; }

        /// <summary>
        /// Source code of the shader
        /// </summary>
        string Code { get; set; }
    }

    public interface IShaderProgram : IDisposable
    {
        int ProgramId { get; }

        /// <summary>
        /// When loading from file this is the asset domain to load from
        /// </summary>
        string AssetDomain { get; set; }

        /// <summary>
        /// A uniqe shader pass number assigned to each shader program
        /// </summary>
        int PassId { get; }

        /// <summary>
        /// The name it was registered with. If you want to load this shader from a file, make sure the use the filename here
        /// </summary>
        string PassName { get; }

        /// <summary>
        /// If true, it well configure the textures to clamp to the edge (CLAMP_TO_EDGE). Requires the textureid to be defined using SetTextureIds
        /// </summary>
        bool ClampTexturesToEdge { get; set; }

        /// <summary>
        /// The vertex shader of this shader program
        /// </summary>
        IShader VertexShader { get; set; }

        /// <summary>
        /// The fragment shader of this shader program
        /// </summary>
        IShader FragmentShader { get; set; }

        /// <summary>
        /// The geometry shader of this shader program
        /// </summary>
        IShader GeometryShader { get; set; }


        void Use();
        void Stop();
        bool Compile();

        void Uniform(string uniformName, float value);

        void Uniform(string uniformName, int value);

        void Uniform(string uniformName, Vec2f value);

        void Uniform(string uniformName, Vec3f value);

        void Uniform(string uniformName, Vec4f value);

        void Uniforms4(string uniformName, int count, float[] values);

        void UniformMatrix(string uniformName, float[] matrix);

        void BindTexture2D(string samplerName, int textureId, int textureNumber);

        void BindTextureCube(string samplerName, int textureId, int textureNumber);

        void UniformMatrices(string uniformName, int count, float[] matrix);

        void UniformMatrices4x3(string uniformName, int count, float[] matrix);

        bool HasUniform(string uniformName);


        /// <summary>
        /// True if this shader has been disposed
        /// </summary>
        bool Disposed { get; }
        bool LoadError { get; }

        Datastructures.OrderedDictionary<string, UBORef> UBOs { get; }
    }


}
