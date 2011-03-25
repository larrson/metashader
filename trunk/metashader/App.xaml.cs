using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace metashader
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
#region variables
        /// <summary>
        /// グラフデータ
        /// </summary>
        ShaderGraphData.ShaderGraphData m_graphData;

        /// <summary>
        /// 選択管理オブジェクト
        /// </summary>
        ShaderGraphData.SelectManager m_selectManager;

        /// <summary>
        /// UI用コマンドマネージャ
        /// UIがデータに対して実行するコマンドを管理
        /// </summary>
        Command.CommandManager m_uiCommandManager;
#endregion

#region properties
        /// <summary>
        /// 現在のアプリケーション
        /// </summary>
        static public App CurrentApp
        {
            get { return (App.Current as App);}
        }

        /// <summary>
        /// グラフデータ
        /// </summary>
        public ShaderGraphData.ShaderGraphData GraphData
        {
            get { return m_graphData; }
        }

        /// <summary>
        /// 選択管理オブジェクト
        /// </summary>
        public ShaderGraphData.SelectManager SelectManager
        {
            get { return m_selectManager; }
        }

        /// <summary>
        /// UIコマンドマネージャ
        /// </summary>
        public Command.CommandManager UICommandManager
        {
            get { return m_uiCommandManager; }
        }
#endregion

#region public method
        /// <summary>
        /// 保存
        /// </summary>
        public void Save(string path, BinaryFormatter formatter)
        {
            FileStream fs = new FileStream(path,
            FileMode.Create,
            FileAccess.Write);
            // グラフの読み込み
            m_graphData.Save(fs, formatter);            
            fs.Close();
        }

        /// <summary>
        /// ロード
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="formatter"></param>
        public void Load(string path, BinaryFormatter formatter)
        {
            FileStream fs = new FileStream(path,
            FileMode.Open,
            FileAccess.Read);            

            // グラフの読み込み
            m_graphData = ShaderGraphData.ShaderGraphData.Load(fs, formatter);            

            fs.Close();

            // 選択解除
            m_selectManager.Clear();
        }
#endregion

#region event handlers
        /// <summary>
        /// アプリケーション開始時に呼ばれるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // データ初期化
            m_graphData = new ShaderGraphData.ShaderGraphData();

            // 選択マネージャ初期化
            m_selectManager = new ShaderGraphData.SelectManager();

            // コマンドマネージャ初期化
            m_uiCommandManager = new Command.CommandManager();
        }
#endregion        
    }
}
