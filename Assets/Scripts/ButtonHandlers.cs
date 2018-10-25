using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_WSA_10_0 && !UNITY_EDITOR
using UnityUWPBTLEPlugin;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using TestApp.Sample;
using System.Linq;
#endif

public class ButtonHandlers : MonoBehaviour
{
    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    Text feedbackText;
    ScrollRect scrollRect;
    List<string> feedbackMsgs;

#if UNITY_WSA_10_0 && !UNITY_EDITOR 
    // The BTLE helper class
    private BluetoothLEHelper ble;

    // The selected device we are connecting with.
    private SampleDevice theSelectedDevice;

    // The cached list of BTLE devices we have seen
    private List<SampleDevice> theCachedDevices = new List<SampleDevice>();

    // Use to optionally filter the incoming list of BTLE devices to only the ones your interested in.
    private String filter = "";
#endif

    // A unity defined script method.  Called when the script object is first created.
    public void Start()
    {
        feedbackMsgs = new List<string>();

        feedbackText = GameObject.Find("FeedbackText").GetComponent<Text>();
        scrollRect = GameObject.Find("Panel-ScrollRect").GetComponent<ScrollRect>();

        feedbackText.text = "Start of run.\n";

    }


#if UNITY_WSA_10_0 && !UNITY_EDITOR 
    /// A unity defined script method, called every tick to allow the script to perform some actions.
    public void Update()
    {
        // If we don't have a BTLE library connection yet we need to create one
        if (ble != null)
        {
            // If the devices changed flag has been set we need to process any additions / removals 
            if (ble.DevicesChanged)
            {
                // Process the list of new devices.
                var newDeviceList = ble.BluetoothLeDevicesAdded;
                foreach (var theNewDevice in newDeviceList)
                {
                    // First see if we already have it in the cache
                    var item = theCachedDevices.SingleOrDefault(r => r.DeviceInfo.Id == theNewDevice.DeviceInfo.Id);
                    if (item == null)
                    {
                        // new item

                        // Create the wrapper for the BTLE object
                        SampleDevice newSampleDevice = new SampleDevice(theNewDevice);

                        // Add it to our cache of devices
                        theCachedDevices.Add(newSampleDevice);

                        ShowFeedback("BTLE Device added: " + theNewDevice.Name);
                        string id = theNewDevice.DeviceInfo.Id;
                        if (filter.Length > 0)
                        {
                            // Filter defined so only take things that contain the filter name
                            if (theNewDevice.Name.Contains(filter))
                            {
                                ShowFeedback("Filtered BTLE Device found");
                            }
                        }
                        else
                        {
                            // No filter so just list everything found
                            ShowFeedback("BTLE Device found: " + theNewDevice.Name);
                        }

                    }
                    else
                    {
                        ShowFeedback("BTLE Duplicate device seen: " + theNewDevice.Name);
                    }
                }

                // For all the removed devices we want to remove them from the display list and cache
                var removedDeviceList = ble.BluetoothLeDevicesRemoved;
                foreach (var removed in removedDeviceList)
                {
                    var itemToRemove = theCachedDevices.SingleOrDefault(r => r.DeviceInfo.Id == removed.DeviceInfo.Id);
                    if (itemToRemove != null)
                    {
                        // Found it so remove it from the display list and the cache.
                        theCachedDevices.Remove(itemToRemove);

                        ShowFeedback("removed: " + removed.Name);
                    }
                }
            }
        }

        while(feedbackMsgs.Count > 0)
        {
            _ShowFeedback(feedbackMsgs[0]);
            feedbackMsgs.RemoveAt(0);
        }
    }
#endif 

    public void OnEnumerateClicked()
    {
        ShowFeedback("OnEnumerateClicked");
#if UNITY_WSA_10_0 && !UNITY_EDITOR 
        if (ble == null)
        {
            ble = BluetoothLEHelper.Instance;
        }
        ble.StartEnumeration();
#endif
    }

    public void OnConnectServicesClicked()
    {
        ShowFeedback("OnConnectServicesClicked");
#if UNITY_WSA_10_0 && !UNITY_EDITOR 

        if (theSelectedDevice != null)
            theSelectedDevice.ConnectService();
#endif
    }

    public void OOnDoSomethingClicked()
    {
        ShowFeedback("OOnDoSomethingClicked");
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        // Call a method on the device based on the manufacturer specifications.
        // if (theSelectedDevice != null)
        //    theSelectedDevice.DoSomething();
#endif
    }
    

    void _ShowFeedback(string msg)
    {
        System.Diagnostics.Debug.WriteLine(msg);
        feedbackText.text += msg + "\n";

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }


    public void ShowFeedback(string msg)
    {
        feedbackMsgs.Add(msg);
    }
}
