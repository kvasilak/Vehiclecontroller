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
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace vehiclecontroller
{
    public class Program
    {
        public Program()
        {
        }

        public static void Main()
        {
            try
            {
                //each corner runs in it's own thread
                Corner LeftFront = new Corner(Position.LeftFront);
                Corner RightFront = new Corner(Position.RightFront);
                Corner LeftRear = new Corner(Position.LeftRear);
                Corner RightRear = new Corner(Position.RightRear);

                while (true)
                {
                    //nothing to do here
                    Thread.Sleep(Timeout.Infinite);
                }

            }
            catch(Exception e)
            {
                Debug.Print("Exception caught; " + e.Message);
            }
        }

    }
}
