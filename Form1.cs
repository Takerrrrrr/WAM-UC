using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Collections;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using ScottPlot.Plottable;
using ScottPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ScottPlot.Renderable;

namespace ut64configurator
{
    public partial class Form1 : Form
    {
        byte flag=0;
        Thread thread_circle;

        Gyro gyro1;

        GyroBuffer gyroBuffer1;

        Communication communication;
        EventTest Event = new EventTest();
        Paint gyroPaint1;

        AutoSizeForm asc = new AutoSizeForm();


        ScatterPlotList<double> scatterChat11;
        ScatterPlotList<double> scatterChat21;
        ScatterPlotList<double> scatterChat12;
        ScatterPlotList<double> scatterChat22;
        VLine vLine11 = new VLine();
        HLine hLine12 = new HLine();
        VLine vLine21 = new VLine();
        HLine hLine22 = new HLine();


        double[] chatXs = { };
        double[] chatYs = { };
        double scanCountgyro1 = 0;


        //扫频中6个radiobutton的控制符
        bool rb1 = true;
        bool rb2 = true;
        bool rb3 = true;

        //LED
        System.Timers.Timer timerLed;

        // dataSave
        System.Timers.Timer timerDataSave;
        //sendbuffer
        private byte [] sendBuffers = { 0, 0, 0x24, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x24 };
        


        public Form1()
        {
            InitializeComponent();
           
            //Init gyro data
            gyro1 = new Gyro();

            //初始化缓存区
            gyroBuffer1 = new GyroBuffer();

            //初始化串口接收
            communication = new Communication(gyro1,gyroBuffer1, sendBuffers);

            port_list.Items.AddRange(communication.getSerialPortID());

            communication.createSerialPort();

            //初始化串口发送事件 保存事件
            Event.ChangeNumSendCmd += new EventTest.NumManipulationHandler(communication.sendCommand);   
            Event.ChangeNumDataSaveStart += new EventTest.NumManipulationHandler(communication.dataSaveStart);
            Event.ChangeNumDataSaveStop += new EventTest.NumManipulationHandler(communication.dataSaveStop);
            Event.ChangeNumDataSaveEmpty += new EventTest.NumManipulationHandler(communication.emptySaveStr);
            //初始化陀螺绘图曲线

            gyroPaint1 = new Paint();
            Init_scanfre_line1();
            Init_IQ_line(ref gyroPaint1);
            Init_PLL_line(ref gyroPaint1);
            Init_PID_line(ref gyroPaint1);
            //WAM
            Init_FA_line(ref gyroPaint1);
            

            InitTimerLed();
            InitTimerDataSave();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化窗口自适应
            asc.controllInitializeSize(this);
            timer3.Start();

        }


        
        private void Form1_Shown(object sender, EventArgs e)
        {
          
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            upDataGyro1();



       
            switch (tabControl3.SelectedIndex)
            {
                case 0:
                    Render.repaintIQ(ref plt1, ref gyroPaint1.SA_IQ_checkboxs, ref gyroPaint1.SA_IQ_lines, 1);
                    Render.repaintIQ(ref plt2, ref gyroPaint1.SB_IQ_checkboxs, ref gyroPaint1.SB_IQ_lines, 1);
                    break;
                case 1:
                    if((scanCountgyro1 != gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY)) && gyroPaint1.sweepEnable == true)
                    {
                        
                        scatterChat11.Add(Convert.ToDouble(gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY).ToString("0.00")), gyro1.getValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHA));
                        scatterChat12.Add(Convert.ToDouble(gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY).ToString("0.00")), gyro1.getValue(Macro.RUNTIME_FIELD_AMP_SCAN_CHB));

