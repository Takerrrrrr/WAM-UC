using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ut64configurator
{
    internal class Paint
    {
        //绘图IQ
        public double[] IA_array = new double[1000];
        public double[] QA_array = new double[1000];
        public double[] AMPA_array = new double[1000];
        public double[] IB_array = new double[1000];
        public double[] QB_array = new double[1000];
        public double[] AMPB_array = new double[1000];

        //扫频
        public double current_fre = 0;
        public int current_list_length = 0;
        public double[] AMPA_scan_array = new double[10000];
        public double[] AMPB_scan_array = new double[10000];
        public double[] Fre_scan_array = new double[10000];
        public double[] PhaseA_array = new double[10000];
        public double[] PhaseB_array = new double[10000];

        ////PLL
        public double[] PLL_current_fre_array = new double[1000];
        public double[] PLL_current_phase_array = new double[1000];
        public double[] PLL_current_error_array = new double[1000];


        //PID style 2
        public double[] PID_error_Amp_array = new double[1000];
        public double[] PID_out_Amp_array = new double[1000];

        public double[] AGC_min_output_array = new double[1000];
        public double[] Fre_resonant_array = new double[1000];

        //WAM
        public double[] FA_EAngel_array = new double[1000];
        public double[] FA_EV_array = new double[1000];
        public double[] FA_QV_array = new double[1000];

        //绘图选择按钮
        public CheckBox[] SA_IQ_checkboxs = new CheckBox[3];
        public CheckBox[] SB_IQ_checkboxs = new CheckBox[3];
        //扫频
        public CheckBox[] Sacnfre_Amp_checkboxs = new CheckBox[2];
        public CheckBox[] Sacnfre_Phase_checkboxs = new CheckBox[2];
        //PLL
        public CheckBox[] PLL_Amp_checkboxs = new CheckBox[2];
        public CheckBox PLL_Phase_checkbox;

        public CheckBox[] PLL_SA_IQ_checkboxs = new CheckBox[3];
        public CheckBox[] PLL_SB_IQ_checkboxs = new CheckBox[3];

        //PID style2
        public CheckBox[] PID_checkboxs = new CheckBox[2];

        public CheckBox Fre_resonant_checkbox;
        public CheckBox AGC_min_output_checkbox;

        //WAM
        public CheckBox FA_EAngel_checkbox;
        public CheckBox FA_EV_checkbox;
        public CheckBox FA_QV_checkbox;

        //绘图曲线IQ 
        public SignalPlot[] SA_IQ_lines = new SignalPlot[3];
        public SignalPlot[] SB_IQ_lines = new SignalPlot[3];
       
        public byte IQlines_sizeChange_auto = 1;

        //扫频曲线
        public SignalPlotXY[] Fre_scan_Amp_lines = new SignalPlotXY[2];
        public SignalPlotXY[] Fre_scan__Phase_lines = new SignalPlotXY[2];
        public byte APlines_sizeChange_auto = 1;
        public bool sweepEnable = false;

        //PLL曲线
        public SignalPlot[] PLL_Amp_lines = new SignalPlot[2];
        public SignalPlot PLL_Phase_lines;
        public SignalPlot[] PLL_SA_IQ_lines = new SignalPlot[3];
        public SignalPlot[] PLL_SB_IQ_lines = new SignalPlot[3];

        //PID style 2
        public SignalPlot[] PID_lines = new SignalPlot[2];

        //
        public SignalPlot Fre_resonant_line;
        public SignalPlot AGC_min_output_line;

        //WAM
        public SignalPlot FA_EAngel_line;
        public SignalPlot FA_EV_line;
        public SignalPlot FA_QV_line;
    }
}
