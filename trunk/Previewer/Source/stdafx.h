// stdafx.h : 標準のシステム インクルード ファイルのインクルード ファイル、または
// 参照回数が多く、かつあまり変更されない、プロジェクト専用のインクルード ファイル
// を記述します。
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Windows ヘッダーから使用されていない部分を除外します。
// Windows ヘッダー ファイル:
#include <windows.h>



// TODO: プログラムに必要な追加ヘッダーをここで参照してください。

/// C ランタイム ///
#include <stdlib.h>
#include <cmath>
#include <string>

/// ライブラリ ///
// ユーティリティ
#include <Utility/Utility.h>
// グラフィックス
#include <Graphics/graphics.h>
// IO
#include <IO/Mouse.h>
// システム
#include <System/App.h>