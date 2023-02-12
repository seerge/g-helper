[System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')       | out-null
[System.Reflection.Assembly]::LoadWithPartialName('presentationframework')      | out-null
[System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')             | out-null
[System.Reflection.Assembly]::LoadWithPartialName('WindowsFormsIntegration')    | out-null


function Set-ScreenRefreshRate
{ 
    param ( 
        [Parameter(Mandatory=$true)] 
        [int] $Frequency
    ) 

    $pinvokeCode = @"         
        using System; 
        using System.Runtime.InteropServices; 

        namespace Display 
        { 
            [StructLayout(LayoutKind.Sequential)] 
            public struct DEVMODE1 
            { 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] 
                public string dmDeviceName; 
                public short dmSpecVersion; 
                public short dmDriverVersion; 
                public short dmSize; 
                public short dmDriverExtra; 
                public int dmFields; 

                public short dmOrientation; 
                public short dmPaperSize; 
                public short dmPaperLength; 
                public short dmPaperWidth; 

                public short dmScale; 
                public short dmCopies; 
                public short dmDefaultSource; 
                public short dmPrintQuality; 
                public short dmColor; 
                public short dmDuplex; 
                public short dmYResolution; 
                public short dmTTOption; 
                public short dmCollate; 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] 
                public string dmFormName; 
                public short dmLogPixels; 
                public short dmBitsPerPel; 
                public int dmPelsWidth; 
                public int dmPelsHeight; 

                public int dmDisplayFlags; 
                public int dmDisplayFrequency; 

                public int dmICMMethod; 
                public int dmICMIntent; 
                public int dmMediaType; 
                public int dmDitherType; 
                public int dmReserved1; 
                public int dmReserved2; 

                public int dmPanningWidth; 
                public int dmPanningHeight; 
            }; 

            class User_32 
            { 
                [DllImport("user32.dll")] 
                public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode); 
                [DllImport("user32.dll")] 
                public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags); 

                public const int ENUM_CURRENT_SETTINGS = -1; 
                public const int CDS_UPDATEREGISTRY = 0x01; 
                public const int CDS_TEST = 0x02; 
                public const int DISP_CHANGE_SUCCESSFUL = 0; 
                public const int DISP_CHANGE_RESTART = 1; 
                public const int DISP_CHANGE_FAILED = -1; 
            } 

            public class PrimaryScreen  
            { 
                static public string ChangeRefreshRate(int frequency) 
                { 
                    DEVMODE1 dm = new DEVMODE1(); 

                    if (0 != User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm)) 
                    { 
                        dm.dmDisplayFrequency = frequency;

                        int iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_TEST); 

                        if (iRet == User_32.DISP_CHANGE_FAILED) 
                        { 
                            return "Unable to process your request. Sorry for this inconvenience."; 
                        } 
                        else 
                        { 
                            iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_UPDATEREGISTRY); 
                            switch (iRet) 
                            { 
                                case User_32.DISP_CHANGE_SUCCESSFUL: 
                                { 
                                    return "Success"; 
                                } 
                                case User_32.DISP_CHANGE_RESTART: 
                                { 
                                    return "You need to reboot for the change to happen.\n If you feel any problems after rebooting your machine\nThen try to change resolution in Safe Mode."; 
                                } 
                                default: 
                                { 
                                    return "Failed to change the resolution"; 
                                } 
                            } 
                        } 
                    } 
                    else 
                    { 
                        return "Failed to change the resolution."; 
                    } 
                } 


            } 
        } 
"@ # don't indend this line

    Add-Type $pinvokeCode
    [Display.PrimaryScreen]::ChangeRefreshRate($frequency) 
}


function isLaptopScreenMain {
    Add-Type -AssemblyName System.Windows.Forms
    $name = [System.Windows.Forms.Screen]::PrimaryScreen | Select-Object -ExpandProperty "DeviceName"
    if ($name -eq "\\.\DISPLAY1") {
        return $true
    }
}

function Get-ScreenRefreshRate
{
    $frequency = Get-WmiObject -Class "Win32_VideoController" | Select-Object -ExpandProperty "CurrentRefreshRate"
    return $frequency
}


