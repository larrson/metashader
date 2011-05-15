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
		: m_pMesh (NULL)
		, m_strFilePath()
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

		// 法線が無ければ作成
		if( !(m_pMesh->GetFVF() & D3DFVF_NORMAL ) )
		{
			ID3DXMesh* pTempMesh;
			hr = m_pMesh->CloneMeshFVF( m_pMesh->GetOptions()
				, m_pMesh->GetFVF() | D3DFVF_NORMAL
				, pd3dDevice
				, &pTempMesh);
			hr = D3DXComputeNormals( pTempMesh, NULL );

			SAFE_RELEASE( m_pMesh );
			m_pMesh = pTempMesh;
		}

		return S_OK;
	}

	//------------------------------------------------------------------------------------------
	void CModel::Destroy()
	{
		/// 破棄 ///
		SAFE_RELEASE( m_pMesh );
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::Render()
	{
		/// 描画 ///
		HRESULT hr = E_FAIL;

		// シェーダを用いて描画する
		shader::CShaderMan* pShaderMan = shader::CShaderMan::GetInstance();
		if( SUCCEEDED( pShaderMan->Activate() ) )
		{
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

			pShaderMan->Deactivate();
		}		
		return hr;
	}
} // end of namespace opk