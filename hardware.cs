using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

using System.ComponentModel;

namespace vehiclecontroller
{
    public enum Position { 
        LeftFront, 
        RightFront, 
        LeftRear, 
        RightRear, 
        SetHeight, 
        SetTilt };
  
    public enum LEDS { OK_LED, Error_LED, IN_Position_LED };

    public enum Solenoid { Open, Closed };

    //A singelton//
    public sealed class Hardware
    {
        private static readonly Hardware instance = new Hardware();

        private static AnalogIn  LFHeight;
        private static AnalogIn  RFHeight;
        private static AnalogIn  LRHeight;
        private static AnalogIn  RRHeight;
        private static AnalogIn  SetHeight;
        private static AnalogIn  SetTilt;

        private OutputPort LFFill;
        private OutputPort RFFill;
        private OutputPort LRFill;
        private OutputPort RRFill;

        private OutputPort LFDump;
        private OutputPort RFDump;
        private OutputPort LRDump;
        private OutputPort RRDump;

        //only a single copy since it's private
        private Hardware() 
        {
            InitAnalog();
            InitDigital();
        }

        public static Hardware Instance
        {
            get
            {
                return instance;
            }
        }

        private void InitAnalog()
        {
            LFHeight = new AnalogIn(AnalogIn.Pin.Ain0);
            RFHeight = new AnalogIn(AnalogIn.Pin.Ain1);
            LRHeight = new AnalogIn(AnalogIn.Pin.Ain2);
            RRHeight = new AnalogIn(AnalogIn.Pin.Ain3);
            SetHeight = new AnalogIn(AnalogIn.Pin.Ain4);
            SetTilt = new AnalogIn(AnalogIn.Pin.Ain5);                   
        }

        private void InitDigital()
        {
            //IO pin mapping
            //
            //    fill   dump
            //LF    8    9
            //RF    6    7
            //LR    4    5
            //RR    2    3

            RFFill = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di6, true);
            RFDump = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di7, true);

            LFFill = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di8, true);
            LFDump = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di9, true);

            RRFill = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di2, true);
            RRDump = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di3, true);

            LRFill = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di4, true);
            LRDump = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di5, true);

        }


        //need to specify corner
        public Int16 GetHeight(Position pos)
        {
            Int16 val = 0;

            switch(pos)
            {
                case Position.LeftFront:
                    val = (Int16)LFHeight.Read();
                    break;
                case Position.RightFront:
                    val = (Int16)RFHeight.Read();
                    break;
                case Position.LeftRear:
                    val = (Int16)LRHeight.Read();
                    break;
                case Position.RightRear:
                    val = (Int16)RRHeight.Read();
                    break;
                case Position.SetHeight:
                    val = (Int16)SetHeight.Read();
                    break;
                case Position.SetTilt:
                    val = (Int16)SetTilt.Read();
                    break;
            }
            return val;
        }

        //open or close the fill valve specified
        //Note that open is false and close is true, so we invert the logic
        public void Fill(Position pos, Solenoid state)
        {
            //pin.write() needs a bool
            bool sol = !(state == Solenoid.Open);

            switch(pos)
            {
                case Position.LeftFront:
                    LFFill.Write(sol);
                    break;
                case Position.RightFront:
                    RFFill.Write(sol);
                    break;
                case Position.LeftRear:
                    LRFill.Write(sol);
                    break;
                case Position.RightRear:
                    RRFill.Write(sol);
                    break;
            }
        }


        public void Dump(Position pos, Solenoid state)
        {
            //pin.write() needs a bool
            bool sol = !(state == Solenoid.Open);

            switch (pos)
            {
                case Position.LeftFront:
                    LFDump.Write(sol);
                    break;
                case Position.RightFront:
                    RFDump.Write(sol);
                    break;
                case Position.LeftRear:
                    LRDump.Write(sol);
                    break;
                case Position.RightRear:
                    RRDump.Write(sol);
                    break;
            }
        }

    }
}

