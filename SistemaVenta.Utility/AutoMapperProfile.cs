using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.Model.Models;
using SistemaVenta.DTO;
using System.Globalization;
namespace SistemaVenta.Utility
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {

            #region Auth
            CreateMap<AuthDTO, Usuario>().ReverseMap();
            //CreateMap<Usuario, SesionDTO>()
         //     .ForMember(dest => dest.RolDescripcion, opt => opt.MapFrom(src => src.IdRolNavigation.Nombre))
         //     .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Foto)); // Se agregó el campo Foto

            #endregion Auth

            #region Token
            CreateMap<TokenDTO, Usuario>().ReverseMap();
            #endregion Token

            #region Rol
            CreateMap<Rol, RolDTO>().ReverseMap();
            #endregion Rol

            #region Menu
            CreateMap<Menu, MenuDTO>().ReverseMap();
            #endregion Menu

            #region Usuario
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo == true ? 1 : 0))
                .ForMember(dest => dest.foto, opt => opt.MapFrom(src => src.foto)); // Se agregó el campo Foto
                

            CreateMap<Usuario, SesionDTO>()
                .ForMember(dest => dest.RolDescripcion, opt => opt.MapFrom(src => src.IdRolNavigation.Nombre))
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.foto)); // Se agregó el campo Foto

            CreateMap<UsuarioDTO, Usuario>()
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo == 1 ? true : false))
                .ForMember(dest => dest.foto, opt => opt.MapFrom(src => src.foto)); // Se agregó el campo Foto
            #endregion Usuario


            #region Categoria
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            #endregion Categoria

           
            #region Producto
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dest => dest.DescripcionCategoria, opt => opt.MapFrom(src => src.IdCategoriaNavigation.Nombre))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => Convert.ToString(src.Precio, new CultureInfo("es-EC"))))
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo == true ? 1 : 0))
                .ForMember(dest => dest.foto, opt => opt.MapFrom(src => src.Foto));  // 🔥 Nuevo campo Foto

            CreateMap<ProductoDTO, Producto>()
                .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => Convert.ToDecimal(src.Precio, new CultureInfo("es-EC"))))
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo == 1 ? true : false))
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.foto));  // 🔥 Nuevo campo Foto
            #endregion Producto


            

            #region Venta
            CreateMap<Venta, VentaDTO>()
                .ForMember(dest => dest.TotalTexto, opt => opt.MapFrom(src => Convert.ToString(src.Total.Value, new CultureInfo("es-EC"))))
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => src.FechaRegistro.Value.ToString("dd/MM/yyyy")));

            CreateMap<VentaDTO, Venta>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => Convert.ToDecimal(src.TotalTexto, new CultureInfo("es-EC"))));
            #endregion Venta

            #region DetalleVenta
            CreateMap<DetalleVenta, DetalleVentaDTO>()
                .ForMember(dest => dest.DescripcionProducto, opt => opt.MapFrom(src => src.IdProductoNavigation.Nombre))
                .ForMember(dest => dest.PrecioTexto, opt => opt.MapFrom(src => Convert.ToString(src.Precio.Value, new CultureInfo("es-EC"))))
                .ForMember(dest => dest.TotalTexto, opt => opt.MapFrom(src => Convert.ToString(src.Total.Value, new CultureInfo("es-EC"))));

            CreateMap<DetalleVentaDTO, DetalleVenta>()
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => Convert.ToDecimal(src.PrecioTexto, new CultureInfo("es-EC"))))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => Convert.ToDecimal(src.TotalTexto, new CultureInfo("es-EC"))));
            #endregion DetalleVenta

            #region ReporteVenta
            CreateMap<DetalleVenta, ReporteDTO>()
                  .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => src.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))
                  .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src => src.IdVentaNavigation.NumeroDocumento))
                  .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.IdVentaNavigation.TipoPago))
                  .ForMember(dest => dest.TotalVenta, opt => opt.MapFrom(src => Convert.ToString(src.IdVentaNavigation.Total.Value, new CultureInfo("es-EC"))))
                  .ForMember(dest => dest.Producto, opt => opt.MapFrom(src => src.IdProductoNavigation.Nombre))
                  .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => Convert.ToString(src.Precio.Value, new CultureInfo("es-EC"))))
                  .ForMember(dest => dest.Total, opt => opt.MapFrom(src => Convert.ToString(src.Total.Value, new CultureInfo("es-EC"))));



            #endregion ReporteVenta
        }
    }
}
