using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ut64configurator
{
    internal class Macro
    {

        public const byte RUNTIME_FIELD_AMP_CHA_I = 1;
        public const byte RUNTIME_FIELD_AMP_CHA_Q = 2;
        public const byte RUNTIME_FIELD_AMP_CHA = 3;
        public const byte RUNTIME_FIELD_AMP_CHB_I = 4;
        public const byte RUNTIME_FIELD_AMP_CHB_Q = 5;
        public const byte RUNTIME_FIELD_AMP_CHB = 6;

        public const byte RUNTIME_FIELD_CURRENT_FREQUENCY = 10; // from SWEEPER and PLL
        public const byte RUNTIME_FIELD_FORMER_FREQUENCY = 12; // from SWEEPER and PLL
        public const byte RUNTIME_FIELD_AMP_SCAN_CHA = 11;          // sweeper
        public const byte RUNTIME_FIELD_AMP_SCAN_CHB = 14;          // sweeper
        public const byte RUNTIME_FIELD_PHASE_A = 7;   // read from AXI
        public const byte RUNTIME_FIELD_PHASE_B = 8;   // read from AXI

        public const byte RUNTIME_FIELD_PLL_CURRENT_ERROR = 21; // SM PLL
        public const byte RUNTIME_FIELD_PLL_CURRENT_PHASE = 22; // SM PLL
        public const byte RUNTIME_FIELD_PLL_CURRENT_FRE = 23; // SM PLL

        //PID style 2

        public const byte RUNTIME_PID_CURRENT_ERROR = 71;  // 
        public const byte RUNTIME_PID_CURRENT_OUTPUT = 72; // 

        //Q 值
        public const byte RUNTIME_FIELD_QFACTOR_START_FREQUENCY = 81;
        public const byte RUNTIME_FIELD_QFACTOR_SHORT_TAU = 82;
        public const byte RUNTIME_FIELD_QFACTOR_LONG_TAU = 83;
        public const byte RUNTIME_FIELD_QFACTOR_MIN = 84;
        public const byte RUNTIME_FIELD_QFACTOR_MAX = 85;

        //谐振频率
        public const byte RUNTIME_FIELD_EST_RESONANT_FREQUENCY = 91;
        public const byte RUNTIME_FIELD_MIN_AGC_OUTPUT = 92;


        //
        public const byte RUNTIME_FIELD_MODEMATCH_AMP_LEFT = 101;
        public const byte RUNTIME_FIELD_MODEMATCH_AMP_RIGHT = 102;

        //WAM
        //能量控制
        public const byte RUNTIME_FIELD_FULLANGEL_DA = 111;
        public const byte RUNTIME_FIELD_FULLANGEL_DB = 112;
        public const byte RUNTIME_FIELD_FULLANGEL_EV = 113;
        public const byte RUNTIME_FIELD_FULLANGEL_EVERROR = 114;
        public const byte RUNTIME_FIELD_FULLANGEL_EA = 115;
        public const byte RUNTIME_FIELD_FULLANGEL_RATE = 116;
        //正交控制
        public const byte RUNTIME_FIELD_FULLANGEL_DAQ= 121;
        public const byte RUNTIME_FIELD_FULLANGEL_DBQ= 122;
        public const byte RUNTIME_FIELD_FULLANGEL_QV= 123;
        public const byte RUNTIME_FIELD_FULLANGEL_QVERROR = 124;
        public const byte RUNTIME_FIELD_FULLANGEL_QA = 125;
        public const byte RUNTIME_FIELD_FULLANGEL_QRATE = 126;


        // 2^29;角度标定值
        public const float angle_factor = 536870912;
    }
}
