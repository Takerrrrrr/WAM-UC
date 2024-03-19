using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ut64configurator
{
    internal class GyroBuffer
    {
        //绘图IQ
        private List<double> IA_array = new List<double>();
        private List<double> QA_array = new List<double>();
        private List<double> AMPA_array = new List<double>();
        private List<double> IB_array = new List<double>();
        private List<double> QB_array = new List<double>();
        private List<double> AMPB_array = new List<double>();

        //扫频
        private List<double> AMPA_scan_array = new List<double>();
        private List<double> AMPB_scan_array = new List<double>();
        private List<double> Fre_scan_array = new List<double>();
        private List<double> PhaseA_array = new List<double>();
        private List<double> PhaseB_array = new List<double>();

        ////PLL
        private List<double> PLL_current_fre_array = new List<double>();
        private List<double> PLL_current_phase_array = new List<double>();
        private List<double> PLL_current_error_array = new List<double>();

        //PID style 2
        private List<double> PID_error_Amp_array = new List<double>();
        private List<double> PID_out_Amp_array = new List<double>();



        //谐振频率
        private List<double> AGC_min_output_array = new List<double>();
        private List<double> Fre_resonant_array = new List<double>();

        //WAM  全角
        //能量控制
        private List<double> FA_EAngel_array = new List<double>();      // 绘制驻波角输出
        private List<double> FA_EV_array = new List<double>();          // 用来绘制能量曲线
        //正交控制
        private List<double> FA_QV_array = new List<double>();          // 用来绘制正交量曲线

        private int arraryLength = 1000;


        public List<double> getValue(byte type)
        {

            switch (type)
            {

                case Macro.RUNTIME_FIELD_AMP_CHA_I:  //get IQ  value
                    return IA_array;
                case Macro.RUNTIME_FIELD_AMP_CHA_Q:
                    return QA_array;
                case Macro.RUNTIME_FIELD_AMP_CHA:
                    return AMPA_array;
                case Macro.RUNTIME_FIELD_AMP_CHB_I:
                    return IB_array;
                case Macro.RUNTIME_FIELD_AMP_CHB_Q:
                    return QB_array;
                case Macro.RUNTIME_FIELD_AMP_CHB:
                    return AMPB_array;
                case Macro.RUNTIME_FIELD_PHASE_A:
                    return PhaseA_array;
                case Macro.RUNTIME_FIELD_PHASE_B:
                    return PhaseB_array;
                case Macro.RUNTIME_FIELD_CURRENT_FREQUENCY:
                    return Fre_scan_array;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHA:
                    return AMPA_scan_array;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHB:
                    return AMPB_scan_array;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_FRE:
                    return PLL_current_fre_array;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR:
                    return PLL_current_error_array;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE:
                    return PLL_current_phase_array;
                case Macro.RUNTIME_PID_CURRENT_ERROR:  //PID style 2
                    return PID_error_Amp_array;
                case Macro.RUNTIME_PID_CURRENT_OUTPUT:
                    return PID_out_Amp_array;
                case Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY:  //
                    return Fre_resonant_array;
                case Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT:
                    return AGC_min_output_array;
                //WAM
                case Macro.RUNTIME_FIELD_FULLANGEL_EA:
                    return FA_EAngel_array;
                case Macro.RUNTIME_FIELD_FULLANGEL_EV:
                    return FA_EV_array;
                case Macro.RUNTIME_FIELD_FULLANGEL_QV:
                    return FA_QV_array;
                default:
                    return null;
            }

        }
        public void setValue(byte type, double value)
        {
            switch (type)
            {
                case Macro.RUNTIME_FIELD_AMP_CHA_I:
                    list_removehead(IA_array);
                    IA_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHA_Q:
                    list_removehead(QA_array);
                    QA_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHA:
                    list_removehead(AMPA_array);
                    AMPA_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHB_I:
                    list_removehead(IB_array);
                    IB_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHB_Q:
                    list_removehead(QB_array);
                    QB_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHB:
                    list_removehead(AMPB_array);
                    AMPB_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_PHASE_A:
                    list_removehead(PhaseA_array);
                    PhaseA_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_PHASE_B:
                    list_removehead(PhaseB_array);
                    PhaseB_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_CURRENT_FREQUENCY:
                    list_removehead(Fre_scan_array);
                    Fre_scan_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHA:
                    list_removehead(AMPA_scan_array);
                    AMPA_scan_array.Add(value);
                    break;
                
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHB:
                    list_removehead(AMPB_scan_array);
                    AMPB_scan_array.Add(value);
                    break;
    
                case Macro.RUNTIME_FIELD_PLL_CURRENT_FRE:
                    list_removehead(PLL_current_fre_array);
                    PLL_current_fre_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR:
                    list_removehead(PLL_current_error_array);
                    PLL_current_error_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE:
                    list_removehead(PLL_current_phase_array);
                    PLL_current_phase_array.Add(value);
                    break;
                case Macro.RUNTIME_PID_CURRENT_ERROR:
                    list_removehead(PID_error_Amp_array);
                    PID_error_Amp_array.Add(value);
                    break;
                case Macro.RUNTIME_PID_CURRENT_OUTPUT:
                    list_removehead(PID_out_Amp_array);
                    PID_out_Amp_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY:
                    list_removehead(Fre_resonant_array);
                    Fre_resonant_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT:
                    list_removehead(AGC_min_output_array);
                    AGC_min_output_array.Add(value);
                    break;
                //WAM
                case Macro.RUNTIME_FIELD_FULLANGEL_EA:
                    list_removehead(FA_EAngel_array);
                    FA_EAngel_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_EV:
                    list_removehead(FA_EV_array);
                    FA_EV_array.Add(value);
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_QV:
                    list_removehead(FA_QV_array);
                    FA_QV_array.Add(value);
                    break;
                default:
                    break;
            }
        }
        private void list_removehead(List<double> list_remove)
        {
            if (list_remove.Count >= arraryLength)
                list_remove.RemoveAt(0);
        }

    }
}
