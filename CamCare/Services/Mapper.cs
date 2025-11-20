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
                case Camera cam when destination is CameraVm camVm:
                    camVm.SerialNumber = cam.SerialNumber;
                    camVm.CustomerId = cam.CustomerId;
                    camVm.CameraTypeId = cam.CameraTypeId;
                    camVm.CreatedAt = cam.CreatedAt;
                    camVm.UpdatedAt = cam.UpdatedAt;
                    if (cam.CameraType != null)
                        camVm.CameraType = Map<CameraType, CameraTypeVm>(cam.CameraType);
                    break;
                case CameraVm camVm when destination is Camera cam:
                    cam.SerialNumber = camVm.SerialNumber;
                    cam.CustomerId = camVm.CustomerId;
                    cam.CameraTypeId = camVm.CameraTypeId;
                    cam.CreatedAt = camVm.CreatedAt;
                    cam.UpdatedAt = camVm.UpdatedAt;
                    break;
                case RepairOrder ro when destination is RepairOrderVm rovm:
                    rovm.Id = ro.Id;
                    rovm.CustomerId = ro.CustomerId;
                    rovm.CameraSerialNumber = ro.SerialNumber;
                    rovm.PiceOfEquipment = ro.PiceOfEquipment;
                    rovm.RepairOrderStatusId = ro.RepairOrderStatusId;
                    rovm.ShippingMethod = ro.ShippingMethod;
                    rovm.LogisticProviderId = ro.LogisticProviderId;
                    rovm.OrderNumber = ro.OrderNumber;
                    rovm.QuoteNumber = ro.QuoteNumber;
                    rovm.DeliveryNoteNumber = ro.DeliveryNoteNumber;
                    rovm.ArrivedAt = ro.ArrivedAt.ToLocalTime();
                    rovm.CreatedAt = ro.CreatedAt;
                    rovm.UpdatedAt = ro.UpdatedAt;
                    if (ro.RepairOrderStatus != null)
                        rovm.RepairOrderStatus = Map<RepairOrderStatus, RepairOrderStatusVm>(ro.RepairOrderStatus);
                    if (ro.RepairOrderStatusHistory != null)
                        rovm.RepairOrderStatusHistories = ro.RepairOrderStatusHistory.Select(Map<RepairOrderStatusHistory, RepairOrderStatusHistoryVm>).ToList();
                    if (ro.LogisticProvider != null)
                        rovm.LogisticProvider = Map<LogisticProvider, LogisticProviderVm>(ro.LogisticProvider);
                    if (ro.Defectives != null)
                        rovm.Defectives = ro.Defectives.Select(Map<Defective, DefectiveVm>).ToList();
                    if (ro.IncludedComponents != null)
                        rovm.IncludedComponents = ro.IncludedComponents.Select(Map<IncludedComponent, IncludedComponentVm>).ToList();
                    if (ro.RepairPositions != null)
                        rovm.RepairPositions = ro.RepairOrderRepairPositions.Select(Map<RepairOrderRepairPosition, RepairPositionVm>).ToList();
                    if (ro.Employees != null)
                        rovm.Employees = ro.Employees.Select(Map<Employee, EmployeeVm>).ToList();
                    break;
                case RepairOrderVm rovm when destination is RepairOrder ro:
                    ro.Id = rovm.Id;
                    ro.CustomerId = rovm.CustomerId ?? 0;
                    ro.SerialNumber = rovm.CameraSerialNumber;
                    ro.PiceOfEquipment = rovm.PiceOfEquipment;
                    ro.RepairOrderStatusId = rovm.RepairOrderStatusId;
                    ro.ShippingMethod = rovm.ShippingMethod;
                    ro.LogisticProviderId = rovm.LogisticProviderId;
                    ro.OrderNumber = rovm.OrderNumber;
                    ro.QuoteNumber = rovm.QuoteNumber;
                    ro.DeliveryNoteNumber = rovm.DeliveryNoteNumber;
                    ro.ArrivedAt = rovm.ArrivedAt.ToUniversalTime();
                    ro.CreatedAt = rovm.CreatedAt;
                    ro.UpdatedAt = rovm.UpdatedAt;
                    break;
                case RepairOrderRepairPosition rorp when destination is RepairPositionVm rpvm:
                    rpvm.Id = rorp.RepairPositionId;
                    rpvm.Artikelnummer = rorp.RepairPosition.Artikelnummer;
                    rpvm.Description = rorp.RepairPosition.Description;
                    rpvm.SortOrder = rorp.RepairPosition.SortOrder;
                    rpvm.Quantity = rorp.Quantity;
                    rpvm.CreatedAt = rorp.RepairPosition.CreatedAt;
                    rpvm.UpdatedAt = rorp.RepairPosition.UpdatedAt;
                    break;
                case RepairPositionVm rpvm when destination is RepairPosition rp:
                    rp.Id = rpvm.Id;
                    rp.Description = rpvm.Description;
                    rp.SortOrder = rpvm.SortOrder;
                    rp.FromAmicron = rpvm.FromAmicron;
                    rp.CreatedAt = rpvm.CreatedAt;
                    rp.UpdatedAt = rpvm.UpdatedAt;
                    if (rpvm.RepairOrders != null)
                        rp.RepairOrders = rpvm.RepairOrders.Select(Map<RepairOrderVm, RepairOrder>).ToList();
                    break;
                case RepairPosition rp when destination is RepairPositionVm rpvm:
                    rpvm.Id = rp.Id;
                    rpvm.Description = rp.Description;
                    rpvm.SortOrder = rp.SortOrder;
                    rpvm.FromAmicron = rp.FromAmicron;
                    rpvm.CreatedAt = rp.CreatedAt;
                    rpvm.UpdatedAt = rp.UpdatedAt;
                    if (rp.RepairOrders != null)
                        rpvm.RepairOrders = rp.RepairOrders.Select(Map<RepairOrder, RepairOrderVm>).ToList();
                    break;
                case RepairPositionVm rpvm when destination is RepairPosition rp:
                    rp.Id = rpvm.Id;
                    rp.Description = rpvm.Description;
                    rp.SortOrder = rpvm.SortOrder;
                    rp.CreatedAt = rpvm.CreatedAt;
                    rp.UpdatedAt = rpvm.UpdatedAt;
                    if (rpvm.RepairOrders != null)
                        rp.RepairOrders = rpvm.RepairOrders.Select(Map<RepairOrderVm, RepairOrder>).ToList();
                    break;
                case Defective def when destination is DefectiveVm defvm:
                    defvm.Id = def.Id;
                    defvm.Description = def.Description;
                    defvm.CreatedAt = def.CreatedAt;
                    defvm.UpdatedAt = def.UpdatedAt;
                    break;
                case DefectiveVm defvm when destination is Defective def:
                    def.Id = defvm.Id;
                    def.Description = defvm.Description;
                    break;
                case IncludedComponent includedComponent when destination is IncludedComponentVm includedComponentVm:
                    includedComponentVm.Id = includedComponent.Id;
                    includedComponentVm.Description = includedComponent.Description;
                    includedComponentVm.CreatedAt = includedComponent.CreatedAt;
                    includedComponentVm.UpdatedAt = includedComponent.UpdatedAt;
                    break;
                case IncludedComponentVm includedComponentVm when destination is IncludedComponent includedComponent:
                    includedComponent.Id = includedComponentVm.Id;
                    includedComponent.Description = includedComponentVm.Description;
                    break;
                case CameraType ct when destination is CameraTypeVm ctv:
                    ctv.Id = ct.Id;
                    ctv.Name = ct.Name;
                    break;
                case CameraTypeVm ctv when destination is CameraType ct:
                    ct.Id = ctv.Id;
                    ct.Name = ctv.Name;
                    break;
                case DataStore ds when destination is DataStoreVm dsvm:
                    dsvm.Id = ds.Id;
                    dsvm.RepairOrderId = ds.RepairOrderId;
                    dsvm.Filename = ds.Filename;
                    dsvm.Description = ds.Description;
                    dsvm.Data = ds.Data;
                    dsvm.Type = ds.Type;
                    break;
                case DataStoreVm dsvm when destination is DataStore ds:
                    ds.Id = dsvm.Id;
                    ds.RepairOrderId = dsvm.RepairOrderId;
                    ds.Filename = dsvm.Filename;
                    ds.Description = dsvm.Description;
                    ds.Data = dsvm.Data;
                    ds.Type = dsvm.Type;
                    break;
                case RepairOrderStatus ros when destination is RepairOrderStatusVm rosvm:
                    rosvm.Id = ros.Id;
                    rosvm.Name = ros.Name;
                    rosvm.Description = ros.Description;
                    rosvm.Order = ros.Order;
                    rosvm.BackgroundColor = ros.BackgroundColor;
                    rosvm.FontColor = ros.FontColor;
                    rosvm.IsActive = ros.IsActive;
                    rosvm.IsDefault = ros.IsDefault;
                    rosvm.IsOrderClose = ros.IsOrderClose;
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
                    ros.IsActive = rosvm.IsActive;
                    ros.IsDefault = rosvm.IsDefault;
                    ros.IsOrderClose = rosvm.IsOrderClose;
                    ros.CreatedAt = rosvm.CreatedAt;
                    ros.UpdatedAt = rosvm.UpdatedAt;
                    break;
                case RepairOrderStatusHistory history when destination is RepairOrderStatusHistoryVm vm:
                    vm.Id = history.Id;
                    vm.RepairOrderId = history.RepairOrderId;
                    vm.RepairOrderStatusId = history.RepairOrderStatusId;
                    vm.ChangedAt = history.ChangedAt.ToLocalTime();
                    if (history.RepairOrderStatus != null)
                        vm.RepairOrderStatus = Map<RepairOrderStatus, RepairOrderStatusVm>(history.RepairOrderStatus);
                    break;
                case RepairOrderStatusHistoryVm vm when destination is RepairOrderStatusHistory history:
                    history.Id = vm.Id;
                    history.RepairOrderId = vm.RepairOrderId;
                    history.RepairOrderStatusId = vm.RepairOrderStatusId;
                    history.ChangedAt = vm.ChangedAt.ToUniversalTime();
                    break;
                case LogisticProvider lp when destination is LogisticProviderVm lpvm:
                    lpvm.Id = lp.Id;
                    lpvm.Name = lp.Name;
                    lpvm.IsDefault = lp.IsDefault;
                    lpvm.IsActive = lp.IsActive;
                    lpvm.CreatedAt = lp.CreatedAt;
                    lpvm.UpdatedAt = lp.UpdatedAt;
                    break;
                case LogisticProviderVm lpvm when destination is LogisticProvider lp:
                    lp.Id = lpvm.Id;
                    lp.Name = lpvm.Name;
                    lp.IsDefault = lpvm.IsDefault;
                    lp.IsActive = lpvm.IsActive;
                    break;
                case Employee e when destination is EmployeeVm evm:
                    evm.Id = e.Id;
                    evm.FirstName = e.FirstName;
                    evm.LastName = e.LastName;
                    break;
                case EmployeeVm evm when destination is Employee e:
                    e.Id = evm.Id;
                    e.FirstName = evm.FirstName;
                    e.LastName = evm.LastName;
                    break;
                case Krd_Data d when destination is Krd_DataVm vm:
                    vm.CameraSerial = d.CameraSerial;
                    vm.Description = d.Description;
                    vm.CameraType = d.CameraType;
                    vm.AdditionalComponents = d.AdditionalComponents;
                    vm.AmicronNumbers = d.AmicronNumbers;
                    vm.ArrivedAt = d.ArrivedAt;
                    vm.Kunde = d.Kunde;
                    vm.Techniker = d.Techniker;
                    break;
                default:
                    throw new NotSupportedException($"Mapping from {typeof(TSource)} to {typeof(TDestination)} is not supported.");
            }
        }
    }
}
