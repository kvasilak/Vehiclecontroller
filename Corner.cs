/*************************************************************************
**   Vehicle Body controller for theFEZ Panda II and .net MH hardware
**
** Copyright 2011 Keith Vasilakes
**
** This file is part of Vehicle Body controller, VBC.
**
** VBC is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
** License as published by the Free Software Foundation, either version 3 of the License, or (at your option) 
** any later version.
**
** VBC is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the 
** implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public 
** License for more details.
**
** You should have received a copy of the GNU General Public License along with MegaRide. If not, see
** http://www.gnu.org/licenses/.
**
**************************************************************************/
using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using System.Threading;

namespace vehiclecontroller
{


    class Corner
    {
        // Public

        //Private
        private const int DeadBand =5;			//how far the corner has to move before we try to adjust the position
        private const int HoldDeadBand =10; 	//deadband + hysterysis
        private ValveOp State;
        private CFilter SetPointFilter;
        private CFilter SlowHeightFilter;
        private CFilter FastHeightFilter;
        private enum ValveOp { Fill, Dump, Hold };

        private Position MyCorner;
        private Hardware TheHW;

        private Int16 SlowHeight;
        private Int16 FastHeight;
        private Int16 SetPoint;

        public Corner(Position ThisCorner)
        {
            MyCorner = ThisCorner;

            TheHW = Hardware.Instance;

            SetPointFilter = new CFilter(1);        //just a little filtering
            SlowHeightFilter = new CFilter();       //a lot of filtering
            FastHeightFilter = new CFilter(1);      //barely any filtering

            // create a thread handler
            Thread MyThreadHandler;

            // create a new thread object
            //and assign to my handler
            MyThreadHandler = new Thread(Run);
            // start my new thread
            MyThreadHandler.Start();
            /////////////////////////////////
            // Do anything else you like to do here

            //Thread.Sleep(Timeout.Infinite);
        }
        
        public const Int64 ticks_per_millisecond = System.TimeSpan.TicksPerMillisecond;

        public static long GetCurrentTimeInTicks()
        {
            return Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
        }

        //this is the main thread for height control
        //Each corner runs in it's own thread with it's own data
        private void Run()
        {
            long start;

            long span;
            

            while (true)
            {
                start = GetCurrentTimeInTicks();
                SetPoint = SetPointFilter.LowPass(TheHW.GetHeight(Position.SetHeight));
                //setpoint = TheHW.GetHeight(Position.SetHeight);
                
                //This corner height
                Int16 height = TheHW.GetHeight(MyCorner);

                FastHeight = FastHeightFilter.LowPass(height);
                SlowHeight = SlowHeightFilter.LowPass(height);

                //Inflate or deflate acording to Height or pressure 
                //depending on other factors such as vehicle speed
                SetHeight();

                span = GetCurrentTimeInTicks() - start;

                Thread.Sleep(100);
            }
        }

        

        //set the ride height, if necessary that is
        private void SetHeight()
        {
            switch (State)
            {
                case ValveOp.Hold:

                    //Debug.Print("hold, " + MyCorner.ToString() + "; set;" + SetPoint.ToString() + ", Height" + SlowHeight.ToString());

                    //react slowly if already within deadband
                    if (SlowHeight < (SetPoint - HoldDeadBand))
                    {
                        State = ValveOp.Fill;
                        TheHW.Fill(MyCorner, Solenoid.Open);

                        //Over ride the filter, force it to the current value
                        //So the hold state doesn't keep changing
                        SlowHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                        FastHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                    }
                    else if (SlowHeight > (SetPoint + HoldDeadBand))
                    {
                        State = ValveOp.Dump;
                        TheHW.Dump(MyCorner, Solenoid.Open);

                        //Over ride the filter, force it to the current value
                        //So the hold state doesn't keep changing
                        SlowHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                        FastHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                    }
                    break;
                case ValveOp.Fill:

                    //Debug.Print("Fill, " + MyCorner.ToString() + "; set;" + SetPoint.ToString() + ", Height" + FastHeight.ToString());
                    //Respond quickly when filling
                    if (FastHeight > (SetPoint - DeadBand))
                    {
                        State = ValveOp.Hold;
                        TheHW.Fill(MyCorner, Solenoid.Closed);

                        //Over ride the filter, force it to the current value
                        //So the hold state doesn't keep changing
                        SetPointFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                        SlowHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                        FastHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                    }
                    break;
                case ValveOp.Dump:
                    //Debug.Print("Dump, " + MyCorner.ToString() + "; set;" + SetPoint.ToString() + ", Height" + FastHeight.ToString());

                    //respond quickly when dumping
                    if (FastHeight < (SetPoint + DeadBand))
                    {
                        State = ValveOp.Hold;
                        TheHW.Dump(MyCorner, Solenoid.Closed);

                        //Over ride the filter, force it to the current value
                        //So the hold state doesn't keep changing
                        SetPointFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                        SlowHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                        FastHeightFilter.OverrideFilter(TheHW.GetHeight(MyCorner));
                    }
                    break;
                default:
                    State = ValveOp.Hold;

                    TheHW.Fill(MyCorner, Solenoid.Closed);
                    TheHW.Dump(MyCorner, Solenoid.Closed);
                    break;
            }
        }
    }

}
