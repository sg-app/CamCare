using CamCare.Domain;
using CamCare.Interfaces.Services;
using CamCare.Models;

namespace CamCare.Services
{
    public class Mapper : IMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            var destination = new TDestination();
            Map(source, destination);
            return destination;
        }

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            switch (source)
            {
                case Address a when destination is AddressVm vm:
                    vm.Id = a.Id;
                    vm.CustomerId = a.CustomerId;
                    vm.AddressTypeId = a.AddressTypeId;
                    vm.Street = a.Street;
                    vm.HouseNumber = a.HouseNumber;
                    vm.PostalCode = a.PostalCode;
                    vm.City = a.City;
                    vm.State = a.State;
                    vm.Country = a.Country;
                    vm.CreatedAt = a.CreatedAt;
                    vm.UpdatedAt = a.UpdatedAt;
                    break;
                case AddressVm vm when destination is Address a:
                    a.Id = vm.Id;
                    a.CustomerId = vm.CustomerId;
                    a.AddressTypeId = vm.AddressTypeId;
                    a.Street = vm.Street;
                    a.HouseNumber = vm.HouseNumber;
                    a.PostalCode = vm.PostalCode;
                    a.City = vm.City;
                    a.State = vm.State;
                    a.Country = vm.Country;
                    a.CreatedAt = vm.CreatedAt;
                    a.UpdatedAt = vm.UpdatedAt;
                    break;
                case AddressType at when destination is AddressTypeVm atvm:
                    atvm.Id = at.Id;
                    atvm.Name = at.Name;
                    atvm.CreatedAt = at.CreatedAt;
                    atvm.UpdatedAt = at.UpdatedAt;
                    break;
                case AddressTypeVm atvm when destination is AddressType at:
                    at.Id = atvm.Id;
                    at.Name = atvm.Name;
                    at.CreatedAt = atvm.CreatedAt;
                    at.UpdatedAt = atvm.UpdatedAt;
                    break;
                case Customer c when destination is CustomerVm cvm:
                    cvm.Id = c.Id;
                    cvm.FirstName = c.FirstName;
                    cvm.LastName = c.LastName;
                    cvm.Email = c.Email;
                    cvm.Phone = c.Phone;
                    cvm.CreatedAt = c.CreatedAt;
                    cvm.UpdatedAt = c.UpdatedAt;
                    break;
                case CustomerVm cvm when destination is Customer c:
                    c.Id = cvm.Id;
                    c.FirstName = cvm.FirstName;
                    c.LastName = cvm.LastName;
                    c.Email = cvm.Email;
                    c.Phone = cvm.Phone;
                    c.CreatedAt = cvm.CreatedAt;
                    c.UpdatedAt = cvm.UpdatedAt;
                    break;
                case Parameter p when destination is ParameterVm pvm:
                    pvm.Key = p.Key;
                    pvm.Value = p.Value;
                    pvm.CreatedAt = p.CreatedAt;
                    pvm.UpdatedAt = p.UpdatedAt;
                    break;
                case ParameterVm pvm when destination is Parameter p:
                    p.Key = pvm.Key;
                    p.Value = pvm.Value;
                    p.CreatedAt = pvm.CreatedAt;
                    p.UpdatedAt = pvm.UpdatedAt;
                    break;
                case RepairOrder ro when destination is RepairOrderVm rovm:
                    rovm.Id = ro.Id;
                    rovm.CustomerId = ro.CustomerId;
                    rovm.CreatedAt = ro.CreatedAt;
                    rovm.UpdatedAt = ro.UpdatedAt;
                    break;
                case RepairOrderVm rovm when destination is RepairOrder ro:
                    ro.Id = rovm.Id;
                    ro.CustomerId = rovm.CustomerId;
                    ro.CreatedAt = rovm.CreatedAt;
                    ro.UpdatedAt = rovm.UpdatedAt;
                    break;
                case RepairOrderRepairPosition rorp when destination is RepairOrderRepairPositionVm rorpvm:
                    rorpvm.RepairOrderId = rorp.RepairOrderId;
                    rorpvm.RepairPositionId = rorp.RepairPositionId;
                    rorpvm.DisplayOrder = rorp.DisplayOrder;
                    break;
                case RepairOrderRepairPositionVm rorpvm when destination is RepairOrderRepairPosition rorp:
                    rorp.RepairOrderId = rorpvm.RepairOrderId;
                    rorp.RepairPositionId = rorpvm.RepairPositionId;
                    rorp.DisplayOrder = rorpvm.DisplayOrder;
                    break;
                case RepairOrderStatus ros when destination is RepairOrderStatusVm rosvm:
                    rosvm.Id = ros.Id;
                    rosvm.Name = ros.Name;
                    rosvm.Description = ros.Description;
                    rosvm.Order = ros.Order;
                    rosvm.BackgroundColor = ros.BackgroundColor;
                    rosvm.FontColor = ros.FontColor;
                    rosvm.CreatedAt = ros.CreatedAt;
                    rosvm.UpdatedAt = ros.UpdatedAt;
                    break;
                case RepairOrderStatusVm rosvm when destination is RepairOrderStatus ros:
                    ros.Id = rosvm.Id;
                    ros.Name = rosvm.Name;
                    ros.Description = rosvm.Description;
                    ros.Order = rosvm.Order;
                    ros.BackgroundColor = rosvm.BackgroundColor;
                    ros.FontColor = rosvm.FontColor;
                    ros.CreatedAt = rosvm.CreatedAt;
                    ros.UpdatedAt = rosvm.UpdatedAt;
                    break;
                case RepairPosition rp when destination is RepairPositionVm rpvm:
                    rpvm.Id = rp.Id;
                    rpvm.Description = rp.Description;
                    rpvm.CreatedAt = rp.CreatedAt;
                    rpvm.UpdatedAt = rp.UpdatedAt;
                    break;
                case RepairPositionVm rpvm when destination is RepairPosition rp:
                    rp.Id = rpvm.Id;
                    rp.Description = rpvm.Description;
                    rp.CreatedAt = rpvm.CreatedAt;
                    rp.UpdatedAt = rpvm.UpdatedAt;
                    break;
                default:
                    throw new NotSupportedException($"Mapping from {typeof(TSource)} to {typeof(TDestination)} is not supported.");
            }
        }
    }
}