# Some fancy icons in base64
$icon_eco = [Drawing.Icon][IO.MemoryStream][Convert]::FromBase64String("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Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAbUibMGI4mzBgAAAAAAAAAAAAAAAAAAAAAAAAAAAIayAwCMtgkAirQGe4q0Bv6KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4m0Bf+PtxH/xNqD//f67v////////////////////////////7+/f/+/vz////////////////////////////8/fn/2Oat/5zALP+JswT/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQGfYu0CwCLtQMAAAAAAAAAAAAAAAAAAAAAAIq0BQCKtAUbirQG0oq0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQF/4+3EP/K3Y7//P35//////////////////P35v/Z56//w9mA/7jRZ/+20GT/v9Z3/9Tjo//v9dz//v79/////////////////+Tuxf+ewTD/ibME/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG1Im0BR2KtAUAAAAAAAAAAAAAAAAAhbIDAIu0BgCKtAZdirQG+4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/i7QH/8DXef/8/fj////////////6/PT/0eGc/6HCNv+Ntg3/ibMD/4izAv+IswL/iLMC/4y1Cf+avif/wdd7//D13//////////////////h7L//lrwh/4mzBP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG+4mzBmCKtAYAi7IDAAAAAAAAAAAAibMFAIizBQWKtAamirQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswP/psZC//T45/////////////j67/+81HD/jbYM/4mzA/+KtAX/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswT/ibQE/6TEPf/k7sb/////////////////yt2P/4y1Cf+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BqiJswQGibMFAAAAAAAAAAAAirMFAIqzBR2KtAbYirQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bf+Otg7/1+aq/////////////f77/8LYfv+LtQj/irQF/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4izAv+fwTL/6/LU////////////9fnq/6TFPv+JswP/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BtqJtAYfirQGAAAAAAAAAAAAirQGAIq0BUOKtAb0irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4mzA/+lxT//9/ru////////////4Ou8/5K5GP+JtAT/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswT/ss5c//r88////////////8vdkP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BvSKtAZDirQGAAAAAAAAAAAAirQGAIq0BmiKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4mzBP/E2YH////////////8/fn/s85c/4izAv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQF/4q0Bf+KtAX/irQF/4q0Bf+JtAT/jrcP/9vos////////////+jwz/+Uuhv/ibQF/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAZlirQGAAAAAAAAAAAAirYHAIq0BoiKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQF/463D//c6bX////////////t89j/l7wi/4mzBP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bf+KtAX/jbYN/463Dv+Otw7/jrcO/463Dv+Otg7/jLYL/73Vc//+//7///////f67v+kxT3/ibMD/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAaHi7QHAAAAAAAAAAAAh7kJAIq0Bp2KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/ibQF/5S6G//p8dH////////////W5aj/jLUL/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQF/422Df+602z/2uix/9votP/b6LP/2+iz/9vos//b6LP/2+iy/+XvyP/+/vz///////3++v+xzVn/iLMC/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAabi7YJAAAAAAAAAAAAYb0vAIq0BqaKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/ibME/5e8Iv/x9uD////////////G24b/ibMD/4q0Bv+KtAb/irQG/4q0Bv+KtAb/ibMD/6TFPv/2+ev///////////////////////////////////////////////////////////+40Wf/iLMB/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAamiMACAAAAAAAAAAAAXMAyAIq0BqaKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/ibME/5e8Iv/x9uH////////////E2YL/iLMC/4q0Bv+KtAb/irQG/4q0Bv+KtAb/ibMD/6PEOv/1+er///////////////////////////////////////////////////////v89v+uy1L/iLMC/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAamiL0EAAAAAAAAAAAAiLYKAIq0BpuKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/ibQF/5S6G//p8dH////////////Q4Zv/i7QI/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/422DP+20GT/1uSn/9flqv/X5ar/1+Wq/9flqv/X5ar/1+Wq/9flqv/X5ar/1uWp/7zUcf+PtxL/irQF/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAadi7kHAAAAAAAAAAAAirYHAIq0BoeKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQF/462Dv/b6LL////////////l7sj/kbkW/4q0Bf+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswT/jLUK/422DP+Ntgv/jbYL/422C/+Ntgv/jbYL/422C/+Ntgz/jLUL/4m0Bf+KtAX/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAaIi7UHAAAAAAAAAAAAi7QGAIq0BmaKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4mzBP/C2H/////////////5+/L/q8lM/4izAv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bf+KtAX/irQF/4q0Bf+KtAX/irQF/4q0Bf+KtAX/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAZoirQGAAAAAAAAAAAAirQGAIq0BkOKtAb0irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4mzA/+jxDz/9vnr////////////2+iy/5C4E/+JtAX/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswT/iLMD/4mzBP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BvSKtAVDirQGAAAAAAAAAAAAirMFAIqzBR+KtAbairQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+Mtgv/0+Oh/////////////P36/8PZf/+LtQn/ibQF/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4mzBP+cwCz/tc9h/5/CNP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BtiJswUdibMFAAAAAAAAAAAAirMFAImzBAaKtAaoirQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswP/pMQ9//L34/////////////j78f/B2Hz/kLgS/4izA/+KtAX/irQG/4q0Bv+KtAb/irQG/4q0Bv+JswT/irQH/67LUv/r8tT////+/+vy1P+cvyz/ibME/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BqaKswUFirMFAAAAAAAAAAAAhbIDAIu0BgCKswZgirQG+4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/7zUcP/7/Pf////////////7/ff/2Oas/6fHQ/+PtxD/ibME/4izAv+IswL/iLMC/4u1CP+YvSP/xNmB//X56/////////////r89f+pyEj/iLMC/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG+4q0Bl2KtAYAjLIDAAAAAAAAAAAAAAAAAIq0BQCKswUdirQG1Iq0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQF/462Dv/H24f//P34//////////////////f67v/e6rn/xdqE/7jSaf+20GP/vdVz/9Hhnf/s89f//v78/////////////////9zptf+TuRj/ibQF/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG0om0BRuKtAUAAAAAAAAAAAAAAAAAAAAAAImxAwCMuwsAirMGfoq0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4m0Bf+Otw//wNd6//X46f////////////////////////////7+/f/9/vz////+///////////////////////9/vr/2eeu/5u/Kv+JtAT/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb+irQGe4e0CgCMswMAAAAAAAAAAAAAAAAAAAAAAAAAAACKswYAirMGJIq0BtWKtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAX/i7QH/6jHRv/e6rn/+/z2/////////////////////////////////////////////v78/+vy1P+91HL/kbkW/4mzBP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAbTirMGIoqzBgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACJsQMAjLgKAIqzBmeKtAb4irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4mzA/+RuBX/rMpP/9Dhm//o8M3/9Pjo//j68P/5+/H/9vns/+zz1v/X5ar/uNFn/5i9I/+JtAT/ibQF/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BveKtAZlirUMAIqzBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAirMGAIqyBQ6KtAanirQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAX/iLMD/4u1CP+Tuhn/nsEw/6bGQv+nx0T/ocM3/5a7H/+Ntgz/ibMD/4mzBP+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0Bv+KtAb/irQG/4q0BqWKswUMirMFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAhLAAAIu0BgCKswUnirQGzIq0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BgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD///////8AAP///////wAA//+AAf//AAD//AAAP/8AAP/4AAAP/wAA/+AAAAf/AAD/wAAAA/8AAP+AAAAB/wAA/wAAAAD/AAD+AAAAAH8AAPwAAAAAPwAA+AAAAAAfAAD4AAAAAB8AAPAAAAAADwAA8AAAAAAPAADgAAAAAAcAAOAAAAAABwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAOAAAAAABwAA4AAAAAAHAADwAAAAAA8AAPAAAAAADwAA+AAAAAAfAAD4AAAAAB8AAPwAAAAAPwAA/gAAAAB/AAD/AAAAAP8AAP+AAAAB/wAA/8AAAAP/AAD/4AAAB/8AAP/wAAAf/wAA//wAAD//AAD//4AB//8AAP///////wAA////////AAA=")
$icon_standard = [Drawing.Icon][IO.MemoryStream][Convert]::FromBase64String("AAABAAEAMDAAAAEAIACoJQAAFgAAACgAAAAwAAAAYAAAAAEAIAAAAAAAACQAABMLAAATCwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvrjoA7646AO+uOgDvrjoA7646AO+uOgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvojIA8ddUAO6tOQbvrjof7646Q++uOmbvrjqH7646m++uOqbvrjqm7646ne+uOojvrjpo7646Q++uOh3vrToG79hWAO+iMgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7604AO+sOAHvrTkd7646YO+uOqjvrjra76469e+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/76469O+uOtjvrjqm7646Xe+tOhvvqzcB76w5AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO+vNwDvticA7646I++uOn7vrjrV7646+u+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646+u+uOtLvrjp87606Ie+mNAHvqzgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADpqzoA8K45AO+uOQ/vrjpn76461e+uOv7vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr976460++uOmXvrToM7646AO+rOgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO+sOQDvsjsA7646J++uOqfvrjr47646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOvfvrjql7646J++qOwDvrzoAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7Ks4APq5PwDvrTo97646zO+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646zu+uOj/vvUEA76s4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADtrDkA87I8AO6tOkTvrjrZ7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOtrvrTpE77I8AO+sOQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO+sNwDurzwA7646P++uOtrvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjrZ7606Pe+vOwDvqjgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7K0/AO+uOQDvrjko7646ze+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/7604/++tOP/vrTj/7604/++uOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646zO+uOifvrjkA760/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7605AO+tOQzvrjql7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/7603/++vO//wtEj/8bpW//K/Yv/yv2T/8bpY//C1Sv/vrz3/7603/++uOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOqfwrjoO8K46AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvqzgA8LY/AO+uOmXvrjr37646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOP/ws0f/9Md3//nesP/87tX//fbq//758P/++fH//ffs//zw2v/54bf/9cyC//G2Tf/vrTj/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOvjvrjpn7607AO+uOQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvrTkA7605Iu+uOtPvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/7689//TGdf/76s3//vz4//////////////////////////////////////////////37//zu1//1z4n/8LNF/++tOP/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjrU8K46I/CuOgAAAAAAAAAAAAAAAAAAAAAAAAAAAO+qNwDvsTsA7646e++uOv7vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOf/wsUP/99ad//758v/////////////////////////////+/f///v3//////////////////////////////fr/+uS+//G6WP/vrTj/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646feeuOQDzrjoAAAAAAAAAAAAAAAAAAAAAAO+tOQDvrTob76460u+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7645//CxQv/42qb///36//////////////////336//65cD/99aa//XOh//1zYT/9tOU//nhtv/99OT///79//////////////////vs0f/yu1v/7604/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/76461O+uOR3wrjoAAAAAAAAAAAAAAAAA76k3AO+uOgDvrjpd7646+++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7647//bTlf///fr////////////++/b/+d+x//K+YP/vsD//7604/++tN//vrTf/7603/++vPf/xuVT/99SW//315v/////////////////76sz/8bdP/++tOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646+++uOmDvrjoA7683AAAAAAAAAAAA7q06AO6sOQXvrjqm7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrTj/88Fp//337P////////////768v/20I7/77A//++tOP/vrjr/7646/++uOv/vrjr/7646/++uOv/vrTj/7645//PAZf/77NL/////////////////+Nun/++vPf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOqjurjkG7q46AAAAAAAAAAAA7q46AO6uOh3vrjrY7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/wsUD/+uO8//////////////77//fVmf/vrzv/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tN//yvF3//PHd/////////////vjv//PAZv/vrTj/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOtrwrjof8K46AAAAAAAAAAAA7646AO+uOkPvrjr07646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOP/zwWf//vnx////////////++nK//C0SP/vrTn/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrTj/9cp+//779v////////////jbp//vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOvTvrjpD7646AAAAAAAAAAAA8K46AO+uOmjvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOP/31pz//////////////fv/9cp+/++tN//vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOf/vrjn/7645/++uOf/vrTn/8LFB//rmw/////////////zv2f/wtUv/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjpl7646AAAAAAAAAAAA8a86AO+uOojvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7645//CxQf/658X////////////88uD/8bdQ/++tOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/77BA//CxQf/wsUH/8LFB//CxQf/wsUH/77A+//bRkP////7///////758v/zwGb/7604/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjqH7646AAAAAAAAAAAA768+AO+uOp3vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7645//C1S//88Nv////////////547r/768+/++uOv/vrjr/7646/++uOv/vrjr/7646/++wP//2z4v/+ubB//rmw//65sP/+ubD//rmw//65sP/+ubC//zt1P///v3////////9+//0yXz/7603/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjqb7687AAAAAAAAAAAA76xFAO+uOqbvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7605//G3UP/99ef////////////32KD/7604/++uOv/vrjr/7646/++uOv/vrjr/7604//PAZv/++O/////////////////////////////////////////////////////////////1zof/76w2/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjqm76w4AAAAAAAAAAAA76xIAO+uOqbvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7605//G3UP/99ef////////////31pz/7603/++uOv/vrjr/7646/++uOv/vrjr/7604//K/Y//++O7///////////////////////////////////////////////////////78+P/0x3b/7603/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjqm76w4AAAAAAAAAAAA7687AO+uOpvvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7645//C1Sv/88Nr////////////53rD/7687/++uOv/vrjr/7646/++uOv/vrjr/7646/++wP//1zYX/+eK5//rjvP/647v/+uO7//rju//647v/+uO7//rju//647z/+eO7//bRj//wskP/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjqd7687AAAAAAAAAAAA8a86AO+uOofvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++wQP/65sL////////////77dP/8LNH/++uOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrTn/7689/++wPv/vsD7/77A+/++wPv/vsD7/77A+/++wPv/vsD7/77A+/++uOf/vrjn/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjqI8K46AAAAAAAAAAAA8K46AO+uOmbvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOP/31Zn////////////++/X/9MVy/++tN//vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjpo8K46AAAAAAAAAAAA7646AO+uOkPvrjr07646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOP/ywGX//vnv////////////+ubD//CyRP/vrjn/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrTj/7603/++tOP/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOvTvrjpD7646AAAAAAAAAAAA7q46AO6tOh/vrjra7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vsD7/+eC1//////////////37//fVmv/vrzz/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOf/xuln/9cuC//K9Xv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOtjurTod7q06AAAAAAAAAAAA7q06AO6sOwbvrjqo7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrTj/88Bl//326f////////////779P/31Jf/8LJD/++tN//vrjn/7646/++uOv/vrjr/7646/++uOv/vrTj/7647//THdv/88d3////+//zx3f/xulj/7604/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOqburjkF7q46AAAAAAAAAAAA76k9AO+uOgDvrjpg7646+++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646//bRjv/+/Pj//////////////fn/+uS9//PCa//wsUL/7604/++tN//vrTf/7603/++vPP/xt1H/99ab//747/////////////789//zw27/7603/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646+++uOl3vrjoA77A3AAAAAAAAAAAAAAAAAO6tOgDurTod76461O+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7645//CxQP/32KD///35//////////////////758v/76Mj/99ee//XOif/1zIT/9tKR//nfsf/88t////79//////////////////rnxf/wtEj/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/76460u+tOhvvrjkAAAAAAAAAAAAAAAAAAAAAAOurOAD3sj0A7646fu+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOf/wsUH/9tSV//747v/////////////////////////////+/f///vz////+/////////////////////////fv/+uW///G6V//vrTn/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr+7646e++tOwDvsDgAAAAAAAAAAAAAAAAAAAAAAAAAAADurjoA7q45JO+uOtXvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/7647//PDbf/66Mf//vz4//////////////////////////////////////////////79//zx3f/20Y//8LNG/++tOP/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjrT7606Iu+uOgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvrjgA7648AO+uOmfvrjr47646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++tOP/ws0b/9MZ0//nesP/879j//vjt//768//++vT//vnw//zy3//647z/9c6H//G4Uf/vrTn/7645/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOvfvrjpl765AAO+uOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7q46AO6uOg7vrjqn7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/7603/++vPP/wtEn/8rxc//PBaf/zwmv/8r5g//G2Tv/vsD7/7603/++tOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOqXvrjkM7645AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA77BAAO+uOQDvrjon7646zO+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjn/7604/++tN//vrTf/7604/++tOf/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646ze+uOifvrzoA76Q9AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOmqOADxsDsA7646Pe+uOtnvrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjra7646P+yuOAD0rjwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvrToA77A6AO+uOkTvrjra7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOtnvrjpE77A6AO+tOgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA76w6AO+1OADvrjo/7646ze+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646zO+uOj3vsjoA76w6AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO+xOwDvozUA7646J++uOqXvrjr37646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOvjvrjqn7646J++wPQDvrDkAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvsT0A7646AO+vOwzvrjpk76460++uOv3vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr+76461e+uOmfvrjoP7646AO+xNwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO+vPADwr0EB7q06Ie+uOnzvrjrS7646+u+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/7646+u+uOtXvrjp+7646I++0LADvrzgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7687AO+vPAHvrjsb7646Xe+uOqbvrjrY76469O+uOv/vrjr/7646/++uOv/vrjr/7646/++uOv/vrjr/76469O+uOtrvrjqo7646YO+tOx3vrD0B7608AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvskIA76AlAO+vOwXvrjod7646Q++uOmjvrjqI7646ne+uOqbvrjqm7646m++uOofvrjpm7646Q++uOh/urjsG8aIkAO+yQgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADvrjoA7646AO+uOgDvrjoA7646AO+uOgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD///////8AAP///////wAA//+AAf//AAD//AAAP/8AAP/4AAAP/wAA/+AAAAf/AAD/wAAAA/8AAP+AAAAB/wAA/wAAAAD/AAD+AAAAAH8AAPwAAAAAPwAA+AAAAAAfAAD4AAAAAB8AAPAAAAAADwAA8AAAAAAPAADgAAAAAAcAAOAAAAAABwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAMAAAAAAAwAAwAAAAAADAADAAAAAAAMAAOAAAAAABwAA4AAAAAAHAADwAAAAAA8AAPAAAAAADwAA+AAAAAAfAAD4AAAAAB8AAPwAAAAAPwAA/gAAAAB/AAD/AAAAAP8AAP+AAAAB/wAA/8AAAAP/AAD/4AAAB/8AAP/wAAAf/wAA//wAAD//AAD//4AB//8AAP///////wAA////////AAA=")
$icon_ultimate = [Drawing.Icon][IO.MemoryStream][Convert]::FromBase64String("AAABAAEAMDAAAAEAIACoJQAAFgAAACgAAAAwAAAAYAAAAAEAIAAAAAAAACQAABMLAAATCwAAAAAAAAAAAAAAAAAMAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAwAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgIP8AICD/ACAg/wYgIP8fICD/QyAg/2YgIP+HICD/myAg/6YgIP+mICD/nSAg/4ggIP9oICD/QyAg/x0gIP8GICD/ACAg/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAICD/ACAg/wEgIP8dICD/YCAg/6ggIP/aICD/9SAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/9CAg/9ggIP+mICD/XSAg/xsgIP8BICD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAg/wAgIP8AICD/IyAg/34gIP/VICD/+iAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/+iAg/9IgIP98ICD/ISAg/wEgIP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgIP8AICD/ACAg/w8gIP9nICD/1SAg//4gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/9ICD/0yAg/2UgIP8MICD/ACAg/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAg/wAgIP8AICD/JyAg/6cgIP/4ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//cgIP+lICD/JyAg/wAgIP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAICD/ACAg/wAgIP89ICD/zCAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/ziAg/z8gIP8AICD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAgIP8AICD/ACAg/0QgIP/ZICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/9ogIP9EICD/ACAg/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAACAg/wAgIP8AICD/PyAg/9ogIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/ZICD/PSAg/wAgIP8AAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAICD/ACAg/wAgIP8oICD/zSAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///Hh7//x0d//8dHf//Hh7//x8f//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/zCAg/ycgIP8AICD/AAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAICD/ACAg/wwgIP+lICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///HR3//yIi//8wMP//QED//05O//9PT///QkL//zMz//8jI///HR3//x8f//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/6cgIP8OICD/AAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAgIP8AICD/ACAg/2UgIP/3ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x0d//8vL///ZGT//6Wl///Q0P//5+f//+/v///v7///6en//9bW//+trf//cnL//zY2//8eHv//Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//ggIP9nICD/ACAg/wAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAgIP8AICD/IiAg/9MgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///JCT//2Ji///Gxv//9/f/////////////////////////////////////////////+/v//9LS//96ev//LS3//x4e//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/UICD/IyAg/wAAAAAAAAAAAAAAAAgAAAAIAAAAACAg/wAgIP8AICD/eyAg//4gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x8f//8qKv//kJD///Dw//////////////////////////////39///8/P/////////////////////////////5+f//trb//0JC//8eHv//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/fSAg/wAgIP8AAAAAAAAAAAgAAAAIAAAAACAg/wAgIP8bICD/0iAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///ykp//+amv//+fn//////////////////+np//+3t///jY3//3d3//90dP//hob//6ys///g4P///f3//////////////////8vL//9FRf//Hh7//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/1CAg/x0gIP8AAAAAAAAAAAgAAAAIAgIPACAg/wAgIP9dICD/+yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ISH//4eH///5+f/////////////19f//pqb//0tL//8mJv//HR3//xwc//8dHf//HR3//yMj//8+Pv//iYn//+Li///////////////////Gxv//ODj//x4e//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/+yAg/2AgIP8AAgIPAAAAAAgAAAAIFBSeACAg/wUgIP+mICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8dHf//Vlb//+rq//////////////Hx//9/f///Jib//x0d//8gIP//ICD//yAg//8gIP//ICD//yAg//8eHv//Hx///1FR///MzP//////////////////m5v//yMj//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/6ggIP8GFBShAAAAAAgAAAAIGxvaACAg/x0gIP/YICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x8f//8nJ///s7P/////////////+/v//4yM//8iIv//Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x0d//9ISP//2dn/////////////7Oz//1JS//8dHf//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/9ogIP8fHBzcAAAAAAgAAAAIHh7sACAg/0MgIP/0ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x0d//9TU///7+//////////////w8P//zAw//8fH///ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8eHv//bW3///X1/////////////5ub//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//QgIP9DHh7tAAAAAAgAAAAIHR3qACAg/2ggIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x4e//+Pj//////////////6+v//bW3//x0d//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///x8f//8fH///Hx///x8f//8fH///KCj//7u7/////////////9TU//8zM///Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP9lHh7rAAAAAAgAAAAIGxvYACAg/4ggIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///ygo//+9vf/////////////c3P//OTn//x4e//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///Jyf//ygo//8oKP//KCj//ygo//8oKP//JSX//4KC///+/v////////Dw//9SUv//HR3//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP+HGxvaAAAAAAgAAAAIFRWlACAg/50gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///zMz///W1v////////////+xsf//JCT//yAg//8gIP//ICD//yAg//8gIP//ICD//yYm//97e///ubn//7u7//+7u///u7v//7u7//+7u///urr//87O///8/P////////v7//9ra///HBz//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP+bFhatAAAAAAgAAAAIAgIRACAg/6YgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hh7//zk5///k5P////////////+Tk///Hh7//yAg//8gIP//ICD//yAg//8gIP//HR3//1JS///t7f////////////////////////////////////////////////////////////93d///HBz//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP+mAgIQAAAAAAgAAAAIAgIQACAg/6YgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hh7//zk5///k5P////////////+Pj///HR3//yAg//8gIP//ICD//yAg//8gIP//HR3//09P///s7P////////////////////////////////////////////////////////f3//9kZP//HR3//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP+mAgIRAAAAAAgAAAAIFhatACAg/5sgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///zMz///V1f////////////+lpf//ISH//yAg//8gIP//ICD//yAg//8gIP//ICD//yUl//90dP//sLD//7Oz//+zs///s7P//7Oz//+zs///s7P//7Oz//+zs///srL//4CA//8qKv//Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP+dFRWlAAAAAAgAAAAIGxvZACAg/4cgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///ycn//+6uv/////////////Ozv//Li7//x8f//8gIP//ICD//yAg//8gIP//ICD//yAg//8eHv//JCT//yUl//8lJf//JSX//yUl//8lJf//JSX//yUl//8lJf//JCT//x8f//8fH///ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP+IGxvZAAAAAAgAAAAIHh7rACAg/2YgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x4e//+MjP/////////////z8///X1///x0d//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP9oHR3qAAAAAAgAAAAIHh7tACAg/0MgIP/0ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x0d//9QUP//7e3/////////////urr//yws//8fH///ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8eHv//HR3//x4e//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//QgIP9DHh7sAAAAAAgAAAAIHBzcACAg/x8gIP/aICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8lJf//q6v/////////////+vr//4yM//8jI///Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x4e//9CQv//cXH//0lJ//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/9ggIP8dGxvaAAAAAAgAAAAIFBShACAg/wYgIP+oICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8dHf//UVH//+bm//////////////Pz//+Kiv//Kyv//x0d//8fH///ICD//yAg//8gIP//ICD//yAg//8eHv//ISH//2Rk///Z2f///v7//9nZ//9CQv//Hh7//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/6YgIP8FFBSeAAAAAAgAAAAIAgIPACAg/wAgIP9gICD/+yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//39////4+P/////////////4+P//tbX//1dX//8pKf//Hh7//xwc//8dHf//HR3//yIi//86Ov//jo7//+3t//////////////b2//9bW///HR3//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/+yAg/10gIP8AAgIPAAAAAAgAAAAIAAAAACAg/wAgIP8dICD/1CAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//Hx///ycn//+UlP//+Pj///////////////////Dw///Bwf//kZH//3l5//9zc///goL//6en///b2////Pz//////////////////729//8wMP//Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/0iAg/xsgIP8AAAAAAAAAAAgAAAAIAAAAACAg/wAgIP8AICD/fiAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x8f//8oKP//h4f//+vr//////////////////////////////39///8/P///v7////////////////////////7+///t7f//0BA//8fH///ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/+ICD/eyAg/wAgIP8AAAAAAAAAAAgAAAAIAAAAAAAAAAAgIP8AICD/JCAg/9UgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///ISH//1lZ///AwP//9/f//////////////////////////////////////////////Pz//9jY//+Bgf//Li7//x4e//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/TICD/IiAg/wAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAgIP8AICD/ACAg/2cgIP/4ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//x4e//8tLf//YWH//6Wl///T0///6+v///Ly///z8///7u7//9ra//+zs///d3f//zo6//8fH///Hx///yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//cgIP9lICD/ACAg/wAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAICD/ACAg/w4gIP+nICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///HR3//yIi//8xMf//Rkb//1ZW//9YWP//S0v//zY2//8lJf//HR3//x4e//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/6UgIP8MICD/AAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAICD/ACAg/wAgIP8nICD/zCAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8fH///Hh7//x0d//8dHf//Hh7//x8f//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/zSAg/ycgIP8AICD/AAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAACAg/wAgIP8AICD/PSAg/9kgIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/aICD/PyAg/wAgIP8AAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAgIP8AICD/ACAg/0QgIP/aICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg/9kgIP9EICD/ACAg/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAICD/ACAg/wAgIP8/ICD/zSAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/zCAg/z0gIP8AICD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAg/wAgIP8AICD/JyAg/6UgIP/3ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//ggIP+nICD/JyAg/wAgIP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgIP8AICD/ACAg/wwgIP9kICD/0yAg//0gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP/+ICD/1SAg/2cgIP8PICD/ACAg/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAg/wAgIP8BICD/ISAg/3wgIP/SICD/+iAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/+iAg/9UgIP9+ICD/IyAg/wAgIP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAICD/ACAg/wEgIP8bICD/XSAg/6YgIP/YICD/9CAg//8gIP//ICD//yAg//8gIP//ICD//yAg//8gIP//ICD/9CAg/9ogIP+oICD/YCAg/x0gIP8BICD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgIP8AICD/ACAg/wUgIP8dICD/QyAg/2ggIP+IICD/nSAg/6YgIP+mICD/myAg/4cgIP9mICD/QyAg/x8gIP8GICD/ACAg/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAMAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAgAAAAIAAAACAAAAAwAAAAAAAAAAH///////gAAf/+AAf/+AAB//AAAP/4AAH/4AAAP/gAAf+AAAAf+AAB/wAAAA/4AAH+AAAAB/gAAfwAAAAD+AAB+AAAAAH4AAHwAAAAAPgAAeAAAAAAeAAB4AAAAAB4AAHAAAAAADgAAcAAAAAAOAABgAAAAAAYAAGAAAAAABgAAQAAAAAACAABAAAAAAAIAAEAAAAAAAgAAQAAAAAACAABAAAAAAAIAAEAAAAAAAgAAQAAAAAACAABAAAAAAAIAAEAAAAAAAgAAQAAAAAACAABAAAAAAAIAAEAAAAAAAgAAQAAAAAACAABAAAAAAAIAAGAAAAAABgAAYAAAAAAGAABwAAAAAA4AAHAAAAAADgAAeAAAAAAeAAB4AAAAAB4AAHwAAAAAPgAAfgAAAAB+AAB/AAAAAP4AAH+AAAAB/gAAf8AAAAP+AAB/4AAAB/4AAH/wAAAf/gAAf/wAAD/+AAB//4AB//4AAH///////gAAAAAAAAAAAAA=")

