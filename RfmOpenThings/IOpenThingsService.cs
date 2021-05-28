namespace RfmOpenThings
{
    internal interface IOpenThingsService
    {
        void StartListen();
        void StartIdentify(uint sensorId);
        void Stop();
        void StartOtaUpdate(uint sensorId, int outputPower, string hexFile);
        void StartIntervalUpdate(uint sensorId, uint interval, int outputPower);
    }
}
