using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class AnchorEnum
    {
        /// <summary>
        /// ��ʾ��Ϣ����
        /// </summary>
        public enum EMessageLevel
        {
            EML_DEBUG, //����
            EML_INFO,  //��Ϣ
            EML_WARN, //����
            EML_ERROR, //����
        }


        /// <summary>
        /// �̶��ʲ�--�ֿ���ڵ�(���ҡ���վ������)��Ӧ�Ĳֿ����
        /// add 2020-03-16 plq
        /// </summary>
        public enum EParentStorageID
        {
            ���� = 1,
            ��վ = 2,
            ���� = 10,
        }

        /// <summary>
        /// RFID����--��������/�澯����
        /// add 2020-03-18 plq
        /// </summary>
        public enum ERfid_OperationType
        {
            ���ΰ� = 1, //�������豸��,��������
            ������ = 2, //�豸��ֿ��
            ���ʲ����� = 3, //�ֿ��и����ʲ�����
            ������� = 4, 
            ��ʧ = 5,
        } 

        /// <summary>
        /// RFID����--����״̬
        /// add 2020-03-18 plq
        /// </summary>
        public enum ERfid_ProcessingState
        {
            δ���� = 1,
            �Ѱ��豸 = 2, //�������豸��
            �������� = 3, //�豸����ֿ��
            �ѽ�� = 4,  //�豸����ֿ��� 
            �Ѷ�ʧ = 5,  //�豸�Ѷ�ʧ  
        }

        /// <summary>
        /// RFID����--�����ֿ����ͱ���
        /// add 2020-03-18 plq
        /// </summary>
        public enum ERfid_ParentType
        {  
            ��վ = 1,
            ���� = 2,
            ���� = 3,
        }

        /// <summary>
        /// RFID����--�澯���ݵĸ������ͱ���
        /// add 2020-03-25 plq
        /// </summary>
        public enum ERfid_UpdateAlarmType
        { 
            ���²������� = 1,
            �����豸������״̬ = 2,
            ���´���״̬ = 3,
        }

    }
}
