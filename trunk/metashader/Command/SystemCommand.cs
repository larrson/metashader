//========================================================================
// システム(UI，グラフデータにまたがる)コマンド群の定義
//========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows;

namespace metashader.Command
{  
    /// <summary>
    /// 新規作成コマンド
    /// </summary>
    public class CreateNewCommand : CommandBase
    {
#region constructor
       public CreateNewCommand()
           : base( "新規作成" )
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
           // 確認ダイアログ
           MessageBoxResult result = MessageBox.Show(
                    "保存されていない変更は失われますが、よろしいですか？"
                , "確認"
                , MessageBoxButton.OKCancel
               );

            // OKなら新規作成
            if( result == MessageBoxResult.OK )
            {
                App.CurrentApp.CreateNew();
            }
       }         
#endregion         
    }

    /// <summary>
    /// ファイルロードコマンド
    /// 内部でダイアログを開き、選択したファイルを開く
    /// </summary>
    public class LoadCommand : CommandBase
    {
#region constructor
       public LoadCommand()
           : base( "ロード" )
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
           // ファイル選択ダイアログ
           OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Metashader Files (*.MSH)|*.MSH";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                App.CurrentApp.Load(dlg.FileName, new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter());
            }
       }         
#endregion 
    }

    /// <summary>
    /// 名前を付けてファイル保存コマンド
    /// </summary>
    public class SaveAsCommand : CommandBase
    {
#region constructor
       public SaveAsCommand()
           : base( "名前を付けて保存" )
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
           // ファイル保存ダイアログ
           SaveFileDialog dlg = new SaveFileDialog();
           // デフォルトファイル名
           dlg.FileName = "新規メタシェーダ";
           // フィルター
            dlg.Filter = "Metashader Files (*.msh)|*.msh";
           // 拡張子が省略された際に不可
            dlg.AddExtension = true;
           // 規定の拡張子
            dlg.DefaultExt = ".msh";           

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                App.CurrentApp.Save(dlg.FileName, new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter());
            }
       }         
#endregion 
    }

    /// <summary>
    /// エクスポートコマンド
    /// </summary>
    public class ExportCommand : CommandBase
    {
        #region constructor
        public ExportCommand()
            : base("書き出し")
        {
        }
        #endregion

        #region override methods
        public override bool CanExecute(object parameter)
        {
            return App.CurrentApp.GraphData.NoError;
        }

        public override void Execute(object parameter)
        {
            // ファイル保存ダイアログ
            SaveFileDialog dlg = new SaveFileDialog();
            // デフォルトファイル名
            dlg.FileName = System.IO.Path.GetFileNameWithoutExtension(App.CurrentApp.FileSettings.CurrentFilePath);
            // フィルター
            dlg.Filter = "HLSL Files (*.hlsl)|*.hlsl";
            // 拡張子が省略された際に不可
            dlg.AddExtension = true;
            // 規定の拡張子
            dlg.DefaultExt = ".hlsl";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                App.CurrentApp.GraphData.ExportShaderCode(dlg.FileName);
            }
        }
        #endregion
    }

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
           return App.CurrentApp.GraphData.NoError;
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
