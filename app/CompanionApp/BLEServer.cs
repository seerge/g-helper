/*
 * at GHelper.csproj:

    <PropertyGroup>
     ..
    <TargetFramework>net8.0-windows10.0.17763.0</TargetFramework>
    ...
    </PropertyGroup>
    <ItemGroup>
    ...
    <PackageReference Include="DSoft.System.BluetoothLe" Version="2.0.2110.291" />
    ...
    </ItemGroup>

*/



using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;

namespace GHelper.CompanionApp
{
    class BLEServer : CompanionService
    {

        private readonly Guid SERVICE_UID = Guid.Parse("5A59D14B-D6B1-4CDE-B336-4EBC87960C4F");
        private readonly Guid CHARACTERISTIC_INFO_UID = Guid.Parse("68D4DC98-E0EA-48B1-838C-4BD6E7A3067A");
        private readonly Guid CHARACTERISTIC_MODES_UID = Guid.Parse("8F9D3A3A-486D-4B5F-A7A7-1304942B75FB");
        private readonly Guid CHARACTERISTIC_SENSOR_UID = Guid.Parse("ADFB2D54-EEC2-4B9B-A77B-BA53ECBF0A87");
        private readonly Guid CHARACTERISTIC_CMD_UID = Guid.Parse("84F4E7DA-4695-4DAA-8485-E4A5CCC91ABB");


        private GattServiceProvider serviceProvider;



        public BLEServer() : base()
        { 
        }

        public override async void Start()
        {
            
            var adapter = await BluetoothAdapter.GetDefaultAsync();
            if (adapter == null)
            {
                SetStatusChanged(EStatus.Stopped);
                return;
            }
            Debug.WriteLine($"BLE: Adapter: {adapter.IsLowEnergySupported} {adapter.DeviceId}");


            if (adapter.IsLowEnergySupported)
            {

                // Create the GATT service provider
                GattServiceProviderResult Gatt = await GattServiceProvider.CreateAsync(SERVICE_UID);

                if (Gatt.Error != BluetoothError.Success)
                {
                    Debug.WriteLine($"BLE: Gatt Error {Gatt.Error}");
                    return;

                }

                serviceProvider = Gatt.ServiceProvider;


                GattLocalCharacteristicResult characteristicInfoResult = await serviceProvider.Service.CreateCharacteristicAsync(
                     CHARACTERISTIC_INFO_UID,
                     new GattLocalCharacteristicParameters
                     {
                         CharacteristicProperties = GattCharacteristicProperties.Read,
                         UserDescription = "G-Helper Info characteristic",
                         ReadProtectionLevel = GattProtectionLevel.Plain

                     });
                if (characteristicInfoResult.Error != BluetoothError.Success)
                {
                    Debug.WriteLine($"BLE: Gatt Characteristic Info Error {characteristicInfoResult.Error}");
                    return;
                }

               
                GattLocalCharacteristicResult characteristicModesResult = await serviceProvider.Service.CreateCharacteristicAsync(
                     CHARACTERISTIC_MODES_UID,
                     new GattLocalCharacteristicParameters
                     {
                         CharacteristicProperties = GattCharacteristicProperties.Read,
                         UserDescription = "G-Helper Modes characteristic",
                         ReadProtectionLevel = GattProtectionLevel.Plain

                     });
                if (characteristicModesResult.Error != BluetoothError.Success)
                {
                    Debug.WriteLine($"BLE: Gatt Characteristic Modes Error {characteristicModesResult.Error}");
                    return;
                }


                GattLocalCharacteristicResult characteristicSensorResult = await serviceProvider.Service.CreateCharacteristicAsync(
                    CHARACTERISTIC_SENSOR_UID,
                    new GattLocalCharacteristicParameters
                    {
                        // CharacteristicProperties = GattCharacteristicProperties.Notify,
                        CharacteristicProperties = GattCharacteristicProperties.Read,
                        UserDescription = "G-Helper Sensor characteristic",
                        ReadProtectionLevel = GattProtectionLevel.Plain

                    });

                if (characteristicSensorResult.Error != BluetoothError.Success)
                {
                    Debug.WriteLine($"BLE: Gatt Sensor Characteristic Error {characteristicSensorResult.Error}");
                    return;
                }

                GattLocalCharacteristicResult cmdCharacteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(
                  CHARACTERISTIC_CMD_UID,
                  new GattLocalCharacteristicParameters
                  {
                      CharacteristicProperties = GattCharacteristicProperties.Write,
                      UserDescription = "G-Helper CMD characteristic",
                      WriteProtectionLevel = GattProtectionLevel.Plain,

                  });

                if (cmdCharacteristicResult.Error != BluetoothError.Success)
                {
                    Debug.WriteLine($"BLE: Gatt CMD Characteristic Error {cmdCharacteristicResult.Error}");
                    return;
                }


                // Subscribe to read and write requests

                characteristicInfoResult.Characteristic.ReadRequested += InfoCharacteristic_ReadRequested;

                characteristicModesResult.Characteristic.ReadRequested += ModesCharacteristic_ReadRequested;

                characteristicSensorResult.Characteristic.ReadRequested += SensorsCharacteristic_ReadRequested;


                //sensorCharacteristic = characteristicSensorResult.Characteristic;
                //sensorCharacteristic.SubscribedClientsChanged += Characteristic_SubscribedClientsChanged;

                cmdCharacteristicResult.Characteristic.WriteRequested += CmdCharacteristic_WriteRequested;



                serviceProvider.AdvertisementStatusChanged += ServiceProvider_AdvertisementStatusChanged;
                // Start advertising the service
                serviceProvider.StartAdvertising(new GattServiceProviderAdvertisingParameters
                {
                    IsConnectable = true,
                    IsDiscoverable = true,

                });

                Debug.WriteLine("BLE: GATT Server started and advertising.");

                //StartUpdate();

                SetStatusChanged(EStatus.Started);

            }
            else
            {
                SetStatusChanged(EStatus.Stopped);

                Debug.WriteLine("BLE: LowEnergy not available on this device");
            }
        }
       


        private void ServiceProvider_AdvertisementStatusChanged(GattServiceProvider sender, GattServiceProviderAdvertisementStatusChangedEventArgs args)
        {
            Debug.WriteLine($"BLE: Advertisement: {args.Status}");
        }

        private async void CmdCharacteristic_WriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var request = await args.GetRequestAsync();

            // Process write requests
            using (var reader = DataReader.FromBuffer(request.Value))
            {
                byte[] data = new byte[reader.UnconsumedBufferLength];

                reader.ReadBytes(data);

                OnRead(data);

            }
            request.Respond();
            deferral.Complete();
        }
       

        private async void InfoCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var request = await args.GetRequestAsync();

            IBuffer buffer = PrepareInfoBuffer().AsBuffer();

            request.RespondWithValue(buffer);

            deferral.Complete();
        }

        private async void ModesCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var request = await args.GetRequestAsync();

            IBuffer buffer = PrepareModesBuffer().AsBuffer();

            request.RespondWithValue(buffer);

            deferral.Complete();
        }

        private async void SensorsCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var request = await args.GetRequestAsync();

            IBuffer buffer = PrepareSensorBuffer().AsBuffer();

            request.RespondWithValue(buffer);

            deferral.Complete();

        }



        public override void Stop()
        {
            base.Stop();

            try
            {
                serviceProvider?.StopAdvertising();
            }
            catch (Exception)
            {

            }
        }
    }
}
