namespace Mission_Service.Common.Constants
{
    public static class MissionServiceConstants
    {
        public static class Configuration
        {
            public const string ALGORITHM_CONFIG_SECTION = "AlgorithmConfig";
            public const string TELEMETRY_WEIGHTS_CONFIG_SECTION = "TelemetryWeights";
            public const string FITNESS_WEIGHTS_CONFIG_SECTION = "FitnessWeights";
        }

        public static class MainAlgorithm
        {
            public const int AMOUNT_OF_MUTATION_OPTIONS = 2;
            public const int MAX_REPAIR_ATTEMPTS = 5;
        }

        public static class TelemetryNormalization
        {
            public const double MAX_PERCENTAGE = 100.0;
            public const double MAX_SPEED_KMPH = 4095.0;
            public const double MAX_ALTITUDE = 65535.0;
            public const double SIGNAL_STRENGTH_OFFSET = 200.0;
            public const double SIGNAL_STRENGTH_RANGE = 400.0;
            public const double MAX_DATA_STORAGE_GB = 10485.75;
            public const double MAX_FLIGHT_TIME_SEC = 1048575.0;
            public const double MAX_THRUST = 1048575.0;
            public const double MAX_ENGINE_DEGREES = 524.287;
            public const double MAX_RPM = 65535.0;
            public const double FULL_ROTATION_DEG = 360.0;
            public const double PITCH_ROLL_OFFSET = 90.0;
            public const double PITCH_ROLL_RANGE = 180.0;
            public const double MAX_DRAG_COEFFICIENT = 6.5535;
            public const double MAX_LIFT_COEFFICIENT = 6.5535;
            public const double LATITUDE_OFFSET = 90.0;
            public const double LATITUDE_RANGE = 180.0;
            public const double LONGITUDE_OFFSET = 180.0;
            public const double LONGITUDE_RANGE = 360.0;
            public const double DEFAULT_NORMALIZED_VALUE = 0.5;
            public const double MIN_NORMALIZED_VALUE = 1.0;
        }
    }
}
