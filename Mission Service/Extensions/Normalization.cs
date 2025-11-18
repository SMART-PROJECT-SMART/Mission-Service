using Core.Common.Enums;
using Mission_Service.Common.Constants;

namespace Mission_Service.Extensions
{
    public static class Normalization
    {
        public static double NormalizeTelemetryValue(this TelemetryFields field, double value)
        {
            return field switch
            {
                TelemetryFields.FuelAmount => value / MissionServiceConstants.TelemetryNormalization.MAX_PERCENTAGE,
                TelemetryFields.ThrottlePercent => value / MissionServiceConstants.TelemetryNormalization.MAX_PERCENTAGE,
                TelemetryFields.CurrentSpeedKmph => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_SPEED_KMPH, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.Altitude => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_ALTITUDE, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.CruiseAltitude => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_ALTITUDE, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.SignalStrength => (value + MissionServiceConstants.TelemetryNormalization.SIGNAL_STRENGTH_OFFSET) / MissionServiceConstants.TelemetryNormalization.SIGNAL_STRENGTH_RANGE,
                TelemetryFields.DataStorageUsedGB => MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE - Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_DATA_STORAGE_GB, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.FlightTimeSec => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_FLIGHT_TIME_SEC, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.ThrustAfterInfluence => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_THRUST, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.EngineDegrees => MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE - Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_ENGINE_DEGREES, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.Rpm => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_RPM, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.YawDeg => value / MissionServiceConstants.TelemetryNormalization.FULL_ROTATION_DEG,
                TelemetryFields.PitchDeg => (value + MissionServiceConstants.TelemetryNormalization.PITCH_ROLL_OFFSET) / MissionServiceConstants.TelemetryNormalization.PITCH_ROLL_RANGE,
                TelemetryFields.RollDeg => (value + MissionServiceConstants.TelemetryNormalization.PITCH_ROLL_OFFSET) / MissionServiceConstants.TelemetryNormalization.PITCH_ROLL_RANGE,
                TelemetryFields.DragCoefficient => MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE - Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_DRAG_COEFFICIENT, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.LiftCoefficient => Math.Min(value / MissionServiceConstants.TelemetryNormalization.MAX_LIFT_COEFFICIENT, MissionServiceConstants.TelemetryNormalization.MIN_NORMALIZED_VALUE),
                TelemetryFields.LandingGearStatus => value,
                TelemetryFields.Latitude => (value + MissionServiceConstants.TelemetryNormalization.LATITUDE_OFFSET) / MissionServiceConstants.TelemetryNormalization.LATITUDE_RANGE,
                TelemetryFields.Longitude => (value + MissionServiceConstants.TelemetryNormalization.LONGITUDE_OFFSET) / MissionServiceConstants.TelemetryNormalization.LONGITUDE_RANGE,
                _ => MissionServiceConstants.TelemetryNormalization.DEFAULT_NORMALIZED_VALUE
            };
        }
    }
}