# Init Config and Log
$ghelper_app_path = "$($env:LOCALAPPDATA)\GHelper"
$ghelper_config_path = "$($env:LOCALAPPDATA)\GHelper\config.json"
$ghelper_log_path = "$($env:LOCALAPPDATA)\GHelper\log.txt"

New-Item -ItemType Directory -Force -Path $ghelper_app_path

#Systray menu
$Main_Tool_Icon = New-Object System.Windows.Forms.NotifyIcon
$Main_Tool_Icon.Text = "G14 Helper"
$Main_Tool_Icon.Icon = $icon_standard
$Main_Tool_Icon.Visible = $true

$Menu_Perf_Title = New-Object System.Windows.Forms.MenuItem("Mode")
$Menu_Perf_Title.Enabled = $false
$Menu_Perf_Silent = New-Object System.Windows.Forms.MenuItem("Silent")
$Menu_Perf_Balanced = New-Object System.Windows.Forms.MenuItem("Balanced")
$Menu_Perf_Turbo = New-Object System.Windows.Forms.MenuItem("Turbo")

$Menu_Eco = New-Object System.Windows.Forms.MenuItem("Eco")
$Menu_Standard = New-Object System.Windows.Forms.MenuItem("Standard")
$Menu_Ultimate = New-Object System.Windows.Forms.MenuItem("Ultimate")

