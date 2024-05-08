public class WeatherStationData
{
    public string EncryptedData { get; private set; }

    public WeatherStationData(string encryptedData)
    {
        EncryptedData = encryptedData;
    }
}