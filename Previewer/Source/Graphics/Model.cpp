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
	CModel::CModel()
		: m_pMesh (NULL)
		, m_strFilePath()
	{}

	//------------------------------------------------------------------------------------------
	CModel::~CModel()
	{
		// �j��
		Destroy();
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::LoadFromFile( const std::string& i_strFilePath )
	{
		HRESULT hr;

		// �p�X��ێ�
		m_strFilePath = i_strFilePath;

		// �쐬�O�ɔj��
		Destroy();

		/// �쐬 ///

		IDirect3DDevice9* pd3dDevice = CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice();

		V_RETURN( D3DXLoadMeshFromXA( m_strFilePath.c_str(), D3DXMESH_MANAGED, pd3dDevice, NULL, NULL, NULL, NULL, &m_pMesh));

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
		HRESULT hr = E_FAIL;

		// �V�F�[�_��p���ĕ`�悷��
		shader::CShaderMan* pShaderMan = shader::CShaderMan::GetInstance();
		if( SUCCEEDED( pShaderMan->Activate() ) )
		{
			// �T�u�Z�b�g�̐����擾����
			DWORD nSubset;
			hr = m_pMesh->GetAttributeTable( NULL, &nSubset );		

			// �S�T�u�Z�b�g�𓯂��}�e���A���ŕ`�悷��̂ŁA�ꊇ�`��
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