$Menu_Title = New-Object System.Windows.Forms.MenuItem("GPU Mode")
$Menu_Title.Enabled = $false
$Menu_Eco = New-Object System.Windows.Forms.MenuItem("Eco")
$Menu_Standard = New-Object System.Windows.Forms.MenuItem("Standard")
$Menu_Ultimate = New-Object System.Windows.Forms.MenuItem("Ultimate")

$Menu_RR = New-Object System.Windows.Forms.MenuItem
$Menu_RR.Enabled = $false
$Menu_RR60 = New-Object System.Windows.Forms.MenuItem("60Hz")
$Menu_RR120 = New-Object System.Windows.Forms.MenuItem("120Hz")
$Menu_OD = New-Object System.Windows.Forms.MenuItem("Panel Overdrive")

$Menu_Exit = New-Object System.Windows.Forms.MenuItem("Exit")

$contextmenu = New-Object System.Windows.Forms.ContextMenu
$Main_Tool_Icon.ContextMenu = $contextmenu

$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Perf_Title)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Perf_Silent)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Perf_Balanced)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Perf_Turbo)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange("-")

$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Title)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Eco)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Standard)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Ultimate)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange("-")

$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_RR)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_RR60)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_RR120)
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_OD)

$Main_Tool_Icon.contextMenu.MenuItems.AddRange("-")
$Main_Tool_Icon.contextMenu.MenuItems.AddRange($Menu_Exit)

