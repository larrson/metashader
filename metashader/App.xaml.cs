using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using metashader.Common;

namespace metashader
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {      
#region variables
        /// <summary>
        /// ファイル関連の設定
        /// </summary>
        Setting.FileSettings m_fileSettings;

        /// <summary>
        /// グローバル設定
        /// グラフデータや、UI設定以外の設定項目
        /// </summary>
        Setting.GlobalSettings m_globalSettings;

        /// <summary>
        /// グラフデータ
        /// </summary>
        ShaderGraphData.ShaderGraphData m_graphData;

        /// <summary>
        /// 選択管理オブジェクト
        /// </summary>
        ShaderGraphData.SelectManager m_selectManager;

        /// <summary>
        /// イベント管理オブジェクト
        /// </summary>
        Event.EventManager m_eventManager;

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
        /// ファイル設定
        /// </summary>
        public Setting.FileSettings FileSettings
        {
            get { return m_fileSettings; }
        }

        /// <summary>
        /// グローバル設定
        /// </summary>
        public Setting.GlobalSettings GlobalSettings
        {
            get { return m_globalSettings; }
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

        /// <summary>
        /// イベントマネージャ
        /// </summary>
        public Event.EventManager EventManager
        {
            get { return m_eventManager; }
        }
#endregion

#region public method
        /// <summary>
        /// 新規作成
        /// </summary>
        public void CreateNew()
        {                  
            // undoredoの無効化
            UndoRedoManager.Instance.Clear();

            // 選択の解除
            m_selectManager.Clear();

            // グラフデータの初期化
            m_graphData.Reset();
            
            // ファイル設定の初期化
            m_fileSettings.Reset();

            // 設定のリセット
            m_globalSettings.Reset();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save(string path, BinaryFormatter formatter)
        {
            path = Path.GetFullPath(path);
            using ( FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write) )
            {
                // ファイル設定の保存
                {
                    // 作業フォルダを設定する
                    m_fileSettings.OldWorkFolderPath = Path.GetDirectoryName(path);
                    formatter.Serialize(fs, m_fileSettings);
                }
                // 全体設定の保存
                formatter.Serialize(fs, m_globalSettings);

                // グラフの保存
                m_graphData.Save(fs, formatter);                        
            }            
        }

        /// <summary>
        /// ロード
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="formatter"></param>
        public void Load(string path, BinaryFormatter formatter)
        {
            // @@@ファイルの有効性のチェック

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))            
            {
                // ファイル設定の読み込み
                m_fileSettings = formatter.Deserialize(fs) as Setting.FileSettings;
                m_fileSettings.CurrentFilePath = System.IO.Path.GetFullPath(path);

                // 全体設定の読み込み
                m_globalSettings.Load(fs, formatter);

                // グラフの読み込み                
                LoadGraphData(fs, formatter);                
            }                        

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
            // フォルダ設定初期化
            m_fileSettings = new Setting.FileSettings();

            // グローバル設定初期化
            m_globalSettings = new Setting.GlobalSettings();

            // データ初期化
            m_graphData = new ShaderGraphData.ShaderGraphData();

            // 選択マネージャ初期化
            m_selectManager = new ShaderGraphData.SelectManager();

            // コマンドマネージャ初期化
            m_uiCommandManager = new Command.CommandManager();

            // イベントマネージャ初期化
            m_eventManager = new Event.EventManager();       
     
            // イベントハンドラ初期化
            m_eventManager.GlobalSettingPropertyChangedEvent += new Event.GlobalSettingPropertyChangedEventHandler(m_eventManager_GlobalSettingPropertyChangedEvent);
        }        

        /// <summary>
        /// アプリケーション終了時に呼ばれるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // アプリケーション設定の保存
            metashader.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// グローバル設定変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void m_eventManager_GlobalSettingPropertyChangedEvent(object sender, metashader.Event.GlobalSettingPropertyChangedEventArgs args)
        {
            // プロパティごとに行う処理を変更
            switch( args.PropertyName )
            {
                // マテリアルタイプが変更された
                case "MaterialType":
                    // グラフデータのエラー検出
                    // グラフデータ側にイベントハンドラをもたせないのは、データとしての独立性を保つため
                    App.CurrentApp.GraphData.DetectError();
                    break;
            }
        }
#endregion                

#region private methods
        /// <summary>
        /// グラフロードのサブルーチン
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="formatter"></param>
        private void LoadGraphData(FileStream fs, BinaryFormatter formatter)
        {
            m_graphData.Reset();

            // ノードとリンク以外のデータ構造をデシリアライズする
            m_graphData = ShaderGraphData.ShaderGraphData.Load(fs, formatter);

            /// ノードのデシリアライズ ///            
            LinkedList<ShaderGraphData.ShaderNodeDataBase> nodeList =
                formatter.Deserialize(fs) as LinkedList<ShaderGraphData.ShaderNodeDataBase>;
            // ノードを再構築
            foreach (ShaderGraphData.ShaderNodeDataBase itr in nodeList)
            {
                m_graphData.AddNode(itr, null);
            }

            /// リンクのデシリアライズ ///            
            LinkedList<ShaderGraphData.LinkData> linkList =
                formatter.Deserialize(fs) as LinkedList<ShaderGraphData.LinkData>;

            // グラフ内のリンクを再構築
            foreach (ShaderGraphData.LinkData itr in linkList)
            {
                m_graphData.AddLink(itr._outNodeHash, itr._outJointIndex, itr._inNodeHash, itr._inJointIndex, null);
            }
        }        
#endregion
    }
}
