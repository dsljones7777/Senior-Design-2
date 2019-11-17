# RFIDCommandCenter
The system is designed to allow for tracking of tagged items. This system could be used in the Senior Project design goal of seeing who is inside or outside of the building and optionally allowing for entry allowance and denial. The system could be used for inventory tracking, inventory management, item finding, and various other applications due to the fac the device's program is mainly controlled by a server. 

There are 4 aspect to the system
-The command center will accept network connections for UI and device clients. UI clients communicate using RPC with object serialization. Device clients comminicate using RPC with network command packets. The command center controls DB access for UI clients. The command center also controls device operation and can modify the program flow as seen fit.

-The SQL database holds the information about tags, locations, users of the system, and access right per location

-A device controller allows for a device to run and comminicate with the server program. It is designed to connect to the server and receive commands while using an RFID reader to read tagged items

-A UI application to set up users, tags, locaitons, and various other functions important to the center
 
