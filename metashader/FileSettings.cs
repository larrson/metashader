using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace metashader
{
    /// <summary>
    /// ファイル設定
    /// </summary>
    [Serializable]
    public class FileSettings
    {
#region variables
        /// <summary>
        /// 新しい作業フォルダのパス
        /// </summary>
        string m_newWorkFolderPath = "";

        /// <summary>
        /// 以前の作業フォルダのパス
        /// デシリアライズされた作業フォルダのパス
        /// </summary>
        string m_oldWorkFolderPath = "";
#endregion

        #region properties
        /// <summary>
        /// 実行ファイルがあるフォルダのパス
        /// </summary>
        public static string ApplicationFolderPath
        {
            get { return Path.GetDirectoryName( System.Windows.Forms.Application.ExecutablePath ); }
        }

        /// <summary>
        /// 以前の作業フォルダ
        /// </summary>
        public string OldWorkFolderPath
        {
            set { m_oldWorkFolderPath = value; }
        }

        /// <summary>
        /// 新しい作業フォルダ
        /// </summary>
        public string NewWorkFolderPath
        {
            set 
            { 
                m_newWorkFolderPath = value; 
                // アプリケーションの作業フォルダを設定
                Directory.SetCurrentDirectory(m_newWorkFolderPath);
            }
        }
        #endregion

        #region public methods 
        /// <summary>
        /// 新旧の作業フォルダのパスに従って、元の絶対パスを新しい絶対パスへ変換する
        /// OnSerialize時に使用する。
        /// </summary>
        /// <param name="path"></param>
        public string ToAbsolutePath(string oldFullPath)
        {
            // 元の作業フォルダからの相対パスへ変換
            Uri oldWorkUri = new Uri(m_oldWorkFolderPath + "\\");
            Uri oldFullUri = new Uri(oldWorkUri, oldFullPath);
            Uri relativeUri = oldWorkUri.MakeRelativeUri(oldFullUri);                                  

            // 新しい作業フォルダを使用して絶対パスへ変換
            Uri newFullUri = new Uri(new Uri(m_newWorkFolderPath + "\\"), relativeUri);
            string newFullPath = newFullUri.AbsolutePath;
            newFullPath = newFullPath.Replace('/', '\\');

            return newFullPath;
        }
        #endregion
    }
}
