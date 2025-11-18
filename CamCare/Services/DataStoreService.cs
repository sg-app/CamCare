using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CamCare.Services
{
    public class DataStoreService : DbService<DataStore, DataStoreVm>, IDataStoreService
    {
        public DataStoreService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger<DataStoreService> logger, NotificationService notificationService)
            : base(contextFactory, mapper, logger, notificationService)
        {
        }
    }
}
