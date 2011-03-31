#pragma once

/**
	@file GlobalFunctions.h
	@brief Dllから公開する関数の宣言を記述
*/

#define DLLEXPORT __declspec(dllexport)

/**
	@brief プレビューアのエントリポイント	
	@param [in] i_nScreenWidth	スクリーンの幅
	@param [in] i_nScreenHeight スクリーンの高さ
	@retval 初期化に成功したか
*/
extern "C" DLLEXPORT int PreviewerMain(int i_nScreenWidth, int i_nScreenHeight);

/**
	@brief 現在のフレームをレンダリング
*/
extern "C" DLLEXPORT void RenderFrame();

/**
	@brief 終了処理
	@retval 終了処理に成功したか
*/
extern "C" DLLEXPORT int ShutDown();

/**
	@brief ウィンドウメッセージ処理
	@param [in] i_hWnd	ウインドウハンドル
	@param [in]	i_nMsg	メッセージ
	@param [in] i_wParam メッセージパラメータ
	@param [in] i_lParam メッセージパラメータ
*/
extern "C" DLLEXPORT void WndProc(int *i_hWnd, int i_nMsg, int i_wParam, int i_lParam);

/**
	@brief 次のレンダリングされたフレームのサーフェイスを取得する
	@retval サーフェイスのポインタ
*/
extern "C" DLLEXPORT void* GetNextSurface();

/**
	@brief グラフィックデバイスの有効性を確認し、ロストしていたらその対応を行う
*/
// extern "C" DLLEXPORT void CheckGraphicDevice();

/**
	@brief 画面のリサイズを処理する
	@param [in] i_nScreenWidth	スクリーンの幅
	@param [in] i_nScreenHeight スクリーンの高さ
*/
extern "C" DLLEXPORT void Resize(int i_nScreenWidth, int i_nScreenHeight );