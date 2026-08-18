using System;
using System.Runtime.InteropServices;
using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    internal static class CairoGlyphLayout
    {
        // The native cairo glyph conversion used by the shipped runtime resolves glyph indices,
        // but it does not perform Arabic shaping. Enabling it leaves right-to-left text disconnected.
        const bool NativeGlyphLayoutEnabled = false;

        [StructLayout(LayoutKind.Sequential)]
        struct NativeGlyph32
        {
            public uint Index;
            public double X;
            public double Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct NativeGlyph64
        {
            public ulong Index;
            public double X;
            public double Y;
        }

        [DllImport("libcairo-2.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int cairo_scaled_font_text_to_glyphs(
            IntPtr scaledFont,
            double x,
            double y,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8,
            int utf8Len,
            out IntPtr glyphs,
            out int numGlyphs,
            IntPtr clusters,
            IntPtr numClusters,
            IntPtr clusterFlags);

        [DllImport("libcairo-2.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void cairo_glyph_free(IntPtr glyphs);

        public static bool TryCreateGlyphs(Context ctx, string text, double originX, double originY, out Glyph[] glyphs)
        {
            glyphs = null;

            if (!NativeGlyphLayoutEnabled)
            {
                return false;
            }

            if (ctx == null || string.IsNullOrEmpty(text) || !ComplexTextLayout.RequiresRightToLeftLayout(text))
            {
                return false;
            }

            string preparedText = ComplexTextLayout.PrepareForRendering(text);
            if (string.IsNullOrEmpty(preparedText))
            {
                return false;
            }

            IntPtr nativeGlyphs = IntPtr.Zero;

            try
            {
                int status = cairo_scaled_font_text_to_glyphs(
                    ctx.GetScaledFont().Handle,
                    originX,
                    originY,
                    preparedText,
                    -1,
                    out nativeGlyphs,
                    out int numGlyphs,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero
                );

                if (status != 0 || nativeGlyphs == IntPtr.Zero || numGlyphs <= 0)
                {
                    return false;
                }

                glyphs = CopyGlyphs(nativeGlyphs, numGlyphs);
                return glyphs.Length > 0;
            }
            catch
            {
                glyphs = null;
                return false;
            }
            finally
            {
                if (nativeGlyphs != IntPtr.Zero)
                {
                    cairo_glyph_free(nativeGlyphs);
                }
            }
        }

        static Glyph[] CopyGlyphs(IntPtr nativeGlyphs, int numGlyphs)
        {
            Glyph[] glyphs = new Glyph[numGlyphs];

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                int glyphSize = Marshal.SizeOf<NativeGlyph32>();
                for (int i = 0; i < numGlyphs; i++)
                {
                    NativeGlyph32 nativeGlyph = Marshal.PtrToStructure<NativeGlyph32>(nativeGlyphs + i * glyphSize);
                    glyphs[i] = new Glyph
                    {
                        Index = nativeGlyph.Index,
                        X = nativeGlyph.X,
                        Y = nativeGlyph.Y
                    };
                }
            }
            else
            {
                int glyphSize = Marshal.SizeOf<NativeGlyph64>();
                for (int i = 0; i < numGlyphs; i++)
                {
                    NativeGlyph64 nativeGlyph = Marshal.PtrToStructure<NativeGlyph64>(nativeGlyphs + i * glyphSize);
                    glyphs[i] = new Glyph
                    {
                        Index = (long)nativeGlyph.Index,
                        X = nativeGlyph.X,
                        Y = nativeGlyph.Y
                    };
                }
            }

            return glyphs;
        }
    }
}