                        scatterChat21.Add(Convert.ToDouble(gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY).ToString("0.00")), gyro1.getValue(Macro.RUNTIME_FIELD_PHASE_A));
                        scatterChat22.Add(Convert.ToDouble(gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY).ToString("0.00")), gyro1.getValue(Macro.RUNTIME_FIELD_PHASE_B));
                        scanCountgyro1 = gyro1.getValue(Macro.RUNTIME_FIELD_CURRENT_FREQUENCY);

                        plt21.Plot.AxisAuto();
                        plt22.Plot.AxisAuto();
                        plt21.Refresh();
                        plt22.Refresh();
                    }
                    break;
                case 2:
                    Render.repaintTwo(ref plt5, ref gyroPaint1.PLL_Amp_checkboxs, ref gyroPaint1.PLL_Amp_lines, 1);
                    Render.repaintOne(ref plt8, ref gyroPaint1.PLL_Phase_checkbox, ref gyroPaint1.PLL_Phase_lines, 1);
                    Render.repaintIQ(ref plt6, ref gyroPaint1.PLL_SA_IQ_checkboxs, ref gyroPaint1.PLL_SA_IQ_lines, 1);
                    Render.repaintIQ(ref plt7, ref gyroPaint1.PLL_SB_IQ_checkboxs, ref gyroPaint1.PLL_SB_IQ_lines, 1);
                    break;
                case 3:
                    Render.repaintTwo(ref plt10, ref gyroPaint1.PID_checkboxs, ref gyroPaint1.PID_lines, 1);
                    break;
                //WAM
                case 4:
                    Render.repaintOne(ref plt11, ref gyroPaint1.FA_EAngel_checkbox, ref gyroPaint1.FA_EAngel_line, 1);
                    Render.repaintOne(ref plt12, ref gyroPaint1.FA_EV_checkbox, ref gyroPaint1.FA_EV_line, 1);
                    Render.repaintOne(ref plt13, ref gyroPaint1.FA_QV_checkbox, ref gyroPaint1.FA_QV_line, 1);
                    break;


                default:
                    break;
            }
        }



        private void button_switch_Click(object sender, EventArgs e)//开关
        {
            try
            {
                if (button_switch.Text == "START")
                {
                    //button_switch.BackColor = Color.Red;
                    communication.clearSerialRevList();
                    button_switch.Text = "STOP";
                    communication.setSerialPortName(port_list.Text);
                    communication .openSerialPort();
          
                    timer1.Start();
                }
                else
                {
                    //button_switch.BackColor = Color.Green; 
                    communication.clearSerialRevList();
                    button_switch.Text = "START";
                    communication .closeSerialPort();
                    timer1.Stop();
                }
            }
            catch
            {
                MessageBox.Show("端口错误,请检查串口", "错误");
                //button_switch.BackColor = Color.Green;
                if (button_switch.Text == "START")
                    button_switch.Text = "STOP";
                else
                    button_switch.Text = "START";
            }
        }

        private void Form1_AutoSizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        private void InitTimerLed()
        {
            int interval = 500;
            timerLed = new System.Timers.Timer(interval);
            timerLed.Interval = interval;
            timerLed.AutoReset = true;
            timerLed.Enabled = true;
            timerLed.Elapsed += new System.Timers.ElapsedEventHandler(TimesUp);
        }
        private void TimesUp(object sender, System.Timers.ElapsedEventArgs e)
        {
           

        }

        private void InitTimerDataSave()
        {
            int interval = 1500;
            timerDataSave = new System.Timers.Timer(interval);
            timerDataSave.Interval = interval;
            timerDataSave.AutoReset = true;
            timerDataSave.Enabled = false;
            timerDataSave.Elapsed += new System.Timers.ElapsedEventHandler(TimesDataSaveUp);
        }
        private void TimesDataSaveUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            storage_str = communication.getDataSaveStr();
            StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, append: true);
            streamWriter.Write(storage_str);
            streamWriter.Close();
            storage_str = String.Empty;
            Event.setValue(4);
        }



        //曲线初始化函数1
        private void Init_IQ_line(ref Paint gyroPaint)//画IQ曲线的两个图
        {
            //按钮选择数组
            gyroPaint.SA_IQ_checkboxs[0] = SA_IQ_IcheckBox;
            gyroPaint.SA_IQ_checkboxs[1] = SA_IQ_QcheckBox;
            gyroPaint.SA_IQ_checkboxs[2] = SA_IQ_AcheckBox;

            gyroPaint.SB_IQ_checkboxs[0] = SB_IQ_IcheckBox;
            gyroPaint.SB_IQ_checkboxs[1] = SB_IQ_QcheckBox;
            gyroPaint.SB_IQ_checkboxs[2] = SB_IQ_AcheckBox;

            gyroPaint.SA_IQ_lines[0] = plt1.Plot.AddSignal(gyroPaint.IA_array);
            gyroPaint.SA_IQ_lines[1] = plt1.Plot.AddSignal(gyroPaint.QA_array);
            gyroPaint.SA_IQ_lines[2] = plt1.Plot.AddSignal(gyroPaint.AMPA_array);
            plt1.Render();
            gyroPaint.SB_IQ_lines[0] = plt2.Plot.AddSignal(gyroPaint.IB_array);
            gyroPaint.SB_IQ_lines[1] = plt2.Plot.AddSignal(gyroPaint.QB_array);
            gyroPaint.SB_IQ_lines[2] = plt2.Plot.AddSignal(gyroPaint.AMPB_array);
            plt2.Render();
            plt1.Plot.XLabel("时间");
            plt1.Plot.YLabel("幅值mV");
            plt2.Plot.XLabel("时间");
            plt2.Plot.YLabel("幅值mV");
            plt1.Plot.AxisAuto();
            plt2.Plot.AxisAuto();
        } 
        private void Init_scanfre_line1()
        {
            scatterChat11 = plt21.Plot.AddScatterList(markerSize: 1);
            //scatterChat11.AddRange(chatXs,chatYs);
            scatterChat21 = plt22.Plot.AddScatterList(markerSize:1);
            //scatterChat21.AddRange(chatXs, chatYs);
            scatterChat12 = plt21.Plot.AddScatterList(markerSize: 1);
            //scatterChat12.AddRange(chatXs, chatYs);
            scatterChat22 = plt22.Plot.AddScatterList(markerSize: 1);
            //scatterChat22.AddRange(chatXs, chatYs);
            
            plt21.Refresh();
            plt22.Refresh();

        }

        private void Init_XY_lineEnable()
        {
            vLine11 = plt21.Plot.AddVerticalLine(((Convert.ToDouble(G1_hfre_textBox.Text)) + (Convert.ToDouble(G1_lfre_textBox.Text))) / 2.0);
            vLine21 = plt22.Plot.AddVerticalLine(((Convert.ToDouble(G1_hfre_textBox.Text)) + (Convert.ToDouble(G1_lfre_textBox.Text))) / 2.0);
            hLine12 = plt21.Plot.AddHorizontalLine(0);
            hLine22 = plt22.Plot.AddHorizontalLine(0);
            vLine11.DragEnabled = true;
            vLine21.DragEnabled = true;
            hLine12.DragEnabled = true;
            hLine22.DragEnabled = true;
            plt21.Plot.AxisAuto();
            plt22.Plot.AxisAuto();
            plt21.Refresh();
            plt22.Refresh();
        }

        private void Init_XY_lineDisable()
        {
            plt21.Plot.Remove(vLine11);
            plt22.Plot.Remove(vLine21);
            plt21.Plot.Remove(hLine12);
            plt22.Plot.Remove(hLine22);
            plt21.Refresh();
            plt22.Refresh();
        }


        private void Init_PLL_line(ref Paint gyroPaint)
        {
         
            gyroPaint.PLL_Amp_checkboxs[0] = checkBox1;
            gyroPaint.PLL_Amp_checkboxs[1] = checkBox2;

            gyroPaint.PLL_Phase_checkbox = checkBox9;

            gyroPaint.PLL_SA_IQ_checkboxs[0] = checkBox3;
            gyroPaint.PLL_SA_IQ_checkboxs[1] = checkBox4;
            gyroPaint.PLL_SA_IQ_checkboxs[2] = checkBox5;

            gyroPaint.PLL_SB_IQ_checkboxs[0] = checkBox6;
            gyroPaint.PLL_SB_IQ_checkboxs[1] = checkBox7;
            gyroPaint.PLL_SB_IQ_checkboxs[2] = checkBox8;

            gyroPaint.PLL_SA_IQ_lines[0] = plt6.Plot.AddSignal(gyroPaint.IA_array);
            gyroPaint.PLL_SA_IQ_lines[1] = plt6.Plot.AddSignal(gyroPaint.QA_array);
            gyroPaint.PLL_SA_IQ_lines[2] = plt6.Plot.AddSignal(gyroPaint.AMPA_array);
            plt6.Render();
            gyroPaint.PLL_SB_IQ_lines[0] = plt7.Plot.AddSignal(gyroPaint.IB_array);
            gyroPaint.PLL_SB_IQ_lines[1] = plt7.Plot.AddSignal(gyroPaint.QB_array);
            gyroPaint.PLL_SB_IQ_lines[2] = plt7.Plot.AddSignal(gyroPaint.AMPB_array);
            plt7.Render();
            plt6.Plot.XLabel("时间");
            plt6.Plot.YLabel("幅值mV");
            plt7.Plot.XLabel("时间");
            plt7.Plot.YLabel("幅值mV");
            plt6.Plot.AxisAuto();
            plt7.Plot.AxisAuto();

            gyroPaint.PLL_Amp_lines[0] = plt5.Plot.AddSignal(gyroPaint.PLL_current_error_array);
            gyroPaint.PLL_Amp_lines[1] = plt5.Plot.AddSignal(gyroPaint.PLL_current_fre_array);
            plt5.Render();

            gyroPaint.PLL_Phase_lines = plt8.Plot.AddSignal(gyroPaint.PLL_current_phase_array);
            plt8.Render();

            plt5.Plot.XLabel("时间");
            // plt5.Plot.YLabel("幅值mV");
            plt8.Plot.XLabel("时间");
            //plt8.Plot.YLabel("幅值mV");
            plt8.Plot.AxisAuto();
            plt8.Plot.AxisAuto();
        }

        
        private void Init_PID_line(ref Paint gyroPaint)
        {
            //按钮选择数组
            //gyroPaint.AGC_Amp_checkbox = checkBox10;

            //gyroPaint.AGC_error_Amp_line = plt10.Plot.AddSignal(gyroPaint.AGC_error_Amp_array);
            gyroPaint.PID_checkboxs[0] = checkBox11;
            gyroPaint.PID_checkboxs[1] = checkBox33;
            gyroPaint.PID_lines[0] = plt10.Plot.AddSignal(gyroPaint.PID_out_Amp_array);
            gyroPaint.PID_lines[1] = plt10.Plot.AddSignal(gyroPaint.PID_error_Amp_array);

            plt10.Render();

            plt10.Plot.XLabel("时间");
            plt10.Plot.YLabel("幅值mV");

            plt10.Plot.AxisAuto();
        }

        //WAM
       private void Init_FA_line(ref Paint gyroPaint)
        {
            
            gyroPaint.FA_EAngel_checkbox = checkBox10;
            gyroPaint.FA_EAngel_line = plt11.Plot.AddSignal(gyroPaint.FA_EAngel_array);
            gyroPaint.FA_EV_checkbox = checkBox12;
            gyroPaint.FA_EV_line = plt12.Plot.AddSignal(gyroPaint.FA_EV_array);
            gyroPaint.FA_QV_checkbox = checkBox13;
            gyroPaint.FA_QV_line = plt13.Plot.AddSignal(gyroPaint.FA_QV_array);
            plt11.Render();
            plt12.Render();
            plt13.Render();
            plt11.Plot.XLabel("时间");
            plt11.Plot.YLabel("角度/°");
            plt11.Plot.AxisAuto();
            plt12.Plot.XLabel("时间");
            plt12.Plot.YLabel("能量");
            plt12.Plot.AxisAuto();
            plt13.Plot.XLabel("时间");
            plt13.Plot.YLabel("正交量");
            plt13.Plot.AxisAuto();
        }




        private void upDataGyro1()
        {
            gyroBuffer1.getValue(Macro.RUNTIME_FIELD_AMP_CHA_I).CopyTo(gyroPaint1.IA_array, 0);
            gyroBuffer1.getValue(Macro.RUNTIME_FIELD_AMP_CHA_Q).CopyTo(gyroPaint1.QA_array, 0);
            gyroBuffer1.getValue(Macro.RUNTIME_FIELD_AMP_CHA).CopyTo(gyroPaint1.AMPA_array, 0);
            gyroBuffer1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_I).CopyTo(gyroPaint1.IB_array, 0);
            gyroBuffer1.getValue(Macro.RUNTIME_FIELD_AMP_CHB_Q).CopyTo(gyroPaint1.QB_array, 0);
            gyroBuffer1.getValue(Macro.RUNTIME_FIELD_AMP_CHB).CopyTo(gyroPaint1.AMPB_array, 0);
            switch(communication.getDatatypeG1())
            {
                case 1:
                    
                    break;
                case 2://PLL的相关数据
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_FRE).CopyTo(gyroPaint1.PLL_current_fre_array, 0);
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE).CopyTo(gyroPaint1.PLL_current_phase_array, 0);
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR).CopyTo(gyroPaint1.PLL_current_error_array, 0);

                    PLL_Nowfre_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_FRE).ToString("0.000");
                    PLL_Nowphase_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE).ToString("0.000");
                    PLL_relativeerror_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR).ToString("0.000");
                    //WAM
                    pidEVpowerG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV).ToString("0.000");
                    break;
                case 6:
                    gyroBuffer1.getValue(Macro.RUNTIME_PID_CURRENT_ERROR).CopyTo(gyroPaint1.PID_error_Amp_array, 0);
                    gyroBuffer1.getValue(Macro.RUNTIME_PID_CURRENT_OUTPUT).CopyTo(gyroPaint1.PID_out_Amp_array, 0);
                   
                    pidAmpErrorG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_PID_CURRENT_ERROR).ToString("0.000");
                    pidAmpOutputG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_PID_CURRENT_OUTPUT).ToString("0.000");
                    break;
                //WAM
                //能量控制
                case 19:
                    pidDAdriveG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_DA).ToString("0.000");
                    pidDBdriveG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_DB).ToString("0.000");
                    pidEAangelG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA).ToString("0.000");
                    pidEVpowerG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV).ToString("0.000");
                    pidRATEangelG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_RATE).ToString("0.000");
                    pidEVerrorG1_textBox.Text= gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EVERROR).ToString("0.000");
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA).CopyTo(gyroPaint1.FA_EAngel_array, 0);
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV).CopyTo(gyroPaint1.FA_EV_array, 0);
                    break;
                //正交控制
                case 20:
                    pidDAQuadG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_DAQ).ToString("0.000");
                    pidDBQuadG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_DBQ).ToString("0.000");
                    pidQAangelG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QA).ToString("0.000");
                    pidQVG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QV).ToString("0.000");
                    pidQRATEangelG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QRATE).ToString("0.000");
                    pidQVerrorG1_textBox.Text = gyro1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QVERROR).ToString("0.000");
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EA).CopyTo(gyroPaint1.FA_EAngel_array, 0);
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_EV).CopyTo(gyroPaint1.FA_EV_array, 0);
                    gyroBuffer1.getValue(Macro.RUNTIME_FIELD_FULLANGEL_QV).CopyTo(gyroPaint1.FA_QV_array, 0);

                    break;
                case 15:
                    
                    break;
                case 16:
                   
                    break;
                default:
                    break;

            }
        }

        private void G1_drivefre_button_Click(object sender, EventArgs e)
        {
            try
            {
                gyroPaint1.sweepEnable = false;
                if (G1_drivefre_textBox.Text != string.Empty && Convert.ToDouble(G1_drivefre_textBox.Text) >= 0.01 && Convert.ToDouble(G1_drivefre_textBox.Text) < 2000000)
                {
                    
                    sendBuffers[4] = 1; //ID
                    communication.from_u16_to_u8(sendBuffers, Communication.Downlink_communication[0], 5);//type
                    if (openLoopEnableG1.BackColor == Color.Green)
                        sendBuffers[10] = 1;
                    else sendBuffers[10] = 0;

                    communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(G1_drivefre_textBox.Text) * 4294967296 / 5000000.0), 11);
                    communication.GetCrc_Send(sendBuffers);//CRC
                    Event.setValue(1);//Event    事件在哪个线程 他会不会在一个新的线程里执行

                }
            }
            catch 
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }


        private void G1_MISCbutton_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, Communication.Downlink_communication[8], 5);//type

                double SA_value = Math.PI * Convert.ToDouble(HV_SA_textBox.Text) / 180.0;
                Int32 SA = ((Convert.ToInt16(Math.Sin(SA_value) * (1 << 14))) << 16) | (Convert.ToInt16(Math.Cos(SA_value) * (1 << 14)));
                communication.from_u32_to_u8(sendBuffers, (UInt32)SA, 7);

                double SB_value = Math.PI * Convert.ToDouble(HV_SB_textBox.Text) / 180;
                Int32 SB = ((Convert.ToInt16(Math.Sin(SB_value) * (1 << 14))) << 16) | (Convert.ToInt16(Math.Cos(SB_value) * (1 << 14)));
                communication.from_u32_to_u8(sendBuffers, (UInt32)SB, 11);

                double CA_value = Math.PI * Convert.ToDouble(HV_CA_textBox.Text) / 180;
                Int32 CA = ((Convert.ToInt16(Math.Sin(CA_value) * (1 << 14))) << 16) | (Convert.ToInt16(Math.Cos(CA_value) * (1 << 14)));
                communication.from_u32_to_u8(sendBuffers, (UInt32)CA, 15);

                double CB_value = Math.PI * Convert.ToDouble(HV_CB_textBox.Text) / 180;
                Int32 value = ((Convert.ToInt16(Math.Sin(CB_value) * (1 << 14))) << 16) | (Convert.ToInt16(Math.Cos(CB_value) * (1 << 14)));
                communication.from_u32_to_u8(sendBuffers, (UInt32)value, 19);

                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            //string str = String.Join(" ", sendBuffers);
            //textBox89.AppendText(str + "\r\n");
        }

        

        private void radioButton1_Click(object sender, EventArgs e)//扫频里的正反扫
        {
            if (rb1)
            {
                radioButton1.Checked = true;
                rb1 = false;
            }
            else
            {
                radioButton1.Checked = false;
                rb1 = true;
            }
        }

        private void radioButton2_Click(object sender, EventArgs e)//扫频里的单连扫
        {
            if (rb2)
            {
                radioButton2.Checked = true;
                rb2 = false;
            }
            else
            {
                radioButton2.Checked = false;
                rb2 = true;
            }
        }

        private void radioButton3_Click(object sender, EventArgs e)//扫频里的启停开关
        {
            if (rb3)
            {
                radioButton3.Checked = true;
                rb3 = false;
            }
            else
            {
                radioButton3.Checked = false;
                rb3 = true;
            }
        }


        private void G1_scanfre_button_Click(object sender, EventArgs e)//扫频发送
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, Communication.Downlink_communication[3], 5);//type

                byte Control_PON = 0;
                byte Control_SOC = 0;
                byte Control_SOS = 0;
                if (radioButton1.Checked)
                    Control_PON = 1;
                else Control_PON = 0;
                if (radioButton2.Checked)
                    Control_SOC = 1;
                else Control_SOC = 0;
                if (radioButton3.Checked)
                {
                    Control_SOS = 0x01;
                    //fix me
                    gyroPaint1.sweepEnable = true;
                }
                else
                {
                    Control_SOS = 0;
                    //fix me
                    gyroPaint1.sweepEnable = false;
                }

                sendBuffers[7] = Control_SOS;
                sendBuffers[8] = Control_SOC;
                sendBuffers[9] = Control_PON;




                // 单次 0   正扫 0   停 0

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(G1_hfre_textBox.Text) * (4294967295 / 5000000.0)), 15);

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(G1_lfre_textBox.Text) * (4294967295 / 5000000.0)), 11);

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(G1_stepfre_textBox.Text) * (4294967295 / 5000000.0)), 19);


                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(G1_wait_textBox.Text), 23);



                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);

            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

     

        private void button9_Click(object sender, EventArgs e)//最上面的输出栏
        {
            textBox89.Text = string.Empty;  
        }

        private void G1_PLL_button_Click(object sender, EventArgs e)
        {
            try
            {
                gyroPaint1.sweepEnable = false;

                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, Communication.Downlink_communication[4], 5);//type

                switch (comboBox1.Text)
                {
                    case "关":
                        sendBuffers[10] = 0;
                        break;
                    case "SA":
                        sendBuffers[10] = 1;
                        break;
                    case "SB":
                        sendBuffers[10] = 2;
                        break;
                }
                communication.from_u32_to_u8(sendBuffers, (UInt32)(Convert.ToDouble(PLL_Lockphase_textBox.Text) * (536870912 / 180.0)), 11);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(PLL_middlefre_textBox.Text) * (4294967295 / 5000000.0)), 15);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(PLL_Lockpha_Limiteamp_textBox.Text) * (4294967295 / 5000000.0)), 31);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(PLL_P_textBox.Text) * 100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(PLL_I_textBox.Text) * 100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(PLL_D_textBox.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

      


        private void Delete_fre_button_Click(object sender, EventArgs e)
        {
            plt21.Plot.Clear();
            plt22.Plot.Clear();
            plt21.Render();
            plt22.Render();
            Init_scanfre_line1();
            G1_dataLineEnable_button.Text = "开";
            label179.Text = "0.00";
            label183.Text = "0.00";
            label184.Text = "0.00";
            label185.Text = "0.00";
        }

        private void button1_Click(object sender, EventArgs e)//G1PLL
        {
            sendBuffers[4] = 1 ;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type   PLL开启
            sendBuffers[7] = 9;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void G1_HV_button_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, Communication.Downlink_communication[2], 5);//type

                sendBuffers[10] = 1;

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32((Convert.ToDouble(HV1_textBox.Text) + 20) * (65535 / 40.0)), 11);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32((Convert.ToDouble(HV2_textBox.Text) + 20) * (65536 / 40.0)), 15);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32((Convert.ToDouble(HV3_textBox.Text) + 20) * (65536 / 40.0)), 19);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32((Convert.ToDouble(HV4_textBox.Text) + 20) * (65536 / 40.0)), 23);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

      

        private void button2_Click(object sender, EventArgs e)//G1扫频
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 1;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        string storage_str = String.Empty;
        byte storage_count = 0;
        bool storage_OK = false;
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        DateTime saveFailName = DateTime.Now;


        private void storage_button_Click(object sender, EventArgs e)
        {
            if (storage_button.Text == "开始保存")
            {
                storage_str = String.Empty;
                storage_button.Text = "停止保存";
                saveFailName = DateTime.Now;
                saveFileDialog.FileName = saveFailName.Year.ToString() + saveFailName.Month.ToString("00") + saveFailName.Day.ToString("00") + "-" + saveFailName.Hour.ToString("00") + saveFailName.Minute.ToString("00") + saveFailName.Second.ToString("00");
                saveFileDialog.Filter = "(*.txt)|*.txt"; 
                if (saveFileDialog.ShowDialog() == DialogResult.OK)               
                {
                    storage_OK = true;
                    timerDataSave.Start();
                    storage_str = String.Empty;
                    Event.setValue(2);

                }
                else
                {
                    storage_button.Text = "开始保存";
                    storage_str = String.Empty;
                    timerDataSave.Stop();
                    Event.setValue(3);
                }

            }
            else
            {
                timerDataSave.Stop();
                storage_str = communication.getDataSaveStr();
                StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, append: true);
                streamWriter.Write(storage_str);
                streamWriter.Close();
                storage_button.Text = "开始保存";
   
                storage_str = String.Empty;
       
                storage_OK = false;
                Event.setValue(3);
                Event.setValue(4);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sendBuffers[4] = 2;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 1;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sendBuffers[4] = 2;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 9;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void openLoopEnableG1_Click(object sender, EventArgs e)
        {
            if(openLoopEnableG1.BackColor == Color.Red)
                openLoopEnableG1.BackColor = Color.Green;
            else
                openLoopEnableG1.BackColor = Color.Red;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CMD_OPENLOOP_ACT_button_Click_1(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;
                communication.from_u16_to_u8(sendBuffers, Communication.Downlink_communication[1], 5);//type
                sendBuffers[10] = 1;
                communication.from_s32_to_u8(sendBuffers, Convert.ToInt32(Convert.ToDouble(Openloop_AS_textBox.Text) * 32768 / 2500.0), 11);
                communication.from_s32_to_u8(sendBuffers, Convert.ToInt32(Convert.ToDouble(Openloop_AC_textBox.Text) * 32768 / 2500.0), 15);
                communication.from_s32_to_u8(sendBuffers, Convert.ToInt32(Convert.ToDouble(Openloop_BS_textBox.Text) * 32768 / 2500.0), 19);
                communication.from_s32_to_u8(sendBuffers, Convert.ToInt32(Convert.ToDouble(Openloop_BC_textBox.Text) * 32768 / 2500.0), 23);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void pid1EnableG1_Click(object sender, EventArgs e)
        {
            if (pid1EnableG1.BackColor == Color.Red)
                pid1EnableG1.BackColor = Color.Green;
            else if(pid1EnableG1.BackColor == Color.Green)
                pid1EnableG1.BackColor = Color.Blue;
            else
                pid1EnableG1.BackColor = Color.Red;
        }

        private void pid2EnableG1_Click(object sender, EventArgs e)
        {
            if (pid2EnableG1.BackColor == Color.Red)
                pid2EnableG1.BackColor = Color.Green;
            else if (pid2EnableG1.BackColor == Color.Green)
                pid2EnableG1.BackColor = Color.Blue;
            else
                pid2EnableG1.BackColor = Color.Red;
        }

        private void pid3EnableG1_Click(object sender, EventArgs e)
        {
            if (pid3EnableG1.BackColor == Color.Red)
                pid3EnableG1.BackColor = Color.Green;
            else if (pid3EnableG1.BackColor == Color.Green)
                pid3EnableG1.BackColor = Color.Blue;
            else
                pid3EnableG1.BackColor = Color.Red;
        }

        private void pid4EnableG1_Click(object sender, EventArgs e)
        {
            if (pid4EnableG1.BackColor == Color.Red)
                pid4EnableG1.BackColor = Color.Green;
            else if (pid4EnableG1.BackColor == Color.Green)
                pid4EnableG1.BackColor = Color.Blue;
            else
                pid4EnableG1.BackColor = Color.Red;
        }


        private void button8_Click(object sender, EventArgs e)
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 10;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 11;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 12;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 13;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }

        private void G1_pid1_button_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, 50, 5);//type

                if (pid1EnableG1.BackColor == Color.Red)
                    sendBuffers[10] = 0;
                else if (pid1EnableG1.BackColor == Color.Green)
                    sendBuffers[10] = 3;
                else
                    sendBuffers[10] = 4;

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid1LockAmpG1.Text) * 2895), 11);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid1MiddleAmpG1.Text) * (32768 / 2500.0)), 15);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid1LimitAmpG1.Text) * (32768 / 2500.0)), 31);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid1PG1.Text)*100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid1IG1.Text)*100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid1DG1.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void G1_pid2_button_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, 51, 5);//type

                if (pid2EnableG1.BackColor == Color.Red)
                    sendBuffers[10] = 0;
                else if (pid2EnableG1.BackColor == Color.Green)
                    sendBuffers[10] = 3;
                else
                    sendBuffers[10] = 4;

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid2LockAmpG1.Text) * 2895), 11);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid2MiddleAmpG1.Text) * (32768 / 2500.0)), 15);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid2LimitAmpG1.Text) * (32768 / 2500.0)), 31);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid2PG1.Text) * 100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid2IG1.Text) * 100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid2DG1.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void G1_pid3_button_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, 52, 5);//type

                if (pid3EnableG1.BackColor == Color.Red)
                    sendBuffers[10] = 0;
                else if (pid3EnableG1.BackColor == Color.Green)
                    sendBuffers[10] = 3;
                else
                    sendBuffers[10] = 4;

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid3LockAmpG1.Text) * 2895), 11);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid3MiddleAmpG1.Text) * (32768 / 2500.0)), 15);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid3LimitAmpG1.Text) * (32768 / 2500.0)), 31);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid3PG1.Text) * 100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid3IG1.Text) * 100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid3DG1.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void G1_pid4_button_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;

                communication.from_u16_to_u8(sendBuffers, 53, 5);//type

                if (pid4EnableG1.BackColor == Color.Red)
                    sendBuffers[10] = 0;
                else if (pid4EnableG1.BackColor == Color.Green)
                    sendBuffers[10] = 3;
                else
                    sendBuffers[10] = 4;

                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid4LockAmpG1.Text) * 2895), 11);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid4MiddleAmpG1.Text) * (32768 / 2500.0)), 15);
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid4LimitAmpG1.Text) * (32768 / 2500.0)), 31);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid4PG1.Text) * 100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid4IG1.Text) * 100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid4DG1.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

     


       

        private void timer2_Tick(object sender, EventArgs e)
        {
            if(G1_dataLineEnable_button.Text == "关")
            {
                string message11 = $"X={(vLine11.X).ToString("0.00")}";
                string message12 = $"Y={(hLine12.Y).ToString("0.00")}";
                string message21 = $"X={(vLine21.X).ToString("0.00")}";
                string message22 = $"Y={(hLine22.Y).ToString("0.00")}";
                label179.Text = message11;
                label183.Text = message12;
                label185.Text = message21;
                label184.Text = message22;
            }     
        }

      

     


        private void G1_dataLineEnable_button_Click(object sender, EventArgs e)
        {
            if(G1_dataLineEnable_button.Text == "开")
            {
                G1_dataLineEnable_button.Text = "关";
                Init_XY_lineEnable();
            }
            else
            {
                G1_dataLineEnable_button.Text = "开";
                Init_XY_lineDisable();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            port_list.Items.Clear();
            port_list.Items.AddRange(communication.getSerialPortID());
        }

        private void button_switch_TextChanged(object sender, EventArgs e)
        {
            switch (button_switch.Text)
            {
                case "START":
                    timer3.Start();
                    break;
                case "STOP":
                    timer3.Stop();
                    break;
            }
        }

        //WAM
        private void pid5EnableG1_Click(object sender, EventArgs e)
        {
            if (pid5EnableG1.BackColor == Color.Red)
                pid5EnableG1.BackColor = Color.Green;
            else if (pid5EnableG1.BackColor == Color.Green)
                pid5EnableG1.BackColor = Color.Blue;
            else
                pid5EnableG1.BackColor = Color.Red;
        }
        private void pid6EnableG1_Click(object sender, EventArgs e)
        {
            if (pid6EnableG1.BackColor == Color.Red)
                pid6EnableG1.BackColor = Color.Green;
            else if (pid6EnableG1.BackColor == Color.Green)
                pid6EnableG1.BackColor = Color.Blue;
            else
                pid6EnableG1.BackColor = Color.Red;
        }

        //WAM
        //能量环控制参数发送
        private void G1_pid5_button_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;                                 // gyro id = 1

                communication.from_u16_to_u8(sendBuffers, 0x81, 5);   // (pc->ps)type , use to transfer EC data

                // 发送全角模式的设置数据，以下是loopsourse,
                if (pid5EnableG1.BackColor == Color.Red)
                    sendBuffers[10] = 0;
                else if (pid5EnableG1.BackColor == Color.Green)
                    sendBuffers[10] = 5;                            // 5 对应下位机 能量控制环路的  分量分配情况
                else
                    sendBuffers[10] = 6;                            // 6 
                // (32768 / 2500.0)
                // * 2895 *2895
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid5FARefPowerG1.Text) ), 11);        // 参考能量
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid5FAMiddleAmpG1.Text)* (32768 / 2500.0)), 15);   // 中心能量
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid5FALimitAmpG1.Text)* (32768 / 2500.0)), 31);    // 能量限幅
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid5PG1.Text) * 100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid5IG1.Text) * 100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid5DG1.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //正交抑制环控制参数发送
        private void G1_pid6_QCbutton_Click(object sender, EventArgs e)
        {
            try
            {
                sendBuffers[4] = 1;                                 // gyro id = 1

                communication.from_u16_to_u8(sendBuffers, 0x82, 5);   // (pc->ps)type , use to transfer EC data

                // 发送全角模式的设置数据，以下是loopsourse,
                if (pid6EnableG1.BackColor == Color.Red)
                    sendBuffers[10] = 0;
                else if (pid6EnableG1.BackColor == Color.Green)
                    sendBuffers[10] = 7;                            // 5 对应下位机 能量控制环路的  分量分配情况
                else
                    sendBuffers[10] = 7;                            // 6 
                // (32768 / 2500.0)
                // * 2895 *2895
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid6FARefQuadG1.Text)), 11);        // 正交参考
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid6FAMiddleAmpG1.Text) * (32768 / 2500.0)), 15);   // 中心能量
                communication.from_u32_to_u8(sendBuffers, Convert.ToUInt32(Convert.ToDouble(pid6FALimitAmpG1.Text) * (32768 / 2500.0)), 31);    // 能量限幅
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid6PG1.Text) * 100000), 19);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid6IG1.Text) * 100000), 23);
                communication.from_s32_to_u8(sendBuffers, (int)(Convert.ToDouble(pid6DG1.Text) * 100000), 27);
                communication.GetCrc_Send(sendBuffers);
                Event.setValue(1);
            }
            catch
            {
                MessageBox.Show("\t数据不合法\r\n 有特殊字符/数字太大/数字太小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {

        }
        // WAM
        // 开启全角能量环 数据显示，标志位19，让下位机发送 全角模式能量输出、误差值、角度
        private void button3_Click_1(object sender, EventArgs e)
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 19;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }
        //开启正交环 数据显示，标志位20，让下位机发送正交量、误差值、角度
        private void button4_Click_1(object sender, EventArgs e)
        {
            sendBuffers[4] = 1;
            communication.from_u16_to_u8(sendBuffers, 0x08, 5);//type
            sendBuffers[7] = 20;
            communication.GetCrc_Send(sendBuffers);
            Event.setValue(1);
        }
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        
    }
}
