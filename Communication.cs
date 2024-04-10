using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ut64configurator
{
    internal class Communication
    {
        private Gyro gyro1 = new Gyro();
        private GyroBuffer gyroBuffer1 = new GyroBuffer();

        private SerialPort serialPort;
        private int baudRate = 2000000;
        private int bag_length = 60;
        private int cmdLength = 42;
        private string name = "COM1";
        private byte recDone = 0;
        private byte[] recBuffer = new byte[60];
        private byte createSuccess = 0;
        private List<byte> SerialRevList = new List<byte>();
        private List<byte> SerialRevDATA = new List<byte>();
        private int dataType = 0;
        private int dataType2 = 0;
        bool sendSuccess= false;    
        private byte[] sendBuffer = new byte[42];
        private byte[] sendBufferCopy = { 0, 0, 0x24, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0x24 };
        public static List<UInt16> Downlink_communication = new List<UInt16>{
            0x0011,0x0012,0x0013,0x0020,0x0031,0x0032,0x0033,0x0034,0x0014
        };//0:开环 2:调谐电压 3:扫频数据 4:PLL数据 5: 6: 7: 8:相位
        bool isDatasave = false;
        string dataSavestr = String.Empty;

        //WAM
        //FAMode varible
        private double Angle = 0, Angle_Last = 0, Angle_Previou = 0, Angle_rate = 0;
        private double DA = 0 , DB = 0;

        //crc
        ushort[] crc_tab ={
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
            0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
            0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
            0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
            0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
            0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
            0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
            0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
            0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
            0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
            0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
            0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
            0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
            0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
            0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
            0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
            0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
            0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
            0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
            0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
            0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
            0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
            0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
            0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
            0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
            0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
            0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
            0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
            0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
            0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
        };
        //CRC校验接收
        public ushort GetCrc(List<byte> bytes)
        {
            ushort crc = 0xffff;
            for (int i = 2; i < bytes.Count - 4; i++)
            {
                UInt16 high = (UInt16)(crc / 256);
                crc <<= 8;
                crc ^= crc_tab[high ^ bytes[i]];

            }
            return crc;
        }

        //CRC校验发送
        public void GetCrc_Send(byte[] send_crc)
        {

            ushort crc = 0xffff;
            for (int i = 4; i < 39; i++)
            {
                UInt16 high = (UInt16)(crc / 256);
                crc <<= 8;
                crc ^= crc_tab[high ^ send_crc[i]];
            }
            from_u16_to_u8(send_crc, crc, 39);
        }
        public void from_u16_to_u8(byte[] buffer_u8, UInt16 data_u16, int index)
        {
            buffer_u8[index] = (byte)((data_u16 >> 8) & 0x000000FF);
            buffer_u8[index + 1] = (byte)(data_u16 & 0x000000FF);
        }

        public void from_u32_to_u8(byte[] buffer_u8, UInt32 data_u16, int index)
        {
            buffer_u8[index] = (byte)((data_u16 >> 24) & 0x000000FF);
            buffer_u8[index + 1] = (byte)((data_u16 >> 16) & 0x000000FF);
            buffer_u8[index + 2] = (byte)((data_u16 >> 8) & 0x000000FF);
            buffer_u8[index + 3] = (byte)(data_u16 & 0x000000FF);
        }

        public void from_s16_to_u8(byte[] buffer_u8, Int16 data_u16, int index)
        {
            buffer_u8[index] = (byte)((data_u16 >> 8) & 0x000000FF);
            buffer_u8[index + 1] = (byte)(data_u16 & 0x000000FF);
        }

        public void from_s32_to_u8(byte[] buffer_u8, Int32 data_u16, int index)
        {
            buffer_u8[index] = (byte)((data_u16 >> 24) & 0x000000FF);
            buffer_u8[index + 1] = (byte)((data_u16 >> 16) & 0x000000FF);
            buffer_u8[index + 2] = (byte)((data_u16 >> 8) & 0x000000FF);
            buffer_u8[index + 3] = (byte)(data_u16 & 0x000000FF);
        }
        public uint deserialized_to_u32(List<byte> list_change, int k)
        {
            uint data = 0;
            data = (uint)((list_change[k] << 24) | (list_change[k + 1] << 16) | (list_change[k + 2] << 8) | (list_change[k + 3]));
            return data;
        }

        public int deserialized_to_32(List<byte> list_change, int k)
        {
            Int32 data = 0;
            data = (int)((list_change[k] << 24) | (list_change[k + 1] << 16) | (list_change[k + 2] << 8) | (list_change[k + 3]));
            return data;
        }

        public ushort deserialized_to_u16(List<byte> list_change, int k)
        {
            UInt16 data = 0;
            data = (UInt16)((list_change[k] << 8) | (list_change[k + 1]));
            return data;
        }

        public Communication (Gyro GYRO1, GyroBuffer GYROBUFFER1, byte[] SENDBUFFER)    // 构造函数
        {
            gyro1 = GYRO1;
            gyroBuffer1 = GYROBUFFER1;
            sendBuffer=SENDBUFFER;
        }
        public void Encode()
        {

        }

        public void Decode() 
        {
            if(SerialRevList.Count > 1)
            {
                //SerialRevList.CopyTo(recBuffer);

                for(int i = 0;i< SerialRevList.Count;i++)
                {
                    if(SerialRevList[i] == 0xEB && SerialRevList[i+1] == 0x90)
                    {
                        SerialRevList.RemoveRange(0, i);
                        dataDecode();    // 这个可以放到一个新的线程中，提高效率
                        break;
                    }
                }
                

                //if (SerialRevList[0] == 0xEB && SerialRevList[1] == 0x90)
                //{
                //    dataDecode();
                //}
                //else if(SerialRevList[0] == 0 && SerialRevList[1] == 0)
                //{
                //    cmdDecode();
                //}
                //else
                //{
                //    SerialRevList.RemoveAt(0);
                //}
            }

        }
        public void dataDecode()
        {
           
            if (SerialRevList.Count >= bag_length)
            {
                if (SerialRevList[bag_length - 2] == 0x0D && SerialRevList[bag_length - 1] == 0x0A)
                {
                    SerialRevDATA.AddRange(SerialRevList);
                    SerialRevDATA.RemoveRange(bag_length, SerialRevDATA.Count - bag_length);
                    SerialRevList.RemoveRange(0, bag_length);
                    SerialRevDATA.CopyTo(recBuffer);
                    //if (isDatasave == true && SerialRevDATA[2] == 1)
                    //{

                    //    dataSavestr = dataSavestr + fromListToString(SerialRevDATA) + "\r\n";
                    //}
                    if (GetCrc(SerialRevDATA) == deserialized_to_u16(SerialRevDATA, 56))
                    {

                        switch (SerialRevDATA[2])
                        {
                            case 1:
                                gyro1.setValue(Macro.RUNTIME_FIELD_AMP_CHA_I, deserialized_to_32(SerialRevDATA, 7) / 2895.0);
                                gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_CHA_I, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA_I));

                                gyro1.setValue(Macro.RUNTIME_FIELD_AMP_CHA_Q, deserialized_to_32(SerialRevDATA, 11) / 2895.0);
                                gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_CHA_Q, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA_Q));

                                gyro1.setValue(Macro.RUNTIME_FIELD_AMP_CHA, deserialized_to_u32(SerialRevDATA, 15) / 2895.0);
                                gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_CHA, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA));

                                gyro1.setValue(Macro.RUNTIME_FIELD_AMP_CHB_I, deserialized_to_32(SerialRevDATA, 19) / 2895.0);
                                gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_CHB_I, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_I));

                                gyro1.setValue(Macro.RUNTIME_FIELD_AMP_CHB_Q, deserialized_to_32(SerialRevDATA, 23) / 2895.0);
                                gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_CHB_Q, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_Q));

                                gyro1.setValue(Macro.RUNTIME_FIELD_AMP_CHB, deserialized_to_u32(SerialRevDATA, 27) / 2895.0);
                                gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_CHB, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB));

                                if (isDatasave == true)
                                {
                                    string str1 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA).ToString("0.000");
                                    string str2 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA_I).ToString("0.000");
                                    string str3 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA_Q).ToString("0.000");

                                    string str4 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB).ToString("0.000");
                                    string str5 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_I).ToString("0.000");
                                    string str6 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_Q).ToString("0.000");                              
                                    dataSavestr = dataSavestr + str1 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6 + "\r\n";
                                }

                                switch (SerialRevDATA[55])
                                {
                                    case 0x01:
                                        if (gyro1.getValuefre() != deserialized_to_u32(SerialRevDATA, 35))
                                        {
                                            gyro1.setValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY, deserialized_to_u32(SerialRevDATA, 35) * (5000000 / 4294967295.0));
                                            gyroBuffer1.setValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY, gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY));

                                            gyro1.setValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHA, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHA));
                                            gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHA, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHA));

                                            gyro1.setValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHB, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB));
                                            gyroBuffer1.setValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHB, gyro1.getValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHB));

                                            gyro1.setValue(Macro.RUNTIME_FIELD_PHASE_A, deserialized_to_32(SerialRevDATA, 43) / 2147483647.0 * 180 * 4);
                                            gyroBuffer1.setValue(Macro.RUNTIME_FIELD_PHASE_A, gyro1.getValue(Macro.RUNTIME_FIELD_PHASE_A));

                                            gyro1.setValue(Macro.RUNTIME_FIELD_PHASE_B, deserialized_to_32(SerialRevDATA, 47) / 2147483647.0 * 180 * 4);
                                            gyroBuffer1.setValue(Macro.RUNTIME_FIELD_PHASE_B, gyro1.getValue(Macro.RUNTIME_FIELD_PHASE_B));

                                            gyro1.setValuefre(deserialized_to_u32(SerialRevDATA, 35));

                                            
                                        }
                                        dataType = 1;
                                        break;
                                    case 0x09:
                                        gyro1.setValue(Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR, deserialized_to_32(SerialRevDATA, 31) / 2147483647.0 * 180 * 4);
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR, gyro1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR));

                                        gyro1.setValue(Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE, deserialized_to_32(SerialRevDATA, 39) / 2147483647.0 * 180 * 4);
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE, gyro1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE));

                                        gyro1.setValue(Macro.RUNTIME_FIELD_PLL_CURRENT_FRE, deserialized_to_32(SerialRevDATA, 35) * (5000000 / 4294967295.0));
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_PLL_CURRENT_FRE, gyro1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_FRE));

                                        //WAM
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EV, deserialized_to_32(SerialRevDATA, 43));
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EA, deserialized_to_32(SerialRevDATA, 47)/1000.0);
                                        dataType = 2;
                                        break;
                                    case 16:
                                        gyro1.setValue(Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY, deserialized_to_32(SerialRevDATA, 31) * (5000000 / 4294967295.0));
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY, gyro1.getValue(Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY));

                                        gyro1.setValue(Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT, deserialized_to_32(SerialRevDATA, 35) * (2500 / 32768.0));
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT, gyro1.getValue(Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT));
                                        dataType = 16;
                                        break;
                                    case 15:
                                        gyro1.setValue(Macro.RUNTIME_FIELD_QFACTOR_START_FREQUENCY, deserialized_to_32(SerialRevDATA, 31) * (5000000 / 4294967295.0));
                                        gyro1.setValue(Macro.RUNTIME_FIELD_QFACTOR_SHORT_TAU, deserialized_to_32(SerialRevDATA, 35) / 1000.0);
                                        gyro1.setValue(Macro.RUNTIME_FIELD_QFACTOR_LONG_TAU, deserialized_to_32(SerialRevDATA, 39) / 1000.0);
                                        gyro1.setValue(Macro.RUNTIME_FIELD_QFACTOR_MIN, deserialized_to_32(SerialRevDATA, 43));
                                        gyro1.setValue(Macro.RUNTIME_FIELD_QFACTOR_MAX, deserialized_to_32(SerialRevDATA, 47));
                                        dataType = 15;
                                        break;
                                    case 10:
                                    case 11:
                                    case 12:
                                    case 13:
                                        gyro1.setValue(Macro.RUNTIME_PID_CURRENT_ERROR, deserialized_to_32(SerialRevDATA, 31) / 2895.0);
                                        gyroBuffer1.setValue(Macro.RUNTIME_PID_CURRENT_ERROR, gyro1.getValue(Macro.RUNTIME_PID_CURRENT_ERROR));

                                        gyro1.setValue(Macro.RUNTIME_PID_CURRENT_OUTPUT, deserialized_to_32(SerialRevDATA, 35) * (2500 / 32768.0));
                                        gyroBuffer1.setValue(Macro.RUNTIME_PID_CURRENT_OUTPUT, gyro1.getValue(Macro.RUNTIME_PID_CURRENT_OUTPUT));

                                        if (isDatasave == true)
                                        {   

                                            string str1 = gyro1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_I).ToString("0.000");
                                            string str2 = gyro1.getValue(Macro.RUNTIME_PID_CURRENT_OUTPUT).ToString("0.000");


                                            dataSavestr = dataSavestr + str2 + "," + str1 + "\r\n";


                                        }

                                        dataType = 6;
                                        break;
                                    case 19:
                                        // WAM : energy control loop data
                                        // EA: energy angle ; EV: energy value
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EVERROR, deserialized_to_32(SerialRevDATA, 31));
                                        double DV = deserialized_to_32(SerialRevDATA, 35) * (2500 / 32768.0);    //currentoutput-全角PID输出；总驱动幅值
                                                                                                                 //
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EV, deserialized_to_32(SerialRevDATA, 43));
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EV, gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV));

                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EA, (deserialized_to_32(SerialRevDATA, 39))/1000.0);       // angle ∈（-90°，90°）
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EA, gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA));

                                        //private double Angle=0, Angle_Last=0, Angle_Previou=0,Angle_rate=0,DA=0,DB=0;
                                        // 以下根据上传的角度和总驱动幅值来 计算DA DB驱动分量
                                        Angle = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA);
                                        Angle_rate = (Angle + Angle_Previou - 2 * Angle_Last) / 2;
                                        Angle_Previou = Angle_Last;
                                        Angle_Last = Angle;
                                        if (true)
                                        {
                                            DA = DV * Math.Sin(Angle / 180 * Math.PI);
                                            DB = DV * Math.Cos(Angle / 180 * Math.PI);
                                        }
                                        else
                                        {
                                            DA = DV * Math.Cos(Angle / 180 * Math.PI);
                                            DB = DV * Math.Sin(Angle / 180 * Math.PI);
                                        }
                                        
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_RATE, Angle_rate);
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_DA, DA);
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_DB, DB);

                                        if (isDatasave == true)
                                        {
                                            string str1 = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA).ToString("0.000");
                                            string str2 = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV).ToString("0.000");
                                            string str3 = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QV).ToString("0.000");

                                            dataSavestr = dataSavestr + str1 + "," + str2 + "," + str3 + "\r\n";
                                        }

                                        dataType = 19;
                                        break;
                                    case 20:
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_QVERROR, deserialized_to_32(SerialRevDATA, 31));
                                        double DVQ = deserialized_to_32(SerialRevDATA, 35) * (2500 / 32768.0);    //currentoutput-全角PID输出；总驱动幅值

                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EA, deserialized_to_32(SerialRevDATA, 39) / 1000.0);
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EA, gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA));   // 把QA 放入EA中，用于显示angle

                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_QV, deserialized_to_32(SerialRevDATA, 43));
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_QV, gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QV));
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EV, deserialized_to_32(SerialRevDATA, 47));
                                        gyroBuffer1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_EV, gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV));

                                        Angle = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA);
                                        Angle_rate = (Angle + Angle_Previou - 2 * Angle_Last) / 2;
                                        Angle_Previou = Angle_Last;
                                        Angle_Last = Angle;
                                        if (true)
                                        {
                                            DA = DVQ * Math.Cos(Angle / 180 * Math.PI);
                                            DB = DVQ * Math.Sin(Angle / 180 * Math.PI)*(-1);
                                        }
                                        else
                                        {
                                            DA = DVQ * Math.Sin(Angle / 180 * Math.PI)*(-1);
                                            DB = DVQ * Math.Cos(Angle / 180 * Math.PI);
                                        }

                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_RATE, Angle_rate);
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_DAQ, DA);
                                        gyro1.setValue(Macro.RUNTIME_FIELD_FULLANGEL_DBQ, DB);

                                        if (isDatasave == true)
                                        {
                                            string str1 = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA).ToString("0.000");
                                            string str2 = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV).ToString("0.000");
                                            string str3 = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QV).ToString("0.000");

                                            dataSavestr = dataSavestr + str1 + "," + str2 + "," + str3 + "\r\n";
                                        }

                                        dataType = 20;
                                        break;
                                    default:
                                        dataType = 0;
                                        break;
                                }
                                break;

                            default:
                                break;
                        }

                    }

                    SerialRevDATA.Clear();
                   

                }
                else
                {
                    SerialRevList.RemoveRange(0, 2);
                }


            }
           

        }

        public void cmdDecode()
        {
            if (SerialRevList.Count >= cmdLength)
            {
                if (SerialRevList[40] == 0x0D && SerialRevList[41] == 0x0A)
                {
                    SerialRevDATA.AddRange(SerialRevList);
                    SerialRevDATA.RemoveRange(bag_length, SerialRevDATA.Count - cmdLength);
                    SerialRevList.RemoveRange(0, cmdLength);
                    sendSuccess=true;
                    SerialRevDATA.Clear();
                }     
            }
           
            
        }

        public byte createSerialPort()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = baudRate;
            serialPort.PortName = name;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            createSuccess = 1;
            return createSuccess;
        }
        public void openSerialPort()
        {
            serialPort.Open();
        }

        public void closeSerialPort()
        {
            serialPort.Close();
        }

        public string[] getSerialPortID()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        public int getDatatypeG1()
        {
            return dataType;
        }


        public void setSerialPortName(string Spname)
        {
            serialPort.PortName = Spname;
        }

        public void setSerialPortBaudRate(int SpbaudRate)
        {
            baudRate = SpbaudRate;
        }

        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] Receive_buffer = new byte[this.serialPort.BytesToRead];
            int r = serialPort.Read(Receive_buffer, 0, Receive_buffer.Length);
            if (r > 0)
            {
                recDone = 1;
            }
            else
            {
                recDone = 0;
            }
            SerialRevList.AddRange(Receive_buffer);

            //
            //

            Decode();     
        }

        public int getRecStatus()
        {
            return recDone;
        }

        public byte[] getRecData()
        {
            return recBuffer;
        }

        public void clearSerialRevList()
        {
            SerialRevList.Clear();
        }
        public void sendCommand()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(sendBuffer, 0, cmdLength);
                    sendBufferCopy.CopyTo(sendBuffer, 0);
                }
                catch
                {
                    MessageBox.Show("串口数据发送出错，请检查.", "错误");
                }
            }
        }
        public void dataSaveStart()
        {
            isDatasave = true;
        }
        public void dataSaveStop()
        {
            isDatasave = false;
        }
        public void emptySaveStr()
        {
            dataSavestr = String.Empty;
        }
        private string fromListToString(List<byte> list)
        {
            string str = String.Empty;
            str = BitConverter.ToString(list.ToArray());
            return str;
        }
        public string getDataSaveStr()
        {
            return dataSavestr;
        }
    }

    

}

