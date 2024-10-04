namespace UserService.EventsBus.Model
{
    public class UserDeleteMessage
    {
        public string  UserId { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
