using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GK6X
{
    public enum OpCodes
    {
        /// <summary>
        /// Information about the keyboard
        /// </summary>
        Info = 0x01,
        /// <summary>
        /// Restarts the keyboard (can reboot in special modes such as "CDBoot")
        /// </summary>
        RestartKeyboard = 0x03,
        Unk04 = 0x04,// Some diagnostics stuff? Or maybe something related to key input? Or macros ("KeyPress")?
        /// <summary>
        /// Set the active layer (base / driver / 1 / 2 / 3)
        /// </summary>
        SetLayer = 0x0B,
        /// <summary>
        /// Ping / keep alive
        /// </summary>
        Ping = 0x0C,

        /// <summary>
        /// Using macros when in the "driver" layer
        /// </summary>
        DriverMacro = 0x15,
        /// <summary>
        /// Set "driver" layer key values
        /// </summary>
        DriverLayerSetKeyValues = 0x16,
        /// <summary>
        /// Set "driver" layer config values (seem to be hard coded in the application)
        /// </summary>
        DriverLayerSetConfig = 0x17,
        /// <summary>
        /// The keyboard sends this packet to enable macros/shortcut/keypress lighting in the "driver" layer (the PC doesn't have to send request)
        /// </summary>
        DriverKeyCallback = 0x18,
        /// <summary>
        /// Updates the lighting in real time when in the "driver" layer
        /// </summary>
        DriverLayerUpdateRealtimeLighting = 0x1A,

        /// <summary>
        /// Resets a type of data (keys, lights, etc) for a layer
        /// </summary>
        LayerResetDataType = 0x21,
        LayerSetKeyValues = 0x22,
        Unk23_KbData = 0x23,// Likely a keyboard data set (see KeyboardLayerDataType)
        Unk24_KbData_Lighting = 0x24,// Some lighting related data (see KeyboardLayerDataType)
        LayerSetMacros = 0x25,
        /// <summary>
        /// Sets the lighting effects which should play when pressing keys ("Press Light")
        /// </summary>
        LayerSetKeyPressLightingEffect = 0x26,
        LayerSetLightValues = 0x27,
        /// <summary>
        /// Function key values
        /// </summary>
        LayerFnSetKeyValues = 0x31,
    }

    public enum OpCodes_SetDriverLayerKeyValues
    {
        KeySet = 1,
        KeySetFn = 2,
        /// <summary>
        /// Lighting effects which should play when pressing keys
        /// </summary>
        KeyPressLightingEffect = 3
    }

    public enum OpCodes_DriverLayerUpdateRealtimeLighting
    {
        Update = 1,
        UpdateComplete = 2
    }

    public enum OpCodes_DriverMacro
    {
        MouseState = 1,
        KeyboardState = 2,
        BeginEnd = 3,// TODO: Think of a better name (my mind is blank!)
    }

    public enum OpCodes_Info
    {
        /// <summary>
        /// The firmware id / version
        /// </summary>
        FirmwareId = 0x01,
        /// <summary>
        /// Unknown value (-1). This comes with crc validation, and always fails due to the value being -1.
        /// This is likely always -1 for the lifetime of the board (or until firmware change) based on the disassembly.
        /// </summary>
        Unk_02 = 0x02,
        /// <summary>
        /// The model id of the keyboard
        /// </summary>
        ModelId = 0x08,
        /// <summary>
        /// Some kind of buffer size info related to holding keyboard data
        /// </summary>
        InitBuffers = 0x09,
    }

    public enum KeyboardLayer
    {
        Invalid,
        Base = 1,
        Layer1 = 2,
        Layer2 = 3,
        Layer3 = 4,
        /// <summary>
        /// A better name for this would be "App"/"Software", as this mode only works when connected to the software.
        /// </summary>
        Driver = 5
    }

    /// <summary>
    /// Types of configurable data that can be sent to the keyboard (keys, lights, macro, etc)<para/>
    /// This is used by "21 XX"
    /// </summary>
    public enum KeyboardLayerDataType
    {
        Invalid,
        KeySet = 1,// Maps to 22 XX
        // Where/what is type 2? It could possibly be "23 XX", there seems to be a handler for this on the keyboard
        /// <summary>
        /// "Lighting effect"? Some sort of lighting related data, but not any of the lighting options I've seen so far
        /// (always 0x210 / 528 bytes of data - which is the same as the amount of data "driver" layer realtime lighting sends)
        /// </summary>
        LEData = 3,// Maps to 24 XX
        Macros = 4,// Maps to 25 XX
        /// <summary>
        /// Lighting effects which should play when pressing keys
        /// </summary>
        KeyPressLightingEffect = 5,//Maps to 26 XX
        /// <summary>
        /// Lighting data
        /// </summary>
        Lighting = 6,// Maps to 27 XX
        FnKeySet = 7// Maps to 31 XX
    }
}
