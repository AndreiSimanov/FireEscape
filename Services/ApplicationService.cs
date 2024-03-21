namespace FireEscape.Services
{
    public class ApplicationService
    {
        readonly DropboxRepository dropboxRepository;

        public ApplicationService(DropboxRepository dropboxRepository)
        {
            this.dropboxRepository = dropboxRepository;
        }
    }
}
