//////////////////////////////////////////////////////////////////////////
// ���[�U�[��`�̃m�[�h
// �X�N���v�g����̂悤�ɁA�ҏW��̃A�v���P�[�V�����{�̂̃R���p�C�����s�v
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// �t���l���m�[�h
    /// </summary>
    [Serializable]   
    [ShaderNodeTypeName("User_Fresnel")]
    class User_FresnelNode : ShaderNodeDataBase
    {   
#region constructors
        public User_FresnelNode(string name, Point pos)
            : base("User_Fresnel", name, pos)
        {

        }
#endregion

#region properties
        /// <summary>
        /// UI��ɕ\������\����
        /// </summary>
        public override string Label
        {
            get { return "Fresnel"; }
        }
#endregion

#region public methods                
        /// <summary>
        /// �X�g���[���փ}�N������������
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderMacroCode(StringWriter stream)
        {
            // ���C�g����錾����
            //@ ���𓮓I�ɕύX
            stream.WriteLine("#define DIR_LIGHT_NUM 3");
            // �J�����ʒu�̐錾��L����
            stream.WriteLine("#define UNIFORM_CameraPosition");
        }

        /// <summary>
        /// �X�g���[���փV�F�[�_��uniform�錾����������
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // uniform��錾����
            stream.WriteLine("uniform float3 \tUniform_DirLightDir[DIR_LIGHT_NUM];"); // ���C�g����            
            stream.WriteLine("uniform float3 \tUniform_CameraPosition;"); // �J��������        
        } 

        /// <summary>
        /// �X�g���[���փV�F�[�_�̖{������������
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderMainCode(StringWriter stream)
        {
            // �o�͌^
            VariableType outputType = GetOutputJointVariableType(0);

            // ���͕ϐ��̖��O
            string n = GetInputJoint(0).VariableName;
            string k = GetInputJoint(1).VariableName;

            stream.WriteLine("\t{0} {1} = CalcFresnel({2}, {3}, In.CameraDir0, Uniform_DirLightDir[0] ) / dot(In.Normal0, In.CameraDir0);",                
                outputType.ToStringExt()
                , Name                              
                , n
                , k
                );
        }

        /// <summary>
        /// �m�[�h�̗L�����𔻒�
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            // ���̓����N�̗L�������m�F����
            // �S�Ă̓��͂����܂��Ă��邩�H
            foreach (JointData inputJoint in m_inputJoints)
            {
                if (inputJoint.JointList.Count != 1)
                    return false;
            }

            // ���͌^���K����(���Z�q�ɑ����Ă��邩)            
            // �^���X�J���[�^���ۂ�
            return GetInputJointVariableType(0).IsScalar() && GetInputJointVariableType(1).IsScalar();
        }
#endregion

#region protected methods
        /// <summary>
        /// �W���C���g�̏�����
        /// </summary>
        protected override void InitializeJoints()
        {
            // �W���C���g�̏�����
            // ����         
            m_inputJointNum = 2;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.None, "n");
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.None, "k");            
            // �o��            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.None);
        }
#endregion
    }
}