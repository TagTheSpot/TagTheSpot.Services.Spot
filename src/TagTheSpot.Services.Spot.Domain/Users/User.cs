namespace TagTheSpot.Services.Spot.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; set; }

        public required string Email { get; set; }

        public Role Role { get; set; }
    }
}
