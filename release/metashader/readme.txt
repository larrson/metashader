＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
本ファイルは、フォルダの説明や動作確認に関して記述したものです。
実行確認やビルド確認をされる前に、ご一読くださいますようお願いいたします。

e-mail to:kekerin@gmail.com
履歴：
	2011/05/26　作成
＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

■フォルダ階層
下記は、提出物に含まれるフォルダ階層とその内容物に関してです。
万一、内容物に不備がございましたら、上記メールアドレスにご連絡ください。

metashader
├─bin  
│  └─Release	…リリースビルドの実行ファイル
├─data
│  ├─model	…本ソフトウェアで使用されているモデルアセット
│  ├─sample　…本ソフトウェアで読み込むことができるサンプルファイル
│  ├─shader	…本ソフトウェアで使用されているシェーダアセット
│  └─texture …本ソフトウェアで使用されているテクスチャアセット
└─doc		…各種ドキュメント

■動作確認
　動作確認をされる前に、下記の「動作確認に必要なもの」をご確認いただき、環境に応じてインストールをお願いいたします。
　ソフトウェアの使用方法に関しては、「metashader\doc\メタシェーダ概要.pptx」をご参照ください。

　■動作環境の要件
　　Windows PC(XP以降）ShaderModel3.0以上に対応したGPU　　

　■動作確認に必要なもの
　　本ソフトウェアは、動作に際して以下の別パッケージを必要とします。　 
　
　　■vcredist_x86.exe
　    MicrosoftVisualC++ 2008 SP1 再配布可能パッケージ（x86）
　　
　　　下記のサイトからのダウンロードが可能です。
　　　http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=a5c84275-3b97-4ab7-a40d-3802b2af5fc2

　　■directx_feb2010_redist.exe
  　  DirectXエンドユーザーランタイム（February 2010）

　　　下記のサイトからのダウンロードが可能です。
　　　http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&FamilyID=0cef8180-e94a-4f56-b157-5ab8109cb4f5

　　■dotNetFx35setup.exe
  　  Microsoft.NET Framework3.5再配布可能パッケージ

　　　下記のサイトからのダウンロードが可能です。
　　　http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=333325fd-ae52-4e35-b531-508d977d32a6&displaylang=ja　　

■最新のソースコードのダウンロード
　本ソフトウェアのソースコードは、「Google Project」のSVNを利用して管理しております。
　そのため、誰もが最新のソースコードをチェックアウトすることが可能です。

　チェックアウト方法は、下記HP(グーグルコード内の本ソフトウェアのページ)をご参照ください。
　http://code.google.com/p/metashader/source/checkout

  ■ビルド方法は、チェックアウトしたフォルダの直下の「readme.txt」ファイルをご参照ください。

　　　
　　　
　　　