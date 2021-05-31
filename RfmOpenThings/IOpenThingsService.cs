namespace RfmOpenThings
{
    internal interface IOpenThingsService
    {
        void StartListen();
        void StartIdentify(uint sensorId);
        OperationResult Stop();
        void StartOtaUpdate(uint sensorId, int outputPower, string hexFile);
        void StartIntervalUpdate(uint sensorId, int outputPower, uint interval);
    }
}
