using static PocketBookClient.Services.Communication;
namespace PocketBookClient.Services
{
    public interface IScheduleService
    {
         Task<bool> ExecuteSheduleTransaction();
         bool ExistTaskToBeExecute => false;
    }
}
