namespace RfmOpenThings
{
    internal interface IOpenThingsService
    {
        void StartListen();
        void StartIdentify(uint sensorId);
        void Stop();
    }
}
