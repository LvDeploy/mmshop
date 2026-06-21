namespace MusicMasterShop.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected void SetUpdateDate(DateTime datetime)
        {
            UpdatedAt = datetime;
        }

        protected void SetCreateDate(DateTime datetime)
        {
            CreatedAt = datetime;
        }
    }
}
