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
#include <map>

/// 外部ライブラリ ///
#include <boost\config.hpp>
#include <boost\shared_ptr.hpp>
#include <boost\utility.hpp>
using boost::shared_ptr;

/// 自作ライブラリ ///
#include <types.h>
// ユーティリティ
#include <Utility/Utility.h>
// グラフィックス
#include <Graphics/graphics.h>
#include <Graphics/ShaderMan.h>
// IO
#include <IO/Mouse.h>
// システム
#include <System/App.h>