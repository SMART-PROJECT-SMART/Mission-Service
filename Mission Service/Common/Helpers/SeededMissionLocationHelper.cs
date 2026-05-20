using Core.Models;
using Mission_Service.Common.Constants;

namespace Mission_Service.Common.Helpers
{
    public static class SeededMissionLocationHelper
    {
        public static Location ScaleDestinationFromPathOrigin(
            double pathOriginLatitude,
            double pathOriginLongitude,
            double pathOriginAltitude,
            double baselineDestinationLatitude,
            double baselineDestinationLongitude,
            double baselineDestinationAltitude
        )
        {
            double scaleFactor = MissionServiceConstants
                .DevSeededMissionPath
                .DESTINATION_DISTANCE_SCALE_FACTOR;
            double deltaLatitude =
                baselineDestinationLatitude - pathOriginLatitude;
            double deltaLongitude =
                baselineDestinationLongitude - pathOriginLongitude;
            double deltaAltitude =
                baselineDestinationAltitude - pathOriginAltitude;
            return new Location(
                pathOriginLatitude + scaleFactor * deltaLatitude,
                pathOriginLongitude + scaleFactor * deltaLongitude,
                pathOriginAltitude + scaleFactor * deltaAltitude
            );
        }
    }
}
