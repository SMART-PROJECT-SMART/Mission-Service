using Core.Common.Enums;

namespace Mission_Service.Extensions
{
    public static class Normalization
    {
        public static double NormalizeTelemetryValue(this TelemetryFields field, double value)
        {
            return field switch
            {
                TelemetryFields.FuelAmount => value / 100.0,
                TelemetryFields.ThrottlePercent => value / 100.0,
                TelemetryFields.CurrentSpeedKmph => Math.Min(value / 4095.0, 1.0),
                TelemetryFields.Altitude => Math.Min(value / 65535.0, 1.0),
                TelemetryFields.CruiseAltitude => Math.Min(value / 65535.0, 1.0),
                TelemetryFields.SignalStrength => (value + 200.0) / 400.0,
                TelemetryFields.DataStorageUsedGB => 1.0 - Math.Min(value / 10485.75, 1.0),
                TelemetryFields.FlightTimeSec => Math.Min(value / 1048575.0, 1.0),
                TelemetryFields.ThrustAfterInfluence => Math.Min(value / 1048575.0, 1.0),
                TelemetryFields.EngineDegrees => 1.0 - Math.Min(value / 524.287, 1.0),
                TelemetryFields.Rpm => Math.Min(value / 65535.0, 1.0),
                TelemetryFields.YawDeg => value / 360.0,
                TelemetryFields.PitchDeg => (value + 90.0) / 180.0,
                TelemetryFields.RollDeg => (value + 90.0) / 180.0,
                TelemetryFields.DragCoefficient => 1.0 - Math.Min(value / 6.5535, 1.0),
                TelemetryFields.LiftCoefficient => Math.Min(value / 6.5535, 1.0),
                TelemetryFields.LandingGearStatus => value,
                TelemetryFields.Latitude => (value + 90.0) / 180.0,
                TelemetryFields.Longitude => (value + 180.0) / 360.0,
                _ => 0.5
            };
        }
    }
}