# Hardware Initialisation

$device_performance = 0x00120075;
$device_eco = 0x00090020;
$device_mux = 0x00090016;
$device_overdrive = 0x00050019;

$asushw = Get-CimInstance -Namespace root/wmi -ClassName AsusAtkWmi_WMNB

function CheckScreen {

    $laptopScreen = isLaptopScreenMain;
    if (-Not $laptopScreen) {
        $Menu_RR.Text = "External Screen is main";
        $Menu_RR60.Visible = $false;
        $Menu_RR120.Visible = $false;
        $Menu_OD.Visible = $false;
        return;
    } else {
        $Menu_RR.Text = "Screen Refresh"
        $Menu_RR60.Visible = $true;
        $Menu_RR120.Visible = $true;
        $Menu_OD.Visible = $true;
    }

    $refresh_rate = Get-ScreenRefreshRate
    if ($refresh_rate -eq 60) {
        $Menu_RR.Text = "Laptop Screen Refresh: "+$refresh_rate+"Hz" 
        $Menu_RR60.Checked = $true;
        $Menu_RR120.Checked = $false;
    } elseif ($refresh_rate -eq 120) {
        $Menu_RR.Text = "Laptop Screen Refresh: "+$refresh_rate+"Hz" 
        $Menu_RR60.Checked = $false;
        $Menu_RR120.Checked = $true;
    }

    $screen_overdrive = (Invoke-CimMethod $asushw -MethodName DSTS -Arguments @{Device_ID=$device_overdrive} | Select-Object -ExpandProperty device_status) - 65536;
    if ($screen_overdrive -eq 1) {
        $Menu_OD.Checked = $true;
        $Menu_OD.Enabled = $true;
    } elseif ($screen_overdrive -eq 0) {
        $Menu_OD.Checked = $false;
        $Menu_OD.Enabled = $true;
    } else {
        $Menu_OD.Enabled = $false;
    }

}

