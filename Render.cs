using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ut64configurator
{
    internal static class Render
    {
        public static  void repaintIQ(ref FormsPlot plt, ref CheckBox[] checkboxs, ref SignalPlot[] lines, byte auto_changeSize)
        {
            switch (checkboxs.Length)
            {
                case 2:
                    if (checkboxs[0].Checked)
                        lines[0].IsVisible = true;
                    else lines[0].IsVisible = false;
                    if (checkboxs[1].Checked)
                        lines[1].IsVisible = true;
                    break;
                case 3:
                    if (checkboxs[0].Checked)
                        lines[0].IsVisible = true;
                    else lines[0].IsVisible = false;
                    if (checkboxs[1].Checked)
                        lines[1].IsVisible = true;
                    else lines[1].IsVisible = false;
                    if (checkboxs[2].Checked)
                        lines[2].IsVisible = true;
                    else lines[2].IsVisible = false;
                    break;
                default:
                    break;
            }
            plt.Render();
            if (auto_changeSize == 1)
                plt.Plot.AxisAuto();
            plt.Plot.Benchmark(enable: true);
        }

        


        public static void repaintTwo(ref FormsPlot plt, ref CheckBox[] checkboxs, ref SignalPlot[] lines, byte auto_changeSize)
        {
            if (checkboxs[0].Checked)
                lines[0].IsVisible = true;
            else lines[0].IsVisible = false;
            if (checkboxs[1].Checked)
                lines[1].IsVisible = true;
            else lines[1].IsVisible = false;

            if (auto_changeSize == 1)
                plt.Plot.AxisAuto();
            plt.Render();
            plt.Plot.Benchmark(enable: true);
        }

        public static void repaintOne(ref FormsPlot plt, ref CheckBox checkbox, ref SignalPlot lines, byte auto_changeSize)
        {
            if (checkbox.Checked)
                lines.IsVisible = true;
            else lines.IsVisible = false;
            if (auto_changeSize == 1)
                plt.Plot.AxisAuto();
            plt.Render();
            plt.Plot.Benchmark(enable: true);
        }
    }
}
