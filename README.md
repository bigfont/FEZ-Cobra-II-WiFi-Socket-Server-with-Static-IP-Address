# G120.SocketServer

A minimal socket server for the G120 Fez Cobra. 

# Output

```
NetworkInterface
GatewayAddress:192.168.1.1
IPAddress:192.168.1.115
IsDhcpEnabled:False
IsDynamicDnsEnabled:True
NetworkInterfaceType:71
PhysicalAddress:00:23:A7:1F:99:9C
SubnetMask:255.255.255.0

WiFiRS9110
GatewayAddress:192.168.1.1
IPAddress:192.168.1.115
IsDhcpEnabled:False
IsDynamicDnsEnabled:True
NetworkInterfaceType:71
PhysicalAddress:00:23:A7:1F:99:9C
SubnetMask:255.255.255.0

    #### Exception System.Net.Sockets.SocketException - CLR_E_FAIL (1) ####
    #### Message: 
    #### Microsoft.SPOT.Net.SocketNative::bind [IP: 0000] ####
    #### System.Net.Sockets.Socket::Bind [IP: 0016] ####
    #### G120.SocketServer.Program::ConnectSocket [IP: 0016] ####
    #### G120.SocketServer.Program::Main [IP: 004a] ####
    #### SocketException ErrorCode = 10022
    #### SocketException ErrorCode = 10022
A first chance exception of type 'System.Net.Sockets.SocketException' occurred in Microsoft.SPOT.Net.dll
    #### SocketException ErrorCode = 10022
An unhandled exception of type 'System.Net.Sockets.SocketException' occurred in Microsoft.SPOT.Net.dll
```

