using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colorimeter_Config_GUI
{
    class CRCClass
    {
        public CRCClass()
        { }
        //////////////////////////////////////////////////////////////////////////////////////////////
        const ushort cnCRC_16 = 0x8005;
        // CRC-16    = X16 + X15 + X2 + X0                  
        const ushort cnCRC_CCITT = 0x1021;
        // CRC-CCITT = X16 + X12 + X5 + X0，据说这个 16 位 CRC 多项式比上一个要好
        const uint cnCRC_32 = 0x04C10DB7;
        // CRC-32    = X32 + X26 + X23 + X22 + X16 + X11 + X10 + X8 + X7 + X5 + X4 + X2 + X1 + X0
        static uint[] Table_CRC = new uint[256]; // CRC 表

        //  构造 16 位 CRC 表
        void BuildTable16(ushort aPoly)
        {
            short i, j;
            ushort nData;
            ushort nAccum;

            for (i = 0; i < 256; i++)
            {
                nData = (ushort)(i << 8);
                nAccum = 0;
                for (j = 0; j < 8; j++)
                {
                    if (((nData ^ nAccum) & 0x8000) != 0) nAccum = (ushort)((nAccum << 1) ^ aPoly);
                    else nAccum <<= 1;
                    nData <<= 1;
                }
                Table_CRC[i] = (uint)nAccum;
            }
        }

        //  计算 16 位 CRC 值，CRC-16 或 CRC-CCITT
        public ushort CRC_16_IMB(ushort[] aData)
        {
            int i;
            ushort nAccum = 0;
            int aSize = aData.Length;

            BuildTable16(cnCRC_16); //  cnCRC_16 or cnCRC_CCITT
            for (i = 0; i < aSize; i++)
                nAccum = (ushort)((nAccum << 8) ^ (ushort)Table_CRC[(nAccum >> 8) ^ aData[i]]);
            return nAccum;
        }

        public ushort CRC_16_CCITT(ushort[] aData)
        {
            int i;
            ushort nAccum = 0;
            int aSize = aData.Length;

            BuildTable16(cnCRC_CCITT); //  cnCRC_16 or cnCRC_CCITT
            for (i = 0; i < aSize; i++)
                nAccum = (ushort)((nAccum << 8) ^ (ushort)Table_CRC[(nAccum >> 8) ^ aData[i]]);
            return nAccum;
        }
        //  构造 32 位 CRC 表
        void BuildTable32(uint aPoly)
        {
            short i, j;
            uint nData;
            uint nAccum;

            for (i = 0; i < 256; i++)
            {
                nData = (uint)(i << 24);
                nAccum = 0;
                for (j = 0; j < 8; j++)
                {
                    if (((nData ^ nAccum) & 0x80000000) != 0) nAccum = (nAccum << 1) ^ aPoly;
                    else nAccum <<= 1;
                    nData <<= 1;
                }
                Table_CRC[i] = nAccum;
            }
        }

        //  计算 32 位 CRC-32 值
        public uint CRC_32(ushort[] aData)
        {
            int i;
            uint nAccum = 0;
            int aSize = aData.Length;
            BuildTable32(cnCRC_32);
            for (i = 0; i < aSize; i++)
                nAccum = (nAccum << 8) ^ Table_CRC[(nAccum >> 24) ^ aData[i]];
            return nAccum;
        }
    }
}