function GetCPUTemperature {
    $t = Get-WmiObject MSAcpi_ThermalZoneTemperature -Namespace "root/wmi"
    $returntemp = ""

    foreach ($temp in $t.CurrentTemperature)
    {
        $currentTempCelsius = ($temp / 10)
        $returntemp += $currentTempCelsius.ToString() + "C "
    }
    return $returntemp
}


function UICheckStats {
    $cpu_fan = [math]::Round(((Invoke-CimMethod $asushw -MethodName DSTS -Arguments @{Device_ID=0x00110013} | Select-Object -ExpandProperty device_status) - 65536)/0.6);
    $gpu_fan = [math]::Round(((Invoke-CimMethod $asushw -MethodName DSTS -Arguments @{Device_ID=0x00110014} | Select-Object -ExpandProperty device_status) - 65536)/0.6);
    $Menu_Perf_Title.Text = $script:title_performance+"  |  CPU Fan: "+$cpu_fan.ToString()+"%"
    $Menu_Title.Text = $script:title_gpu+"  |  GPU Fan: "+$gpu_fan.ToString()+"%"
}

function Get-TimeStamp {
    return "[{0:MM/dd/yy} {0:HH:mm:ss}]" -f (Get-Date)
}

function WriteLog ([string]$event_name) {
    Write-Output "$(Get-TimeStamp) $event_name" | Out-file $ghelper_log_path -append
}

