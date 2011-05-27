＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
本ファイルは、提出物に含まれるフォルダの説明や動作確認およびビルド環境に関して記述したものです。
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
│  ├─Debug	…デバッグビルドの実行ファイル
│  └─Release	…リリースビルドの実行ファイル
├─data
│  ├─model	…本ソフトウェアで使用されているモデルアセット
│  ├─sample　…本ソフトウェアで読み込むことができるサンプルファイル
│  ├─shader	…本ソフトウェアで使用されているシェーダアセット
│  └─texture …本ソフトウェアで使用されているテクスチャアセット
├─doc		…各種ドキュメント
├─metashader	…ソフトウェア本体のソースコード
├─pakages	…動作確認に必要な外部パッケージ
└─Previewer	…ソフトウェア内で使用しているDLLのソースコード

■動作確認
　動作確認をされる前に、下記の「動作確認に必要なもの」をご確認いただき、環境に応じてインストールをお願いいたします。
　ソフトウェアの使用方法に関しては、「metashader\doc\メタシェーダ概要.pptx」をご参照ください。

　■動作確認に必要なもの
　　本ソフトウェアは、動作に際して以下の別パッケージを必要とします。
　  「metashader\packages」フォルダ内の各パッケージのインストールをお願いいたします。　
　
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

■ソースコードからのビルド
　下記、「ソースコードからのビルドに必要なもの」をご参照の上、
　「metashader\metashader.sln」をVisualStudioで開き、ビルドを行ってください。
　また、ソースコードの設計や使用している技術等に関しては、「」をご参照ください。

　■ソースコードからのビルドに必要なもの
　　■開発ソフト
　　　VisualStudio2008 SP1以降
　　　(Visual Studio2010ではビルド確認をしておりません)

　　■DirectX SDK February 2010以降    
　　  ■VisualStudio自体の設定（プロジェクトファイルの設定ではない）にDirectX SDKのパスを通っている必要がございます。
　　　　パスを通していない場合は、下記のパスを、VisualStudioかPreviewer.vcprojに設定していただく必要がございます。
	
　　　　■インクルードフォルダのパス	
　　　　　$(DXSDK_DIR)Include

　　　　■ライブラリフォルダのパス
　　　　　$(DXSDK_DIR)Lib\x86

　　■サードパーティ製ライブラリ
　　　■Boost C++
　　　　本ソフトウェアでは、Boost C++ライブラリを一部使用しております。
　　　　http://www.boost.org/users/download/より最新版をダウンロードしていただき、
　　　　解凍後の「boost_XXX\boost」（Xはバージョン番号）フォルダを
　　　　「metashader\Previewer\3rdParty」の直下にコピーしてください。

　　　　例）smart_ptr.hppが、metashader\Previewer\3rdParty\boost\smart_ptr.hppという階層に配置されることになります。

■最新のソースコードのダウンロード
　本ソフトウェアのソースコードは、「Google Project」のSVNを利用して管理しております。
　そのため、誰もが最新のソースコードをチェックアウトすることが可能です。

　チェックアウト方法は、下記HP(グーグルコード内の本ソフトウェアのページ)をご参照ください。
　http://code.google.com/p/metashader/source/checkout

　　　
　　　
　　　