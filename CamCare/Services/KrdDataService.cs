using CamCare.Domain;
using CamCare.Extensions;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Radzen;
using System;

namespace CamCare.Services
{
    public class KrdDataService : IKrdDataService
    {
        private readonly IKrdDbContextFactory _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<KrdDataService> _logger;

        public KrdDataService(IKrdDbContextFactory contextFactory, IMapper mapper, ILogger<KrdDataService> logger)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<Paginated<Krd_DataVm>>> GetAllAsync(LoadDataArgs args)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var (totalCount, query) = context
                    .KrdData
                    .AsNoTracking()
                    .AsQueryable()
                    .LoadByLoadDataArgs(args, null);

                var items = await query.ToListAsync();
                var vms = items.Select(e => _mapper.Map<Krd_Data, Krd_DataVm>(e)).ToList();

                var paginated = new Paginated<Krd_DataVm>
                {
                    Items = vms,
                    TotalCount = totalCount
                };
                return ServiceResponse.Success(paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetAllAsync(LoadDataArgs)");
                return ServiceResponse.Failure<Paginated<Krd_DataVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }
    }
}
