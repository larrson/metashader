﻿//
// C++で作成されたDll内のネイティブ関数のインポート
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace metashader
{
    /// <summary>
    /// C++で作成されたDll内のネイティブ関数のインポート
    /// </summary>
    static class NativeMethods
    {
#if DEBUG
        const string dllName = "Previewerd.dll";
#else
        const string dllName = "Previewer.dll";
#endif
        [DllImport(dllName)]
        public unsafe static extern int PreviewerMain(int i_nScreenWidth, int i_nScreenHeight);
        [DllImport(dllName)]
        public unsafe static extern void RenderFrame();   
        [DllImport(dllName)]
        public unsafe static extern int ShutDown();
        [DllImport(dllName)]
        public unsafe static extern IntPtr WndProc(IntPtr i_hWnd, int i_nMsg, IntPtr i_wParam, IntPtr i_lParam);
        [DllImport(dllName)]
        public unsafe static extern IntPtr GetBackBuffer();
        [DllImport(dllName)]
        public unsafe static extern void Render();
        [DllImport(dllName)]
        public unsafe static extern void Resize(int i_nScreenWidth, int i_nScreenHeight);

        /// シェーダの作成
        [DllImport(dllName)]
        public unsafe static extern void CreatePixelShaderFromBuffer(byte[] i_buffer, int i_nSize );

        // シェーダの切り替え
        [DllImport(dllName)]
        public unsafe static extern void UseDefaultShader();

        /// パラメータの設定系        
        [DllImport(dllName, CharSet = CharSet.Ansi)]
        public unsafe static extern void SetUniformFloat(string i_pszName, float i_fValue);
        [DllImport(dllName, CharSet = CharSet.Ansi)]
        public unsafe static extern void SetUniformVector4(string i_pszName, float x, float y, float z, float w);
        [DllImport(dllName, CharSet=CharSet.Ansi)]
        public unsafe static extern void SetTexturePath(string i_pszName, string i_pszPath);
        [DllImport(dllName)]
        public unsafe static extern void SetSamplerState(string i_pszName, ShaderGraphData.SamplerState i_samplerState);
        [DllImport(dllName)]
        public unsafe static extern void SetBlendMode( int i_nBlendMode );

        /// テクスチャサムネイル用データの取得
        [DllImport(dllName, CharSet=CharSet.Ansi)]
        public unsafe static extern bool GetImagePixelData(string i_pszPath, int i_nWidth, int i_nHeight, IntPtr i_pBuffer );
    }
}
