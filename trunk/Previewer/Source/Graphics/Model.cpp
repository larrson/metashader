/**
	@file Model.cpp
	@brief ���f���N���X
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Model.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace opk
{
	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CModel::CModel( std::wstring i_strFilePath )
		: m_pMesh (NULL)
		, m_strFilePath( i_strFilePath )
	{}

	//------------------------------------------------------------------------------------------
	CModel::~CModel()
	{
		// �j��
		Destroy();
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::Restore()
	{
		HRESULT hr;

		// �쐬�O�ɔj��
		Destroy();

		/// �č쐬 ///
		IDirect3DDevice9* pd3dDevice = CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice();

		V_RETURN( D3DXLoadMeshFromX( m_strFilePath.c_str(), D3DXMESH_MANAGED, pd3dDevice, NULL, NULL, NULL, NULL, &m_pMesh));

		// �@����������΍쐬
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
		/// �j�� ///
		SAFE_RELEASE( m_pMesh );
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::Render()
	{
		/// �`�� ///
		HRESULT hr;
		// �T�u�Z�b�g�̐����擾����
		DWORD nSubset;
		hr = m_pMesh->GetAttributeTable( NULL, &nSubset );

		// �S�T�u�Z�b�g�𓯂��}�e���A���ŕ`�悷��̂ŁA�ꊇ�`��
		for(size_t i = 0; i < nSubset; ++i )
		{
			V_RETURN( m_pMesh->DrawSubset(i) );
		}	
		return hr;
	}
} // end of namespace opk