function SaveConfigSetting ([string]$Name, $Value){
    
    $global:ghelper_config[$Name] = $Value

    $config = $global:ghelper_config
    $config | ConvertTo-Json | Out-File $ghelper_config_path
}

function GetConfigSetting ([string]$Name) {
    return $global:ghelper_config[$Name]
}

function LoadConfig {

    $global:ghelper_config =  @{}

    $configJson = (Get-Content -Raw $ghelper_config_path -ErrorAction SilentlyContinue -ErrorVariable ConfigError) | ConvertFrom-Json
    $configJson.psobject.properties | Foreach { $global:ghelper_config[$_.Name] = $_.Value }

    if ($ConfigError) {
        $global:ghelper_config =  @{
            "performance_mode" = 0
            "panel_overdrive" = 1
        }

        $config = $global:ghelper_config
        $config | ConvertTo-Json | Out-File $ghelper_config_path
    }

}

function SetPeformanceMode ($performance_mode = 0) {

    $Menu_Perf_Silent.Checked = $false;
    $Menu_Perf_Balanced.Checked = $false;
    $Menu_Perf_Turbo.Checked = $false;

    switch ($performance_mode)
    {
        1 {
            $script:title_performance = "Mode: Turbo"
            $Menu_Perf_Turbo.Checked = $true
        }
        2 {
            $script:title_performance = "Mode: Silent"
            $Menu_Perf_Silent.Checked = $true
        }
        Default  {
            $script:title_performance = "Mode: Balanced"
            $Menu_Perf_Balanced.Checked = $true
            $performance_mode = 0
        }

    }

    SaveConfigSetting -Name 'performance_mode' -Value $performance_mode
    UICheckStats

    Invoke-CimMethod $asushw -MethodName DEVS -Arguments @{Device_ID=$device_performance ; Control_status=$performance_mode }
    WriteLog("Performance set to "+$performance_mode)
}

function SetPanelOverdrive ($overdrive = 1) {
    SaveConfigSetting -Name 'panel_overdrive' -Value $overdrive
    Invoke-CimMethod $asushw -MethodName DEVS -Arguments @{Device_ID=$device_overdrive ; Control_status=$overdrive }
    WriteLog("Panel Overdrive set to "+$overdrive)
}


