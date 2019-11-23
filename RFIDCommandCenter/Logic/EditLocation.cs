﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class EditLocation
    {
        public void Execute(string currentLocationName, string newLocationName, string readerSerialIn, string readerSerialOut, DataContext context)
        {
            var locationToEdit = context.Locations.SingleOrDefault(l => l.LocationName == currentLocationName);

            if (locationToEdit == null)
                throw new Exception("There is no location in the system with that name");

            if (newLocationName != null)
            {
                if (context.Locations.Any(l => l.LocationName == newLocationName))
                    throw new Exception("A location with that name already exists");
                else
                    locationToEdit.LocationName = newLocationName;
            }
            
            if(readerSerialIn != null)
            {
                if (context.Locations.Any(l => l.ReaderSerialIn == readerSerialIn))
                    throw new Exception("A location with that reader serial in already exists");
                else
                    locationToEdit.ReaderSerialIn = readerSerialIn;
            }

            if(readerSerialOut != null)
            {
                locationToEdit.ReaderSerialOut = readerSerialOut;
            }

            context.SaveChanges();
        }
    }
}
