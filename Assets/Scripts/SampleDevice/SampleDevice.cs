// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.Collections.Generic;
#if UNITY_WSA_10_0 && !UNITY_EDITOR
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityUWPBTLEPlugin;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

#endif

namespace TestApp.Sample
{
    /// <summary>
    ///  A possible sample device implementation.  This is used to interface between the UX and BTLE low level implementations.
    /// </summary>
    public class SampleDevice
    {
        /// <summary>
        /// UUID's used to interact with the device.
        /// These will be from the device manufacturer.  They are just place holders and MUST be replaced with actual device UUID's
        /// </summary>
        const string sService_UUID = "358407F4-BF93-408A-B128-57515EBAF150";
        const string sCommandCharacteristic = "7042D954-39BF-4E4F-A24B-0F43C0FA6B94";

#if UNITY_WSA_10_0 && !UNITY_EDITOR


        /// <summary>
        /// If the device supported sensors this is an example of how it might be accessed
        /// </summary>
        const string sSensorCharacteristic = "7CFF1AFE-A558-4B8F-81AC-ACF28A21FA89";

        BluetoothLEDeviceWrapper BTLEDevice;

        /// The number and type of characteristics is defined by the device manufacturer
        GattCharacteristicsWrapper CommandCharacteristic;
        GattCharacteristicsWrapper SensorCharacteristic;

        /// <summary>
        ///  The low level GATT services interface
        /// </summary>
        GattDeviceServiceWrapper GattDeviceService;

        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name
        {
            get { return BTLEDevice.Name; }
        }

        /// <summary>
        ///  Device property so we can look items up
        /// </summary>
        public DeviceInformation DeviceInfo
        {
            get { return BTLEDevice.DeviceInfo; }
        }

        public int ServiceCount { get; internal set; }

        /// <summary>
        /// Constructor, passing in the found BTLE device to interact with.
        /// </summary>
        /// <param name="device"></param>
        public SampleDevice(BluetoothLEDeviceWrapper device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            BTLEDevice = device;
        }

        /// <summary>
        /// Called to make a connection with the BTLE device. 
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            bool connected = false;
            ShowFeedback("Connect");

            // is it already connected?
            if (BTLEDevice.IsConnected)
            {
                ShowFeedback("BTLE device Connected");
                connected = true;
            }else
            {
                ShowFeedback("BTLE device wasn't connected, trying to connect");

                connected = BTLEDevice.Connect();
            }


            if (connected)
            {
                ShowFeedback("BTLE Device connected");
            }else
            {
                ShowFeedback("BTLE device not connected");
            }



            return connected;
        }

        /// <summary>
        /// Connect to the service and then list out the characteristics looking for our special characteristics by UUID
        /// </summary>
        /// <returns></returns>
        public bool ConnectService()
        {
            bool connectOk = false;
            ShowFeedback("Device service count: " + BTLEDevice.ServiceCount);
            ServiceCount = BTLEDevice.ServiceCount;
            connectOk = true;

            foreach (var service in BTLEDevice.Services)
            {
                ShowFeedback(service.Name);
                if (service.Name.Contains(sService_UUID))
                {
                    GattDeviceService = service;
                }
            }

            if (GattDeviceService != null)
            {
                ShowFeedback("Service found");
                ShowFeedback("Characteristics count: " + GattDeviceService.Characteristics.Count);

                int retry = 5;
                while (GattDeviceService.Characteristics.Count == 0 && retry > 0)
                {
                    Task.Delay(500);
                    retry--;
                }

                if (GattDeviceService.Characteristics.Count == 0)
                {
                    ShowFeedback("Characteristics count: " + GattDeviceService.Characteristics.Count);
                }

                ShowFeedback("Characteristics count: " + GattDeviceService.Characteristics.Count);
                foreach (var characteristic in GattDeviceService.Characteristics)
                {
                    ShowFeedback("Characteristic Name: " + characteristic.Name);
                    ShowFeedback("Characteristic UUID: " + characteristic.UUID);
                    string UUID = characteristic.UUID.ToUpper();

                    // See if this is the command characteristic used for device control
                    if (UUID.Contains(sCommandCharacteristic))
                    {
                        ShowFeedback("Command characteristic found");
                        CommandCharacteristic = characteristic;
                        ShowFeedback(CommandCharacteristic.Name);

                        continue;
                    }

                    // See if this is the sensor (optional depending on device) characteristic
                    if (UUID.Contains(sSensorCharacteristic))
                    {
                        ShowFeedback("Sensor characteristic found");
                        SensorCharacteristic = characteristic;
                        SensorCharacteristic.Characteristic.ValueChanged += SensorCharacteristic_ValueChanged;

                        // If you don't call SetNotify you won't get the ValueChanged notifications.  
                        bool notifyOk = SensorCharacteristic.SetNotify();

                        continue;
                    }
                }
            }

            return connectOk;
        }

        /// Again depending on the BTLE device manufacturer
        /// NOTE: Because unity uses the .c# 4.0 spec we don't have direct access to the async / Task functions
        /// but we will when running under the runtime as we will be in the WSA space
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        public async Task Send(Command whatToSend)
        {
            await whatToSend.Send(CommandCharacteristic);
        }
#endif

        /// <summary>
        /// If the device detects a change in a characteristic value it will call this (if SetNotify has been called).
        /// What will be considered a change is up to the device manufacturer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SensorCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            ShowFeedback("SensorCharacteristic_ValueChanged");

            // How to read characteristics values will be determined by the device manufacturer
            byte[] newValue = args.CharacteristicValue.ToArray();
        }

        /// A list of feedback messages to be displayed
        public List<string> Feedback { get; internal set; }

        /// <summary>
        /// Pass feedback to the UX for display.
        /// </summary>
        /// <param name="msg"></param>
        public void ShowFeedback(string msg)
        {
            if (Feedback != null)
            {
                Feedback.Add(msg);
            }
        }
#endif
    }
}
