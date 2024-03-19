using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ut64configurator
{
    internal class Gyro
    {
        private double amp_cha_i = 0;
        private double amp_cha_q = 0;
        private double amp_cha = 0;
        private double amp_chb_i = 0;
        private double amp_chb_q = 0;
        private double amp_chb = 0;
        
        // data from sweeper and pll
        private double currentFrequency = 0; // variable
                                             // sweeper data
        private UInt32 formerFrequency = 0;
        private double phase_a = 0;
        private double phase_b = 0;

        private double amp_scan_a = 0;
        private double amp_scan_b = 0;

        // PLL data
        private double pllCurrentError = 0;
        private double pllCurrentPhase = 0;
        private double pllCurrentFre = 0;


        //PID style 2
        private double pidCurrentError = 0;
        private double pidCurrentOutput = 0;

        //Q值
        private double qfactorStartFrequency = 0;
        private double qfactorShortTau = 0;
        private double qfactorLongTau = 0;
        private double qfactorMin = 0;
        private double qfactorMax = 0;

        //谐振频率
        private double estResonantFrequency = 0;
        private double minAgcOutput = 0;

        private double leftAmp = 0;
        private double rightAmp = 0;
        //
        //WAM
        //能量控制
        private double ampDA = 0;
        private double ampDB = 0;
        private double gyroangel = 0;
        private double gyropower = 0;
        private double gyropowererror = 0;
        private double gyroangelrate = 0;
        //正交控制
        private double ampDAQ = 0;
        private double ampDBQ = 0;
        private double gyroangelQ = 0;
        private double gyropowerQ = 0;
        private double gyropowererrorQ = 0;
        private double gyroangelrateQ = 0;



        public double getValue( byte type)
        {
            
            switch(type)
            {
                
                case Macro.RUNTIME_FIELD_AMP_CHA_I:  //get IQ  value
                    return amp_cha_i;  
                case Macro.RUNTIME_FIELD_AMP_CHA_Q:
                    return amp_cha_q;
                case Macro.RUNTIME_FIELD_AMP_CHA:
                    return amp_cha;
                case Macro.RUNTIME_FIELD_AMP_CHB_I:
                    return amp_chb_i;
                case Macro.RUNTIME_FIELD_AMP_CHB_Q:
                    return amp_chb_q;
                case Macro.RUNTIME_FIELD_AMP_CHB:
                    return amp_chb;
                case Macro.RUNTIME_FIELD_PHASE_A:
                    return phase_a;
                case Macro.RUNTIME_FIELD_PHASE_B: 
                    return phase_b;
                case Macro.RUNTIME_FIELD_CURRENT_FREQUENCY:
                    return currentFrequency;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHA:
                    return amp_scan_a;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHB:
                    return amp_scan_b;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR:
                    return pllCurrentError;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE:
                    return pllCurrentPhase;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_FRE:
                    return pllCurrentFre;
                case Macro.RUNTIME_PID_CURRENT_ERROR:  // PID style 2
                    return pidCurrentError;
                case Macro.RUNTIME_PID_CURRENT_OUTPUT:
                    return pidCurrentOutput;
                case Macro.RUNTIME_FIELD_QFACTOR_START_FREQUENCY: 
                    return qfactorStartFrequency;
                case Macro.RUNTIME_FIELD_QFACTOR_SHORT_TAU:
                    return qfactorShortTau;
                case Macro.RUNTIME_FIELD_QFACTOR_LONG_TAU:
                    return qfactorLongTau;
                case Macro.RUNTIME_FIELD_QFACTOR_MIN:
                    return qfactorMin;
                case Macro.RUNTIME_FIELD_QFACTOR_MAX:
                    return qfactorMax;
                case Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY:
                    return estResonantFrequency;
                case Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT:
                    return minAgcOutput;
                case Macro.RUNTIME_FIELD_MODEMATCH_AMP_LEFT:
                    return leftAmp;
                case Macro.RUNTIME_FIELD_MODEMATCH_AMP_RIGHT:
                    return rightAmp;
                //WAM
                //能量控制
                case Macro.RUNTIME_FIELD_FULLANGEL_DA:
                    return ampDA;
                case Macro.RUNTIME_FIELD_FULLANGEL_DB:
                    return ampDB;
                case Macro.RUNTIME_FIELD_FULLANGEL_EA:
                    return gyroangel;
                case Macro.RUNTIME_FIELD_FULLANGEL_EV:
                    return gyropower;
                case Macro.RUNTIME_FIELD_FULLANGEL_EVERROR:
                    return gyropowererror;
                case Macro.RUNTIME_FIELD_FULLANGEL_RATE:
                    return gyroangelrate;
                //正交控制
                case Macro.RUNTIME_FIELD_FULLANGEL_DAQ:
                    return ampDAQ;
                case Macro.RUNTIME_FIELD_FULLANGEL_DBQ:
                    return ampDBQ;
                case Macro.RUNTIME_FIELD_FULLANGEL_QA:
                    return gyroangelQ;
                case Macro.RUNTIME_FIELD_FULLANGEL_QV:
                    return gyropowerQ;
                case Macro.RUNTIME_FIELD_FULLANGEL_QVERROR:
                    return gyropowererrorQ;
                case Macro.RUNTIME_FIELD_FULLANGEL_QRATE:
                    return gyroangelrateQ;
                default:
                    return 0;
            }
        }


        public void setValue(byte type,double value)
        {
            switch (type)
            {
                case Macro.RUNTIME_FIELD_AMP_CHA_I:
                    amp_cha_i = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHA_Q:
                    amp_cha_q = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHA:
                    amp_cha = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHB_I:
                    amp_chb_i = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHB_Q:
                    amp_chb_q = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_CHB:
                    amp_chb = value;
                    break;
                case Macro.RUNTIME_FIELD_PHASE_A:
                    phase_a = value;
                    break;
                case Macro.RUNTIME_FIELD_PHASE_B:
                    phase_b = value;
                    break;
                case Macro.RUNTIME_FIELD_CURRENT_FREQUENCY:
                    currentFrequency = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHA:
                    amp_scan_a = value;
                    break;
                case Macro.RUNTIME_FIELD_AMP_SCAN_CHB:
                    amp_scan_b = value;
                    break;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_FRE:
                    pllCurrentFre = value;
                    break;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_ERROR:
                    pllCurrentError = value;
                    break;
                case Macro.RUNTIME_FIELD_PLL_CURRENT_PHASE:
                    pllCurrentPhase = value;
                    break;
                case Macro.RUNTIME_PID_CURRENT_ERROR:  //PID style 2
                    pidCurrentError = value;
                    break;
                case Macro.RUNTIME_PID_CURRENT_OUTPUT:
                    pidCurrentOutput = value;
                    break;
                case Macro.RUNTIME_FIELD_QFACTOR_START_FREQUENCY:
                    qfactorStartFrequency = value;
                    break;
                case Macro.RUNTIME_FIELD_QFACTOR_SHORT_TAU:
                    qfactorShortTau = value;
                    break;
                case Macro.RUNTIME_FIELD_QFACTOR_LONG_TAU:
                    qfactorLongTau = value;
                    break;
                case Macro.RUNTIME_FIELD_QFACTOR_MIN:
                    qfactorMin = value;
                    break;
                case Macro.RUNTIME_FIELD_QFACTOR_MAX:
                    qfactorMax = value;
                    break;
                case Macro.RUNTIME_FIELD_EST_RESONANT_FREQUENCY:
                    estResonantFrequency = value;
                    break;
                case Macro.RUNTIME_FIELD_MIN_AGC_OUTPUT:
                    minAgcOutput = value;
                    break;
                case Macro.RUNTIME_FIELD_MODEMATCH_AMP_LEFT:
                    leftAmp = value;
                    break;
                case Macro.RUNTIME_FIELD_MODEMATCH_AMP_RIGHT:
                    rightAmp = value;
                    break;
                //WAM  
                //能量控制
                case Macro.RUNTIME_FIELD_FULLANGEL_DA:
                    ampDA = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_DB:
                    ampDB = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_EA:
                    gyroangel = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_EV:
                    gyropower = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_EVERROR:
                    gyropowererror = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_RATE:
                    gyroangelrate = value;
                    break;
                //正交控制
                case Macro.RUNTIME_FIELD_FULLANGEL_DAQ:
                    ampDAQ = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_DBQ:
                    ampDBQ = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_QA:
                    gyroangelQ = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_QV:
                    gyropowerQ = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_QVERROR:
                    gyropowererrorQ = value;
                    break;
                case Macro.RUNTIME_FIELD_FULLANGEL_QRATE:
                    gyroangelrateQ = value;
                    break;
                default:
                    break;
            }
        }

        public void setValuefre(uint data)
        {
            formerFrequency = data;
        }
        public uint getValuefre()
        {
            return formerFrequency;
        }

    }
}
