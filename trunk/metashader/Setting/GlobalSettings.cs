///
/// グラフ構造やUI設定以外の設定項目を管理するクラスの定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using metashader.Common;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace metashader.Setting
{
    /// <summary>
    /// マテリアルの種類
    /// </summary>
    public enum MaterialType : int
    {
        /// <summary>
        /// フォンマテリアル
        /// </summary>
        Phong,
        /// <summary>
        /// カスタム
        /// </summary>
        Custom,
        /// <summary>
        /// 最大数
        /// </summary>
        Max,
    }

    /// <summary>
    /// アルファブレンディングの種類
    /// </summary>
    public enum BlendMode : int
    {
        /// <summary>
        /// 無し
        /// </summary>
        None,   
        /// <summary>
        /// 通常
        /// </summary>
        Normal, 
        /// <summary>
        /// 加算
        /// </summary>
        Add,
        /// <summary>
        /// 減算
        /// </summary>
        Sub,
        /// <summary>
        /// 最大数
        /// </summary>
        Max,
    }

    /// <summary>
    /// グラフ構造やUI設定以外の設定項目
    /// </summary>
    [Serializable]
    public class GlobalSettings : IDeserializationCallback
    {
#region variables
        /// <summary>
        /// マテリアルの種類
        /// </summary>
        MaterialType m_materialType = MaterialType.Phong;

        /// <summary>
        /// アルファブレンディングの種類
        /// </summary>
        BlendMode m_blendMode = BlendMode.None;        
#endregion    
    
#region properties
        public MaterialType MaterialType
        {
            get { return m_materialType; }
            set { m_materialType = value; }
        }

        /// <summary>
        /// アルファブレンディングの種類
        /// </summary>
        public BlendMode BlendMode
        {
            get { return m_blendMode; }
            set { m_blendMode = value; }
        }
#endregion

        #region override methods        
        /// <summary>
        /// デシリアライズ時のコールバック
        /// </summary>
        /// <param name="sender"></param>
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            // リセット対象のプロパティを別のサブシステムへ反映させるため、イベントを利用して初期化する
            NotifyAllProperties();
        }
        #endregion

#region public methods
        /// <summary>
        /// ファイルからロード
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="formatter"></param>
        public void Load(FileStream fileStream, BinaryFormatter formatter)
        {
            // ロード
            GlobalSettings settings = formatter.Deserialize(fileStream) as GlobalSettings;

            // ロードしたプロパティを別のサブシステムへ反映させるため、イベントを利用して初期化する
            NotifyAllProperties();
        }

        /// <summary>
        /// 初期化処理
        /// 「新規作成」機能用
        /// </summary>
        public void Reset()
        {
            // アプリケーション中の他のサブシステムへ通知するため、イベントを利用して初期化する
            NotifyAllProperties();         
        }
        
        /// <summary>
        /// 指定したプロパティへ新しい値を設定する
        /// </summary>
        /// <typeparam name="ParamterType">プロパティの型</typeparam>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="newValue">新しい値</param>
        /// <param name="undoredo">Undo/Redo用コマンドバッファ<</param>
        /// <returns>新しい値の設定が成功したか</returns>
        public bool ChangeProperty<ParameterType>(string propertyName, ParameterType newValue, UndoRedoBuffer undoredo)
        {
            // パラメータ設定用のIUndoRedoクラスのインタンス
            ParameterUndoRedo<ParameterType, GlobalSettings> parameterUndoRedo = new ParameterUndoRedo<ParameterType, GlobalSettings>(this, propertyName, newValue);

            // 値の設定を試みる
            if (parameterUndoRedo.Do() == false)
            {
                // 失敗
                return false;
            }

            // 成功
            // undoredoコマンドバッファへ積む
            if (undoredo != null)
            {
                undoredo.Add(parameterUndoRedo);
            }

            return true;
        }        
#endregion

#region private methods
        /// <summary>
        /// 全プロパティを外部へ通知する
        /// </summary>
        private void NotifyAllProperties()
        {
            ChangeProperty<MaterialType>("MaterialType", m_materialType, null);
            ChangeProperty<BlendMode>("BlendMode", BlendMode.None, null);
        }
#endregion
    }
}
