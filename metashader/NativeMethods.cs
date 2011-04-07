//
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
        public unsafe static extern IntPtr GetNextSurface();
        [DllImport(dllName)]
        public unsafe static extern void Resize(int i_nScreenWidth, int i_nScreenHeight);

        /// シェーダの作成
        [DllImport(dllName)]
        public unsafe static extern void CreatePixelShaderFromBuffer(byte[] i_buffer, int i_nSize );

        /// パラメータの設定系

        [DllImport(dllName, CharSet = CharSet.Ansi)]
        public unsafe static extern void SetUniformVector4(string i_pszName, float x, float y, float z, float w);
    }
}
