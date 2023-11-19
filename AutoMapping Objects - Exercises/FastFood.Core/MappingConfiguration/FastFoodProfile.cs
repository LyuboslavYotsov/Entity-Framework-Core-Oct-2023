namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Core.ViewModels.Items;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Categories
            CreateMap<CreateCategoryInputModel, Category>()
                    .ForMember(c => c.Name, cim => cim.MapFrom(x => x.CategoryName));
            CreateMap<Category, CategoryAllViewModel>()
                .ForMember(c => c.Name, cam => cam.MapFrom(x => x.Name));
            CreateMap<Category, CreateItemViewModel>()
                .ForMember(c => c.CategoryId, cam => cam.MapFrom(x => x.Id));

            //Positions
            CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(rem => rem.PositionId, p => p.MapFrom(x => x.Id));

            //Employees
            CreateMap<RegisterEmployeeInputModel, Employee>();
            CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(eam => eam.Position, e => e.MapFrom(x => x.Position.Name));


            //Item
            CreateMap<CreateItemInputModel, Item>();
            CreateMap<Item, ItemsAllViewModels>()
                .ForMember(ivm => ivm.Category, i => i.MapFrom(x => x.Category.Name));

            //Orders
            CreateMap<Order, OrderAllViewModel>()
                .ForMember(ovm => ovm.Employee, o => o.MapFrom(x => x.Employee.Name))
                .ForMember(ovm => ovm.OrderId, o => o.MapFrom(x => x.Id));

            CreateMap<CreateOrderInputModel, Order>();
            





            
        }
    }
}
