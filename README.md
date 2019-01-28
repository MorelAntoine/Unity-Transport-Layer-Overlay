# Unity-Transport-Layer-Overlay

Abstract overlay to facilitate the implementation of a communication protocol

**! Important !**

    Only message that fit in a buffer of 1024 bytes works. This project is made for a specific need and with
    the deprecated sign of the Transport Layer API of Unity I've no reason to go further ; if you want to go deeper, feel
    free to fork the project.

## Component

ANetworkClient

    Base class to inherit to create a network client overlay
    
ANetworkServer

    Base class to inherit to create a network server overlay

NetworkEventDispatcher

    Mandatory component, must be present in your scene!
    Help to bind NetworkEvent to the corret host

## How to use it?

### Setup

- Implement your custom client and server overlay with ANetworkClient and ANetworkServer.

- Add the @NetworkEventDispatcher prefab to your scene

- Add a NetworkServer in your desired project scene

- Add a NetworkClient in your desired project scene

### Launch

- For the NetworkServer, call the StartServer method

- For the NetworkClient, call the StartClient method then AttemptToConnectToServer method

Now you can send message between your client and your server. 

## Project Information

Developed and tested under **Unity 2018.3.0f2**

Made by **Antoine Morel**

Version **1.0.0**

## License

License MIT