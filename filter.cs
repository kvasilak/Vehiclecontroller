using System;
using Microsoft.SPOT;

namespace vehiclecontroller
{
    class CFilter
    {
        private int FilterShift = 8; // Parameter K
        private Int32 filter_reg; // Delay element – 32 bits
        private Int16 filter_output; // Filter output – 16 bits

        public CFilter()
        {
            //default to 8
           // FilterShift = 8;
        }

        public CFilter(int rate)
        {
            FilterShift = rate;
        }

        //Low pass filter
        public Int16 LowPass(Int16 input)
        {
            // Update filter with current sample.
            filter_reg = filter_reg - (filter_reg >> FilterShift) + input;

            // Scale output for unity gain.
            filter_output = (Int16)(filter_reg >> FilterShift);

            return filter_output;
        }

        //Force the filter to a specific value
        public void OverrideFilter(Int16 newval)
        {
            filter_reg = ((newval + 2) << FilterShift);
        }
    }
}