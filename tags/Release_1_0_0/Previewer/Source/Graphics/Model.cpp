/**
	@file Model.cpp
	@brief モデルクラス
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Model.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace opk
{
	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CModel::CModel()
		: m_strFilePath()
		, m_pMesh (NULL)
		, m_pDeclaration ( NULL )		
		, m_pVertexBuffer ( NULL )
		, m_pIndexBuffer ( NULL )
	{}

	//------------------------------------------------------------------------------------------
	CModel::~CModel()
	{
		// 破棄
		Destroy();
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::LoadFromFile( const std::string& i_strFilePath )
	{
		HRESULT hr;

		// パスを保持
		m_strFilePath = i_strFilePath;

		// 作成前に破棄
		Destroy();

		/// 作成 ///

		IDirect3DDevice9* pd3dDevice = CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice();

		V_RETURN( D3DXLoadMeshFromXA( m_strFilePath.c_str(), D3DXMESH_MANAGED, pd3dDevice, NULL, NULL, NULL, NULL, &m_pMesh));		

		// 法線＆接線ベクトルを作成
		{
			// 法線と接線をもつ頂点宣言を作成
			D3DVERTEXELEMENT9 vertDecl[] = 
			{
				{ 0, 0,  D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0 },
				{ 0, 12, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL,   0 },
				{ 0, 24, D3DDECLTYPE_FLOAT2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0 },
				{ 0, 32, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TANGENT,  0 },
				{ 0, 44, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BINORMAL, 0 },
				D3DDECL_END()
			};

			// 元のメッシュのコピー用（新頂点宣言を適用したもの）と接線生成後の格納用メッシュ
			LPD3DXMESH clonedMesh, newMesh;			

			// 新しい頂点宣言をもつメッシュへコピー
			hr = m_pMesh->CloneMesh(D3DXMESH_VB_MANAGED, vertDecl, pd3dDevice, &clonedMesh); MY_ASSERT( SUCCEEDED(hr) );						

			// 法線をもつか
			bool bHasNormal = (m_pMesh->GetFVF() & D3DFVF_NORMAL) != 0;

			// 接線を生成
			hr = D3DXComputeTangentFrameEx( clonedMesh, D3DDECLUSAGE_TEXCOORD, 0, D3DDECLUSAGE_TANGENT, 0,
				D3DDECLUSAGE_BINORMAL, 0
				, bHasNormal ? D3DX_DEFAULT : D3DDECLUSAGE_NORMAL
				, 0
				, bHasNormal ? 0 : D3DXTANGENT_CALCULATE_NORMALS
				, NULL, 0.01f, 0.25f, 0.01f, &newMesh, NULL ); MY_ASSERT( SUCCEEDED(hr) );

			// メッシュを解放			
			m_pMesh->Release();
			clonedMesh->Release();						

			// 接線の生成されたメッシュを保持
			m_pMesh = newMesh;			

			// 各バッファを取得			
			D3DVERTEXELEMENT9 declaration[MAX_FVF_DECL_SIZE];			
			hr = m_pMesh->GetDeclaration( declaration ); MY_ASSERT( SUCCEEDED(hr) );
			pd3dDevice->CreateVertexDeclaration( declaration, &m_pDeclaration );
			hr = m_pMesh->GetVertexBuffer( &m_pVertexBuffer ); MY_ASSERT( SUCCEEDED(hr) );
			hr = m_pMesh->GetIndexBuffer( &m_pIndexBuffer ); MY_ASSERT( SUCCEEDED(hr) );			
		}

		return S_OK;
	}

	//------------------------------------------------------------------------------------------
	void CModel::Destroy()
	{
		/// 破棄 ///
		SAFE_RELEASE( m_pDeclaration );
		SAFE_RELEASE( m_pVertexBuffer );
		SAFE_RELEASE( m_pIndexBuffer );
		SAFE_RELEASE( m_pMesh );		
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::Render()
	{
		/// 描画 ///
		HRESULT hr = E_FAIL;

		IDirect3DDevice9* pd3dDevice = CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice(); MY_ASSERT( pd3dDevice );

		// シェーダを用いて描画する
		shader::CShaderMan* pShaderMan = shader::CShaderMan::GetInstance();
		if( SUCCEEDED( pShaderMan->Activate() ) )
		{
			// バッファを指定して描画
			// (接線ベクトル生成後にサブセットの数を取得すると何故か0が帰ってくるので)
			DWORD bytePerVertex = m_pMesh->GetNumBytesPerVertex();
			DWORD vertexNum = m_pMesh->GetNumVertices();
			DWORD faceNum = m_pMesh->GetNumFaces();

			pd3dDevice->SetVertexDeclaration( m_pDeclaration );
			pd3dDevice->SetIndices( m_pIndexBuffer );			
			pd3dDevice->SetStreamSource( 0, m_pVertexBuffer, 0, bytePerVertex );
			pd3dDevice->DrawIndexedPrimitive( D3DPT_TRIANGLELIST, 0, 0, vertexNum, 0, faceNum );

#if 0
			// サブセットの数を取得する
			DWORD nSubset;
			hr = m_pMesh->GetAttributeTable( NULL, &nSubset );					

			// 全サブセットを同じマテリアルで描画するので、一括描画
			for(size_t i = 0; i < nSubset; ++i )
			{
				hr = m_pMesh->DrawSubset(i);
				if( FAILED(hr) )
				{
					MY_ASSERT(false);
					break;
				}
			}	
#endif

			pShaderMan->Deactivate();
		}		
		return hr;
	}
} // end of namespace opk