function GetGPUMode {

    $mux_mode = (Invoke-CimMethod $asushw -MethodName DSTS -Arguments @{Device_ID=$device_mux} | Select-Object -ExpandProperty device_status) - 65536;
    $eco_mode = (Invoke-CimMethod $asushw -MethodName DSTS -Arguments @{Device_ID=$device_eco} | Select-Object -ExpandProperty device_status) - 65536;
    $script:gpu_mode = "standard"
    
    if ($mux_mode -eq 0) {
        $script:gpu_mode = "ultimate"
    } else {
    
        if ($eco_mode -eq 1) {
            $script:gpu_mode = "eco"
        } else {
            $script:gpu_mode = "standard"
        }
    
        if (-Not $mux_mode -eq 1) {
            # No MUX Switch
            $Menu_Ultimate.Enabled  = $false
        }
    }

    UIGPUMode($script:gpu_mode)
    WriteLog("GPU mode detected : $script:gpu_mode")
}


function SetGPUMode ($gpu_mode = "standard") {

    if ($gpu_mode -eq $script:gpu_mode) {return}

    $restart = $false;
    $changed = $false;

    if ($script:gpu_mode -eq "ultimate") {
        $msgBox =  [System.Windows.MessageBox]::Show('Switching off Ultimate Mode requires restart','Reboot now?','OKCancel')
        if  ($msgBox -eq 'OK') {
            Invoke-CimMethod $asushw -MethodName DEVS -Arguments @{Device_ID=$device_mux ; Control_status=1 }
            $restart = $true;
            $changed = $true;                
        }
    } elseif ($gpu_mode -eq "ultimate") {
        $msgBox =  [System.Windows.MessageBox]::Show('Ultimate mode requires restart','Reboot now?','OKCancel')
        if  ($msgBox -eq 'OK') {
            Invoke-CimMethod $asushw -MethodName DEVS -Arguments @{Device_ID=$device_mux ; Control_status=0 }
            $restart = $true;
            $changed = $true;                 
        }
    } elseif ($gpu_mode -eq "eco") { 
        UIGPUMode($gpu_mode);
        Invoke-CimMethod $asushw -MethodName DEVS -Arguments @{Device_ID=$device_eco ; Control_status=1 }
        $changed = $true; 
    } elseif ($gpu_mode -eq "standard") { 
        UIGPUMode($gpu_mode);
        Invoke-CimMethod $asushw -MethodName DEVS -Arguments @{Device_ID=$device_eco ; Control_status=0 }
        $changed = $true; 
    }

    if ($changed) {
        $script:gpu_mode = $gpu_mode;
        SaveConfigSetting -Name 'gpu_mode' -Value $gpu_mode
        WriteLog("GPU set to "+$gpu_mode)
    }


    if ($restart) {
        UIGPUMode($gpu_mode);
        WriteLog("Restarting")
        Restart-Computer
    }

}

function UIGPUMode ($gpu_mode) {

    $Menu_Eco.Checked = $false;
    $Menu_Standard.Checked = $false;
    $Menu_Ultimate.Checked = $false;

    switch ($gpu_mode)
    {
        "eco" {
            $script:title_gpu = "GPU: iGPU only"
            $Menu_Eco.Checked = $true
            $Main_Tool_Icon.Icon = $icon_eco
        }
        "ultimate" {
            $script:title_gpu = "GPU: dGPU exclusive"
            $Menu_Ultimate.Checked = $true
            $Main_Tool_Icon.Icon = $icon_ultimate
        }
        Default  {
            $script:title_gpu = "GPU: Balanced"
            $Menu_Standard.Checked = $true
            $gpu_mode = "standard"
            $Main_Tool_Icon.Icon = $icon_standard
        }
    }

    UICheckStats

}

LoadConfig

SetPeformanceMode(GetConfigSetting('performance_mode'))
SetPanelOverdrive(GetConfigSetting('panel_overdrive'))

GetGPUMode
CheckScreen


$timer = New-Object System.Windows.Forms.Timer
$timer.Add_Tick({UICheckStats})
$timer.Interval = 3000
$timer.Enabled = $True

# ---------------------------------------------------------------------
# Action when after a click on the systray icon
# ---------------------------------------------------------------------
$Main_Tool_Icon.Add_Click({                    
    CheckScreen
    UICheckStats
    If ($_.Button -eq [Windows.Forms.MouseButtons]::Left) {
        $Main_Tool_Icon.GetType().GetMethod("ShowContextMenu",[System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::NonPublic).Invoke($Main_Tool_Icon,$null)
    }
})


$Menu_Perf_Silent.add_Click({
    SetPeformanceMode(2)
})

$Menu_Perf_Balanced.add_Click({
    SetPeformanceMode(0)
})

$Menu_Perf_Turbo.add_Click({
    SetPeformanceMode(1)
})

$Menu_Eco.add_Click({
    SetGPUMode("eco")
})

$Menu_Standard.add_Click({
    SetGPUMode("standard")
})

$Menu_Ultimate.add_Click({
    SetGPUMode("ultimate")
})


$Menu_RR60.add_Click({
    Set-ScreenRefreshRate -Frequency 60
    CheckScreen
})

$Menu_RR120.add_Click({
    Set-ScreenRefreshRate -Frequency 120
    CheckScreen
})

$Menu_OD.add_Click({
    if ($Menu_OD.Checked) {
        SetPanelOverdrive(0)
    } else {
        SetPanelOverdrive(1)
    }
    CheckScreen
})


# When Exit is clicked, close everything and kill the PowerShell process
$Menu_Exit.add_Click({
    $Main_Tool_Icon.Visible = $false
    Stop-Process $pid
 })
 
# Force garbage collection just to start slightly lower RAM usage.
[System.GC]::Collect()

# Create an application context for it to all run within.
# This helps with responsiveness, especially when clicking Exit.
$appContext = New-Object System.Windows.Forms.ApplicationContext
[void][System.Windows.Forms.Application]::Run($appContext)