namespace PCG
{
    public static class GlobalConfiguration
    {
        public static int MinRoomSpacePadding { get; set; } = 10;

        //RoomSpace configuration
        public static int MaxWidth { get; set; } = 50;
        public static int MaxHeight { get; set; } = 200;

        //Room configuration
        public static int MaxRoomWidth { get; set; } = 4;
        public static int MinRoomWidth { get; set; } = 2;
        public static int MaxRoomHeight { get; set; } = 5;
        public static int MinRoomHeight { get; set; } = 3;
    }
}