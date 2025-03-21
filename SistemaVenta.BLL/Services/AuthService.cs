using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaVenta.BLL.Services.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model.Models;


namespace SistemaVenta.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IMapper _mapper;

        public AuthService(IGenericRepository<Usuario> usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<string> GenerarToken(Usuario usuario)
        {
            return await Task.Run(() =>
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Correo ?? ""),
                    new Claim(ClaimTypes.Role, usuario.IdRol.ToString() ?? "Usuario")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("N4v$8x&!FzQm@2Kg#LpR7*Wt%Xa9Bd3YsJkVc^1ZDhM6Tr!Po"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "http://localhost:5185",
                    audience: "http://localhost:5185",
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            });
        }

        public async Task<Usuario> ValidarUsuario(AuthDTO authDTO)
        {
            var usuario = await _usuarioRepository.
                Obtener(u =>
                u.Correo == authDTO.Username && u.Clave == authDTO.Password)
                ;

            return usuario;
        }
    }
}
