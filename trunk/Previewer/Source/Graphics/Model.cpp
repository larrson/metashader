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
		: m_strFilePath()
		, m_pMesh (NULL)
		, m_pDeclaration ( NULL )		
		, m_pVertexBuffer ( NULL )
		, m_pIndexBuffer ( NULL )
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

		// �@�����ڐ��x�N�g�����쐬
		{
			// �@���Ɛڐ��������_�錾���쐬
			D3DVERTEXELEMENT9 vertDecl[] = 
			{
				{ 0, 0,  D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0 },
				{ 0, 12, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL,   0 },
				{ 0, 24, D3DDECLTYPE_FLOAT2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0 },
				{ 0, 32, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TANGENT,  0 },
				{ 0, 44, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BINORMAL, 0 },
				D3DDECL_END()
			};

			// ���̃��b�V���̃R�s�[�p�i�V���_�錾��K�p�������́j�Ɛڐ�������̊i�[�p���b�V��
			LPD3DXMESH clonedMesh, newMesh;			

			// �V�������_�錾�������b�V���փR�s�[
			hr = m_pMesh->CloneMesh(D3DXMESH_VB_MANAGED, vertDecl, pd3dDevice, &clonedMesh); MY_ASSERT( SUCCEEDED(hr) );						

			// �@��������
			bool bHasNormal = (m_pMesh->GetFVF() & D3DFVF_NORMAL) != 0;

			// �ڐ��𐶐�
			hr = D3DXComputeTangentFrameEx( clonedMesh, D3DDECLUSAGE_TEXCOORD, 0, D3DDECLUSAGE_TANGENT, 0,
				D3DDECLUSAGE_BINORMAL, 0
				, bHasNormal ? D3DX_DEFAULT : D3DDECLUSAGE_NORMAL
				, 0
				, bHasNormal ? 0 : D3DXTANGENT_CALCULATE_NORMALS
				, NULL, 0.01f, 0.25f, 0.01f, &newMesh, NULL ); MY_ASSERT( SUCCEEDED(hr) );

			// ���b�V�������			
			m_pMesh->Release();
			clonedMesh->Release();						

			// �ڐ��̐������ꂽ���b�V����ێ�
			m_pMesh = newMesh;			

			// �e�o�b�t�@���擾			
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
		/// �j�� ///
		SAFE_RELEASE( m_pDeclaration );
		SAFE_RELEASE( m_pVertexBuffer );
		SAFE_RELEASE( m_pIndexBuffer );
		SAFE_RELEASE( m_pMesh );		
	}

	//------------------------------------------------------------------------------------------
	HRESULT CModel::Render()
	{
		/// �`�� ///
		HRESULT hr = E_FAIL;

		IDirect3DDevice9* pd3dDevice = CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice(); MY_ASSERT( pd3dDevice );

		// �V�F�[�_��p���ĕ`�悷��
		shader::CShaderMan* pShaderMan = shader::CShaderMan::GetInstance();
		if( SUCCEEDED( pShaderMan->Activate() ) )
		{
			// �o�b�t�@���w�肵�ĕ`��
			// (�ڐ��x�N�g��������ɃT�u�Z�b�g�̐����擾����Ɖ��̂�0���A���Ă���̂�)
			DWORD bytePerVertex = m_pMesh->GetNumBytesPerVertex();
			DWORD vertexNum = m_pMesh->GetNumVertices();
			DWORD faceNum = m_pMesh->GetNumFaces();

			pd3dDevice->SetVertexDeclaration( m_pDeclaration );
			pd3dDevice->SetIndices( m_pIndexBuffer );			
			pd3dDevice->SetStreamSource( 0, m_pVertexBuffer, 0, bytePerVertex );
			pd3dDevice->DrawIndexedPrimitive( D3DPT_TRIANGLELIST, 0, 0, vertexNum, 0, faceNum );

#if 0
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
#endif

			pShaderMan->Deactivate();
		}		
		return hr;
	}
} // end of namespace opk