//========================================================================
// システム(UI，グラフデータにまたがる)コマンド群の定義
//========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.Command
{
    /// <summary>
    /// 現在のシェーダグラフを実行する
    /// </summary>
    public class ExecuteShaderCommand : CommandBase
    {
#region constructor
       public ExecuteShaderCommand()
           : base( "シェーダを実行" )
       {       
       }
#endregion

#region override methods
       public override bool CanExecute(object parameter)
       {
           return true;
       }

       public override void Execute(object parameter)
       {
           // シェーダコードの出力先バッファ
           byte[] buffer;

           // バッファにシェーダコードを書きだす
           if( App.CurrentApp.GraphData.ExportShaderCodeToBuffer( out buffer ) )
           {
               // 書き出しが成功したので、ビューアへ送る
               NativeMethods.CreatePixelShaderFromBuffer(buffer, buffer.Length);

               // シェーダのパラメータを設定
               App.CurrentApp.GraphData.ApplyAllParameters();               
           }           
       }         
#endregion   
    }
}
