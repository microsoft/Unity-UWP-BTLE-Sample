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
using System.Diagnostics;
#if UNITY_WSA_10_0 && !UNITY_EDITOR
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityUWPBTLEPlugin;
using Windows.Storage.Streams;
#endif

namespace TestApp.Sample
{
    /// <summary>
    ///  This is a sample of how a bluetooth LE device could be interfaced with.  The actual implementation will be dependent
    ///  upon the device manufacturer specifications.
    /// </summary>
    public class Command
    {
        /// <summary>
        ///  Command id that will be sent to the device
        /// </summary>
        public enum CommandIds
        {
            MoveLeft = 0x01,
            MoveRight = 0x02,
            GetBattery = 0xA,
            // etc
        }

        /// <summary>
        ///  A helper function for development use.
        /// </summary>
        /// <param name="msg"></param>
        public void ShowFeedback(string msg)
        {
            Debug.WriteLine(msg);
        }

        CommandIds commandId;
        
        /// Derived commands will have payload of from 0 to 19 bytes in length depending on command.
        private byte commandLength = 1;

        /// How a command is packaged and sent will be determined by the BTLE device manufacturer
        public Command(CommandIds c, byte length)
        {
            CommandId = c;
            CommandLength = length;
        }

        public CommandIds CommandId
        {
            get { return commandId; }
            set { commandId = value; }
        }

        public byte CommandLength
        {
            get { return commandLength; }
            set { commandLength = value; }
        }

        /// <summary>
        /// How a command is packaged and sent will be determined by the BTLE device manufacturer
        /// This is just a possible example.
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void PopulateBuffer(byte[] buffer)
        {
            buffer[0] = (byte)commandId;
        }

        /// <summary>
        /// Called to send a command to the BTLE device.  This is a possible example of how tat would be done but the specifics 
        /// will be based on device manufacturer specifications.
        /// </summary>
        /// <param name="commandCharacteristic"></param>
        /// <returns></returns>
        /// NOTE: Because unity uses the .c# 4.0 spec we don't have direct access to the async / Task functions
        /// but we will when running under the runtime as we will be in the WSA space
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        internal async Task Send(GattCharacteristicsWrapper commandCharacteristic)
        {
            byte[] message = new byte[CommandLength];
            PopulateBuffer(message);

            IBuffer buff = message.AsBuffer();

            await commandCharacteristic.Characteristic.WriteValueAsync(buff);
        }
#endif
    }
}
