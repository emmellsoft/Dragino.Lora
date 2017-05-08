# .NET Library for the Dragino LoRa Extension Boards
A Windows IoT Core library for the Dragino LoRa expansion boards

***NOTE: This is a work in progress.***

## Hardware

The library is developed and tested on the following expansion boards:

[Dragino LoRa/GPS HAT](http://www.dragino.com/products/module/item/106-lora-gps-hat.html "Dragino's product page")

[Dragino LoRa/GPS Shield](http://www.dragino.com/products/lora/item/108-lora-gps-shield.html "Dragino's product page")

[Dragino LoRa Shield](http://www.dragino.com/products/lora/item/102-lora-shield.html "Dragino's product page")

# Tutorials

[A LoRaWAN "The Things Network" Gateway for Windows IoT Core](https://www.hackster.io/laserbrain/a-lorawan-the-things-network-gateway-for-windows-iot-core-441210 "Project on hackster.io")

*More to come...*

# The .NET library

## Radio
The `Dragino.Radio.ITransceiver` interface represents the radio driver.
The transceiver code supports the Semtech's [SX1276](http://www.semtech.com/images/datasheet/sx1276_77_78_79.pdf "SX1276 Datasheet as PDF") radio chip (used by the Dragino boards).

Create an instance of the transceiver using the static `Dragino.Radio.TransceiverFactory` class. Its `Create`-method takes an instance of `TransceiverSettings` and `TransceiverPinSettings`.
The `TransceiverSettings` tells how the radio chip should operate and the `TransceiverPinSettings` tells which GPIO pins are used to hook up the expansion board. If the LoRa/GPS HAT is used there is a `TransceiverPinSettings.DraginoLoraGpsHat` preset that can be used, otherwise the pin numbers must be specified.

## GPS

*To be described here...*

# Demo Code

## LoRaWAN Gateway
There is a simple LoRaWAN gateway, implemented as a packet forwarder (which means that it will pick up radio packets and forward them to a server).
The `Dragino.Lora.Demo.Gateway.sln` shows how it is used.

## Point-to-point communication
If you have two LoRa expansion boards (and two Raspberry Pis) you can let them communicate with each other.
Run both `Dragino.Lora.Demo.Sender.sln` and `Dragino.Lora.Demo.Receiver.sln`.
Note that there really isn't any difference between the sender and the receiver; but in my demo the sender will transmitt a short message periodically, and the receiver will then send it back in reverse order.

