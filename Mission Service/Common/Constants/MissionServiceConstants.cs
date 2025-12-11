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
            public const int OFFSPRING_PAIR_SIZE = 2;
            public const int FIRST_OFFSPRING_INDEX = 0;
            public const int SECOND_OFFSPRING_INDEX = 1;
            public const double NO_IMPROVEMENT_THRESHOLD = 0.0001;
        }

        public static class Crossover
        {
            public const int MIN_CHROMOSOMES_FOR_CROSSOVER = 3;
            public const int MIN_CROSSOVER_POINT = 1;
            public const double GENE_SELECTION_PROBABILITY = 0.5;
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

        public static class APIResponses
        {
            public const string ASSIGNMENT_REQUEST_ACCEPTED =
                "Assignment request accepted and is being processed";
            public const string ASSIGNMENT_RESULT_NOT_FOUND =
                "Assignment result not found or not ready yet";
            public const string ASSIGNMENT_RESULT_READY = "Result is ready";
            public const string ASSIGNMENT_RESULT_PROCESSING = "Result is still processing";
        }

        public static class Controllers
        {
            public const string ASSIGNMENT_RESULT_CONTROLLER = "AssignmentResult";
        }

        public static class Actions
        {
            public const string CREATE_ASSIGNMENT_SUGGESTION = "create-assignment-suggestion";
            public const string STATUS = "status";
        }

        public static class HttpClients
        {
            public const string CALLBACK_HTTP_CLIENT = "AssignmentCallback";
        }
    }
}
